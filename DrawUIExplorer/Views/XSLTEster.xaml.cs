using System.IO;
using System.Windows;
using System.Windows.Controls;
using br.corp.bonus630.DrawUIExplorer.ViewModels;

namespace br.corp.bonus630.DrawUIExplorer.Views
{
    /// <summary>
    /// Interaction logic for XSLTEster.xaml
    /// </summary>
    public partial class XSLTEster : UserControl
    {
        XSLTesterViewModel xSLTesterViewModel;

        public XSLTEster(Core core)
        {
            InitializeComponent();
            xSLTesterViewModel = new XSLTesterViewModel(core);
            this.DataContext = xSLTesterViewModel;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(xSLTesterViewModel.xslFile))
            {
                txt_xsl.Text = File.ReadAllText(xSLTesterViewModel.xslFile);
            }
            if (File.Exists(xSLTesterViewModel.xmlfile))
            {
                txt_xml.Text = File.ReadAllText(xSLTesterViewModel.xmlfile);
            }
        }


    }
}
