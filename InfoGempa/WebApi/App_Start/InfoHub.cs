using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Net;
using System.Xml;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Net.Http;
using System.Net.Http.Headers;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Web;
using FcmSharp.Settings;
using FcmSharp;
using FcmSharp.Requests;

namespace WebService
{
    public class Broadcaster
    {
        private readonly static Lazy<Broadcaster> _instance =
            new Lazy<Broadcaster>(() => new Broadcaster());
         // We're going to broadcast to all clients a maximum of 25 times per second
        private readonly TimeSpan BroadcastInterval =
            TimeSpan.FromMinutes(1);
        private readonly IHubContext _hubContext;
        private Timer _broadcastLoop;
        private Gempa _model;
        private bool _modelUpdated;
      //  private FcmClientSettings settings;

        public Broadcaster()
        {
            // Save our hub context so we can easily use it 
            // to send to its connected clients
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<InfoHub>();
            _model = new Gempa();
            _modelUpdated = false;

          //  settings = FileBasedFcmClientSettings.CreateFromFile("bmkg-f7780", AppDomain.CurrentDomain.BaseDirectory + "bmkg-f7780-firebase-adminsdk-3zfxv-f298fdfb18.json");

            // Start the broadcast loop
            _broadcastLoop = new Timer(
                BroadcastShape,
                null,
                BroadcastInterval,
                BroadcastInterval);
                             

        }


        public async void BroadcastShape(object state)
        {
 
            // Construct the Client:
            //using (var client = new FcmClient(settings))
            //{
            //    // Construct the Data Payload to send:
            //    var data = new Dictionary<string, string>()
            //    {
            //        {"A", "B"},
            //        {"C", "D"}
            //    };

            //    // The Message should be sent to the News Topic:
            //    var message = new FcmMessage()
            //    {          
            //        ValidateOnly = false,       
            //        Message = new Message
            //        {    Notification =new Notification() { Title="Judul",  Body="BODY"},
            //            Topic = "news"
            //        }
            //    };
            //    try
            //    {
            //        CancellationTokenSource cts = new CancellationTokenSource();

            //        // Send the Message and wait synchronously:
            //        var results = client.SendAsync(message, cts.Token).GetAwaiter().GetResult();

            //        // Print the Result to the Console:
            //        System.Console.WriteLine("Message ID = {0}", results.Name);
            //    }
            //    catch (Exception ex)
            //    {
                      
            //    }
            //    // Finally send the Message and wait for the Result:
               
                                   
            //}




            // No need to send anything if our model hasn't changed
            await Task.Delay(10);
            var result = GetGempaInfo();
            if (result != null)
            {
                if (_model == null)
                {
                    _model = result;
                    _modelUpdated = true;
                }

                else if (_model.Tanggal != result.Tanggal || _model.Jam != result.Jam)
                {
                    _model = result;
                    _modelUpdated = true;
                }

                _modelUpdated = true;
            }

            if (_modelUpdated)
            {
                _model.Jam = DateTime.Now.ToLongTimeString() + " WIT";

                _hubContext.Clients.AllExcept(_model.LastUpdatedBy).updateShape(_model);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.appcenter.ms");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/v0.1/apps/ocph23/TestApp/push/notifications");
                    var str = "{'notification_content' : { 'name' : 'test', 'title' : 'Gempa Dirasakan', 'body' : '" + _model.Wilayah1 + "', 'custom_data':{'sound' : 'alarm', 'waktu' : 'dimuka'}}}";
                    request.Content = new StringContent(str,
                                                        Encoding.UTF8,
                                                        "application/json");//CONTENT-TYPE header
                    request.Content.Headers.Add("x-api-token", "4b46ec5c9477e63a4701a8d1291b9f8c877eb9d1");
                    //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    await client.SendAsync(request)
                          .ContinueWith(responseTask =>
                          {
                              Console.WriteLine("Response: {0}", responseTask.Result);
                          });







                }


