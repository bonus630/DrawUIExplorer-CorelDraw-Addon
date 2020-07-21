using Microsoft.Win32;
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
using br.corp.bonus630.DrawUIExplorer;
using System.IO.Packaging;
using System.Diagnostics;
using br.corp.bonus630.DrawUIExplorer.Views;

namespace Standalone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ////AttachByROT att = new AttachByROT();
            ////att.TestROT();
            ////this.Close();
            ////return;
            string filePath = "";
#if Debug
             filePath = "C:\\Users\\bonus\\AppData\\Roaming\\Corel\\CorelDRAW Graphics Suite X8\\Draw\\Workspace\\_default.cdws";
            start(filePath);
             return;
#endif
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "DrawUI|*.xml|Workspace file|*.cdws";
            if (!(bool)of.ShowDialog())
                return;
            filePath = of.FileName;
            start(filePath);
        }

        private void TagWindow_Closed(object sender, EventArgs e)
        {
            this.Close();
        }
        private void start(string filePath)
        {

            // string filePath = "C:\\Program Files\\Corel\\CorelDRAW Graphics Suite 2017\\Draw\\UIConfig\\DrawUI.xml";
            // string filePath = "C:\\Users\\Reginaldo\\AppData\\Roaming\\Corel\\CorelDRAW Graphics Suite 2018\\Draw\\Workspace\\_default.cdws";
            
            //Process[] processes = Process.GetProcessesByName("C")


            //Type pia_type = Type.GetTypeFromProgID("CorelDRAW.Application.18");
            //object app = Activator.CreateInstance(pia_type) as object;
           // Type.GetTypeFromHandle
           // (app as dynamic).Visible = true;
            XMLTagWindow tagWindow = new XMLTagWindow(filePath);
            tagWindow.Show();
            this.Visibility = Visibility.Hidden;
            tagWindow.StartProcess(filePath);
            tagWindow.Closed += TagWindow_Closed;
            //grid_main.Children.Add(tagWindow);
        }
    }
}
