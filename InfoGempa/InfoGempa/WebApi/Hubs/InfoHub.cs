using Microsoft.AspNetCore.SignalR;
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

namespace WebApi.Hubs
{
    public class InfoHub : Hub
    {
        private Timer timer;

        public object Last { get; private set; }

        public InfoHub() {
            RunTimer();

        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }


        private async void RunTimer()
        {
            await Task.Delay(5000);
            timer = new Timer(TimerMethode, Last, 10000, 100000);
        }

        private void TimerMethode(object state)
        {
            PrintResult((InfoGempa)state);
        }

        private async void PrintResult(InfoGempa state)
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
            var resuts = (InfoGempa)ObjectToXML(xmlStr, typeof(InfoGempa));
                 var aContext = Startup.ConnectionManager.GetHubContext("infoHub");
            await Helper.GetContext().Clients.All.SendMessage("Server", resuts.Gempas.FirstOrDefault().Wilayah1);
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
}