                _modelUpdated = false;
            }
        }
        public void UpdateShape(Gempa clientModel)
        {
            _model = clientModel;
            _modelUpdated = true;
        }
        public static Broadcaster Instance
        {
            get
            {
                return _instance.Value;
            }
        }



        private Gempa GetGempaInfo()
        {

            try
            {
                StringBuilder sb = new StringBuilder();
                string URLString = "http://data.bmkg.go.id/autogempa.xml";
                string xmlStr;
                using (var wc = new WebClient())
                {
                    xmlStr = wc.DownloadString(URLString);
                }
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlStr);
                var resut = (InfoGempa)ObjectToXML(xmlStr, typeof(InfoGempa));
                if (resut != null && resut.Gempas.Count > 0)
                    return resut.Gempas[0];
                return null;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public Object ObjectToXML(string xml, Type objectType)
        {
            StringReader strReader = null;
            XmlSerializer serializer = null;
            XmlTextReader xmlReader = null;
            Object obj = null;
            try
            {
                strReader = new StringReader(xml);
                serializer = new XmlSerializer(objectType);
                xmlReader = new XmlTextReader(strReader);
                obj = serializer.Deserialize(xmlReader);
            }
            catch (Exception exp)
            {
                //Handle Exception Code
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
                if (strReader != null)
                {
                    strReader.Close();
                }
            }
            return obj;
        }
    }


    public class InfoHub : Hub
    {
        private Timer timer;

        public object Last { get; private set; }

        public IHubContext HubContext => GlobalHost.ConnectionManager.GetHubContext<InfoHub>();

        private Broadcaster _broadcaster;
        public InfoHub()
            : this(Broadcaster.Instance)
        {
        }
        public InfoHub(Broadcaster broadcaster)
        {
            _broadcaster = broadcaster;
        }
        public void UpdateModel(Gempa clientModel)
        {
            clientModel.LastUpdatedBy = Context.ConnectionId;
            // Update the shape model within our broadcaster
            _broadcaster.UpdateShape(clientModel);
        }







    }


    [XmlRoot("Infogempa")]
    public class InfoGempa
    {
        [XmlElement("gempa")]
        public List<Gempa> Gempas { get; set; }

    }

    [XmlRoot("gempa")]
    public class Gempa
    {

        [XmlElement("Tanggal")]
        public string Tanggal { get; set; }

        [XmlElement("Jam")]
        public string Jam { get; set; }

        [XmlElement("Lintang")]
        public string Lintang { get; set; }

        public point Point { get; set; }

        [XmlElement("Bujur")]
        public string Bujur { get; set; }

        [XmlElement("Magnitude")]
        public string Magnitude { get; set; }

        [XmlElement("Kedalaman")]
        public string Kedalaman { get; set; }


        [XmlElement("Dirasakan")]
        public string Dirasakan { get; set; }

        [XmlElement("Wilayah")]
        public string Wilayah { get; set; }

        [XmlElement("Wilayah1")]
        public string Wilayah1 { get; set; }

        [XmlElement("Wilayah2")]
        public string Wilayah2 { get; set; }

        [XmlElement("Wilayah3")]
        public string Wilayah3 { get; set; }

        [XmlElement("Wilayah4")]
        public string Wilayah4 { get; set; }

        [XmlElement("Wilayah5")]
        public string Wilayah5 { get; set; }

        [XmlElement("Linkdetail")]
        public string Linkdetail { get; set; }

        [XmlElement("NamaPeta")]
        public string NamaPeta { get; set; }

        [XmlElement("Keterangan")]
        public List<string> Keterangan { get; set; }


        public string LastUpdatedBy { get; set; }
    }



    [XmlRoot("point")]
    public class point
    {
        public string coordinates { get; set; }
    }
    public static class MyExtention
    {
        public static T Deserializes<T>(this XmlDocument document) where T : class

        {

            XmlReader reader = new XmlNodeReader(document);

            var serializer = new XmlSerializer(typeof(T));

            T result = (T)serializer.Deserialize(reader);

            return result;

        }
    }



    public class notification_content

    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public custom_data custom_data { get; set; } = new custom_data();
    }

    public class custom_data
    {
        public string Key1 { get; set; }
        public string Key2 { get; set; }
    }
           
}