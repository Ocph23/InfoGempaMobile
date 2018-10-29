using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace BMKG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_ClickAsync(object sender, RoutedEventArgs e)
        {

            GetCuaca();
            LoadDataAsync();


        }

        private async void LoadDataAsync()
        {

      //      var autoGempa = await GempaDirasakan<InfoGempa>("http://data.bmkg.go.id/autogempa.xml");
      //      var terkini = await GempaDirasakan<InfoGempa>("http://data.bmkg.go.id/gempaterkini.xml");
 //           var dirasakan = await GempaDirasakan<InfoGempa>("http://data.bmkg.go.id/gempadirasakan.xml");
            var last = await GempaDirasakan<InfoGempa>("http://data.bmkg.go.id/lastgempadirasakan.xml");
            var tsunami = await GempaDirasakan<Infotsunami>("http://data.bmkg.go.id/lasttsunami.xml");
        }

        private Task<T> GempaDirasakan<T>(string Url)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string URLString = Url;
                string xmlStr;
                using (var wc = new WebClient())
                {
                    xmlStr = wc.DownloadString(URLString);
                }
                var xml=  xmlStr.Replace("Gempa", "gempa");
                var last =(T)ObjectToXML(xml, typeof(T));
                return Task.FromResult(last);
                //   return resut;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        private Task<InfoGempa> GetDataGempa( string Url)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string URLString = Url;
                string xmlStr;
                using (var wc = new WebClient())
                {
                    xmlStr = wc.DownloadString(URLString);
                }
              
                var resut = (InfoGempa)ObjectToXML(xmlStr, typeof(InfoGempa));
                return Task.FromResult(resut);
            }
            catch (Exception)
            {

                return null;
            }

        }

        private InfoGempa Autogempa()
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
                return resut;
            }
            catch (Exception)
            {

                return null;
            }

        }

        private InfoGempa  GempaTerkini()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string URLString = "http://data.bmkg.go.id/gempaterkini.xml";
                string xmlStr;
                using (var wc = new WebClient())
                {
                    xmlStr = wc.DownloadString(URLString);
                }
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlStr);
                var resut = (InfoGempa)ObjectToXML(xmlStr, typeof(InfoGempa));
                return resut;
            }
            catch (Exception ex)
            {

                return null;
            }
        }


        private void GetCuaca()
        {
            try
            {
                string URLString = "http://data.bmkg.go.id/pesan.txt";
                string input = new WebClient().DownloadString(@"http://www.bmkg.go.id/cuaca/prakiraan-cuaca.bmkg?Kota=Jayapura&AreaID=501447&Prov=24");
                var doc = new HtmlDocument();
                doc.LoadHtml(input);
                List<DataCuaca> listData = new List<DataCuaca>();


                for (var i =2;i<=3;i++)
                {

                    var Data = new DataCuaca();
                    HtmlNode[] nodes = doc.DocumentNode.SelectNodes("//div[@id='TabPaneCuaca"+i+"'] //div[@class='service-block clearfix']").ToArray();

                    
                    var tanggals = doc.DocumentNode.SelectNodes("//a[@href='#TabPaneCuaca" + i + "']").FirstOrDefault();
                    Data.Tanggal = tanggals.InnerText;
                    foreach (var item in nodes)
                    {
                        var har = item.Descendants("h2").Select(x => x.InnerText.Trim()).ToArray();
                        var result = item.Descendants("div").Select(x => x.InnerText.Trim()).ToArray();
                        var data = result[1].Split('\n').Select(x => x.Trim()).ToArray()[1].Split("&deg;C".ToArray()).Where(x => !string.IsNullOrEmpty(x)).ToArray();

                        var hari = new cuaca();
                        hari.Nama = har[0];
                        hari.Kondisi = result[0];
                        hari.SuhuTerendah = data[0];
                        hari.Tertinggi = data[1];
                        hari.Kelembaban = data[2];
                        hari.Suhu = har[1].Split("&deg;C".ToArray()).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
                        Data.Datas.Add(hari);

                    }

                    listData.Add(Data);
                }

                          



            }
            catch (Exception ex)
            {

               
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


    class cuaca
    {
        public string Nama { get; internal set; }
        public string Suhu { get; internal set; }
        public string Kondisi { get; internal set; }
        public string SuhuTerendah { get; internal set; }
        public string Tertinggi { get; internal set; }
        public string Kelembaban { get; internal set; }
    }

    class DataCuaca
    {
        public List<cuaca> Datas { get; set; } = new List<cuaca>();
        public string Tanggal { get; internal set; }
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
