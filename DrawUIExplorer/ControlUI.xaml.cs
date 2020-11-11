using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using corel = Corel.Interop.VGCore;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Microsoft.Win32;

namespace br.corp.bonus630.DrawUIExplorer
{

    public partial class ControlUI : UserControl
    {
        private corel.Application corelApp = null;
        Views.XMLTagWindow xMLTagsForm;
        public static IntPtr corelHandle;
        
        private string currentTheme;
        public ControlUI(object app)
        {
            InitializeComponent();
            try
            {
                this.corelApp = app as corel.Application;
                this.corelApp.OnApplicationEvent += CorelApp_OnApplicationEvent;
                LoadThemeFromPreference();
            }
            catch (Exception)
            {
                global::System.Windows.MessageBox.Show("VGCore Erro");
            }
            string filePath = "";
//#if Debug
//             filePath = "C:\\Users\\bonus\\AppData\\Roaming\\Corel\\CorelDRAW Graphics Suite X8\\Draw\\Workspace\\_default.cdws";
//            CallXMLForm(filePath);
//             return;
//#endif
            btn_Command.Click += (s, e) => {
                OpenFileDialog of = new OpenFileDialog();
                of.Filter = "DrawUI|*.xml|Workspace file|*.cdws";
                if (!(bool)of.ShowDialog())
                    return;
                filePath = of.FileName;

                CallXMLForm(filePath);
            };
        }
        public async Task save()
        {
            await Task.Run(
                new Action(() => {
                    corelApp.ActiveDocument.Save();
                })
                );

        }
        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();
        private void CallXMLForm(string filePath)
        {
            xMLTagsForm = new Views.XMLTagWindow(this.corelApp, filePath);
            xMLTagsForm.Closed += (s, e) => { xMLTagsForm = null; };
            IntPtr ownerWindowHandler = GetFocus();
            ControlUI.corelHandle = ownerWindowHandler;
            WindowInteropHelper helper = new WindowInteropHelper(xMLTagsForm);
            helper.Owner = ownerWindowHandler;
            xMLTagsForm.Show();
            xMLTagsForm.StartProcess(filePath);
        }
#region theme select
        //Keys resources name follow the resource order to add a new value, order to works you need add 5 resources colors and Resources/Colors.xaml
        //1º is default, is the same name of StyleKeys string array
        //2º add LightestGrey. in start name of 1º for LightestGrey style in corel
        //3º MediumGrey
        //4º DarkGrey
        //5º Black
        private readonly string[] StyleKeys = new string[] {
         "ControlUI.Button.MouseOver.Background" ,
         "ControlUI.Button.MouseOver.Border",
         "ControlUI.Button.Static.Border" ,
         "ControlUI.Button.Static.Background" ,
         "ControlUI.Button.Pressed.Background" ,
         "ControlUI.Button.Pressed.Border" ,
         "ControlUI.Container.Static.Background",
         "ControlUI.Default.Static.Foreground" 

  };
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadThemeFromPreference();
        }
        public void LoadStyle(string name)
        {

            string style = name.Substring(name.LastIndexOf("_") + 1);
            for (int i = 0; i < StyleKeys.Length; i++)
            {
                this.Resources[StyleKeys[i]] = this.Resources[string.Format("{0}.{1}", style, StyleKeys[i])];
            }
        }
        private void CorelApp_OnApplicationEvent(string EventName, ref object[] Parameters)
        {
            if (EventName.Equals("WorkspaceChanged") || EventName.Equals("OnColorSchemeChanged"))
            {
                LoadThemeFromPreference();
            }
        }
        public void LoadThemeFromPreference()
        {
            try
            {
                string result = string.Empty;
#if X8
                 result = corelApp.GetApplicationPreferenceValue("WindowScheme", "Colors").ToString();
#endif
#if X9
                  result = corelApp.GetApplicationPreferenceValue("WindowScheme", "Colors").ToString();
#endif
#if X10
                  result = corelApp.GetApplicationPreferenceValue("WindowScheme", "Colors").ToString();
#endif
#if X11
                  result = corelApp.GetApplicationPreferenceValue("WindowScheme", "Colors").ToString();
#endif
#if X12
                  result = corelApp.GetApplicationPreferenceValue("WindowScheme", "Colors").ToString();
#endif
                if (!result.Equals(currentTheme))
                {
                    currentTheme = result;
                    LoadStyle(currentTheme);
                }
            }
            catch { }

        }
#endregion
    }
}
