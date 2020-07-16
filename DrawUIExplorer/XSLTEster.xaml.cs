using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Xml.Xsl;
using System.IO;
using br.corp.bonus630.DrawUIExplorer.DataClass;

namespace br.corp.bonus630.DrawUIExplorer
{
    /// <summary>
    /// Interaction logic for XSLTEster.xaml
    /// </summary>
    public partial class XSLTEster : UserControl
    {
        XmlDocument xmlDoc;
        XslCompiledTransform xslCompiledTransform;
        IBasicData basicData;

        string path;
        string xslFile;
        string resultFile;
        string xmlfile ;

        public XSLTEster()
        {
            InitializeComponent();
        }
        Core core;
        public XSLTEster(Core core)
        {
            InitializeComponent();
            this.core = core;
            xmlDoc = new XmlDocument();
            xslCompiledTransform = new XslCompiledTransform(true);
             path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DrawUIExplorer");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
             xslFile = path + "\\temp.xslt";
             resultFile = path + "\\result.txt";
             xmlfile = path + "\\temp.xml";
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(File.Exists(xslFile))
            {
                txt_xsl.Text = File.ReadAllText(xslFile);
            }
            if (File.Exists(xmlfile))
            {
                txt_xml.Text = File.ReadAllText(xmlfile);
            }
        }
        public void Update(IBasicData basicData)
        {
            this.basicData = basicData;


        }
        
        //int level = 0;
        private void GenXmlText(IBasicData basicData)
        {
            txt_xml.Text = core.GetXml(basicData);
            return;
            //level++;
            txt_xml.Text += "<";
            txt_xml.Text += basicData.TagName;


            foreach (DataClass.Attribute item in basicData.Attributes)
            {
                txt_xml.Text += " ";
                txt_xml.Text += item.ToString();
            }
            txt_xml.Text += ">\n";
            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                for (int r = 0; r < (basicData.Childrens[i].TreeLevel - this.basicData.TreeLevel); r++)
                {
                    txt_xml.Text += "\t";
                }

                GenXmlText(basicData.Childrens[i]);
            }
            for (int r = 0; r < (basicData.TreeLevel - this.basicData.TreeLevel); r++)
            {
                txt_xml.Text += "\t";
            }
            txt_xml.Text += string.Format("</{0}>\n", basicData.TagName);

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
                File.WriteAllText(xslFile, txt_xsl.Text);
                File.WriteAllText(xmlfile, txt_xml.Text);
                xslCompiledTransform.Load(xslFile);
                xslCompiledTransform.Transform(xmlfile, resultFile);
                txt_result.Text = File.ReadAllText(resultFile);
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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            process();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            txt_xml.Text = string.Empty;
            GenXmlText(this.basicData);
        }

      
    }
}
