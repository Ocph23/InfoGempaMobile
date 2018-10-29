using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BMKG
{

    [XmlRoot("Infogempa")]
    public class InfoGempa
    {
        [XmlElement("gempa")]
        public List<Gempa> Gempa { get; set; }

       
    }


    [XmlRoot("Infotsunami")]
    public class Infotsunami
    {
        [XmlElement("gempa")]
        public List<Gempa> Gempa { get; set; }


    }



    [XmlRoot("gempa")]
    public class Gempa
    {
        private string lintang;
        private string bujur;

        [XmlElement("Tanggal")]
        public string Tanggal { get; set; }

        [XmlElement("Jam")]
        public string Jam { get; set; }

        [XmlElement("Lintang")]
        public string Lintang {
            get { return lintang;}
            set
            {
                lintang = value;
                if(!string.IsNullOrEmpty(value))
                {
                    var result = value.Split(' ');
                    if(result.Count()==2)
                    {
                        Latitude = Convert.ToDouble(result[0]);
                        switch (result[1].ToUpper())
                        {
                            case "LS":
                                Latitude=Latitude * -1;
                                Console.WriteLine(Latitude);
                                break;
                                
                            default:
                                break;
                        }
                    }
                }
            }

        }

        public point Point { get; set; }

        [XmlElement("Bujur")]
        public string Bujur {
            get { return bujur;}
            set
            {
                bujur = value;
                if (!string.IsNullOrEmpty(value))
                {
                    var result = value.Split(' ');
                    if (result.Count() == 2)
                    {
                        Longitude = Convert.ToDouble(result[0]);
                        switch (result[1].ToUpper())
                        {
                            case "BB":
                                Longitude = Longitude * -1;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }


        }

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

        [XmlElement("Posisi")]
        public string Posisi { get; set; }

        [XmlElement("Keterangan")]
        public string[] Keterangan { get; set; }

        [XmlElement("_symbol")]
        public string Symbol { get; set; }

        [XmlElement("Potensi")]
        public string Potensi { get; set; }


        [XmlElement("Area")]
        public string Area { get; set; }


        public string LastUpdatedBy { get; set; }
        public double Latitude { get;  set; }
        public double Longitude { get;  set; }
    }

}
