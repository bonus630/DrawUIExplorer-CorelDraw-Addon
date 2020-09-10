using br.corp.bonus630.DrawUIExplorer.DataClass;
using br.corp.bonus630.DrawUIExplorer.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace br.corp.bonus630.DrawUIExplorer.ViewModels
{
    class XSLTesterViewModel : ViewModelDataBase
    {
        XmlDocument xmlDoc;
        XslCompiledTransform xslCompiledTransform;

        string path;
        public string xslFile;
        string resultFile;
        public string xmlfile;

        private string xmlText;

        public string XmlText
        {
            get { return xmlText; }
            set { xmlText = value;NotifyPropertyChanged(); }
        }

        private string xslText;

        public string XslText
        {
            get { return xslText; }
            set { xslText = value; NotifyPropertyChanged(); }
        }
        private string resultText;

        public string ResultText
        {
            get { return resultText; }
            set { resultText = value; NotifyPropertyChanged(); }
        }

        public SimpleCommand GenXmlCommand { get { return new SimpleCommand(GenXmlText); } }
        public SimpleCommand ProcessCommand { get { return new SimpleCommand(process); } }

        public XSLTesterViewModel(Core core):base(core)
        {
            xmlDoc = new XmlDocument();
            xslCompiledTransform = new XslCompiledTransform(true);
            path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DrawUIExplorer");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            xslFile = path + "\\temp.xslt";
            resultFile = path + "\\result.txt";
            xmlfile = path + "\\temp.xml";
        }
        protected override void Update(IBasicData basicData)
        {
            CurrentBasicData = basicData;
        }

        //int level = 0;
        private void GenXmlText()
        {
            XmlText = core.GetXml(this.CurrentBasicData);
        }
        private void process()
        {
            try
            {
                string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DrawUIExplorer");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string xslFile = path + "\\temp.xslt";
                string resultFile = path + "\\result.txt";
                string xmlfile = path + "\\temp.xml";
                File.WriteAllText(xslFile, XslText);
                File.WriteAllText(xmlfile,XmlText);
                xslCompiledTransform.Load(xslFile);
                xslCompiledTransform.Transform(xmlfile, resultFile);
                ResultText = File.ReadAllText(resultFile);
            }
            catch (Exception erro) { this.core.DispactchNewMessage(erro.Message, MsgType.Console); }
        }
        private XmlReader CreateXmlReader(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            //text = text.Replace(@"\", "");
            BufferedStream stream = new BufferedStream(new MemoryStream());
            stream.Write(Encoding.ASCII.GetBytes(text), 0, text.Length);
            stream.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(stream);
            XmlReader reader = XmlReader.Create(sr);

            stream.Close();
            return reader;
        }


    }
}
