using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace FastResampler
{
    public struct otodata
    {
        public string file;
        public double offset;
        public double consonant;
        public double cutoff;
        public double preutterance;
        public double overlap;
        public double vend;
        public double endlap;
        public double length;
    }
    public class Oto
    {
        
        private XmlDocument otoXml;
        public Dictionary<string, otodata> otoData = new Dictionary<string, otodata>();
        public const string otoVersion = "0.1";
        private LangPack lang;
        public Oto(string otoFile, LangPack lang) {
            otoXml = new XmlDocument();
            otoXml.Load(otoFile);
            this.lang = lang;
            this.initOto();
        }

        private void initOto(){
            XmlNodeList root = this.otoXml.GetElementsByTagName("oto");
            if (root.Count > 0)
            {
                if(((XmlElement)root[0]).GetAttribute("version") == otoVersion){
                    XmlNodeList settings = ((XmlElement)root[0]).GetElementsByTagName("setting");
                    foreach (XmlNode setting in settings)
                    {
                        try
                        {
                            XmlElement settingData = (XmlElement)setting;
                        }
                        catch (Exception e) { }
                    }
                    XmlNodeList tones = ((XmlElement)root[0]).GetElementsByTagName("tone");
                    otodata tempData = new otodata();
                    foreach (XmlNode tone in tones)
                    {
                        try
                        {
                            XmlElement toneData = (XmlElement)tone;
                            string alias = toneData.GetElementsByTagName("name")[0].InnerText;
                            tempData.file = toneData.GetAttribute("file");
                            tempData.length = Convert.ToDouble(toneData.GetElementsByTagName("length")[0].InnerText);
                            tempData.offset = Convert.ToDouble(toneData.GetElementsByTagName("offset")[0].InnerText);
                            tempData.consonant = Convert.ToDouble(toneData.GetElementsByTagName("consonant")[0].InnerText);
                            tempData.vend = Convert.ToDouble(toneData.GetElementsByTagName("vend")[0].InnerText);
                            tempData.preutterance = Convert.ToDouble(toneData.GetElementsByTagName("preutterance")[0].InnerText);
                            tempData.cutoff = Convert.ToDouble(toneData.GetElementsByTagName("cutoff")[0].InnerText);
                            tempData.overlap = Convert.ToDouble(toneData.GetElementsByTagName("overlap")[0].InnerText);
                            tempData.endlap = Convert.ToDouble(toneData.GetElementsByTagName("endlap")[0].InnerText);
                            otoData[alias] = tempData;
                        }
                        catch (Exception e) { }
                    }
                } else {
                    MessageBox.Show(this.lang.fetch("扩展oto版本过高，请使用最新版FastResampler。"));
                }
            }
        }

        public string getToneFile(string tone)
        {
            if (this.otoData.ContainsKey(tone))
            {
                return otoData[tone].file;
            }
            else
            {
                return tone + ".wav";
            }
        }

        public otodata getToneData(string tone)
        {
            if (this.otoData.ContainsKey(tone))
            {
                return otoData[tone];
            }
            else
            {
                return new otodata();
            }
        }
    }
}
