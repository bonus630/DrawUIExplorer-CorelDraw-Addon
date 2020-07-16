using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Corel.Interop.VGCore;

namespace br.corp.bonus630.DrawUIExplorer
{
    public class CorelAutomation
    {
        private Application app;
        private Core core;
        public CorelAutomation(Application app, Core core)
        {
            this.app = app;
            this.core = core;
        }
        public string GetCaptionDocker(string guid)
        {
            app.FrameWork.ShowDocker(guid);
            string caption = app.FrameWork.Automation.GetCaptionText(guid);
            app.FrameWork.HideDocker(guid);
            return caption;
        }
        public string GetActiveMenuItemGuid(int index)
        {
#if X7
            return "";
#else
            return app.FrameWork.Automation.GetActiveMenuItemGuid(index);
#endif
        }
        public string a(string guid, string propertieName)
        {
            ICUIControlData data = app.FrameWork.Automation.GetControlData(guid);
            return data.GetValue(propertieName).ToString();
        }
        public string CommandBarModeName()
        {
            CommandBarModes modes = app.FrameWork.CommandBars[0].Modes;
            return modes[0].Name;
        }
        public System.Windows.Rect GetItemRect(string guidParent, string guidItem)
        {
            int left, top, width, height = 0;
            bool data = app.FrameWork.Automation.GetItemScreenRect(guidParent, guidItem, out left, out top, out width, out height);
            return new System.Windows.Rect() { X = left, Y = top, Width = width, Height = height };

        }
#if X9
        public string LoadLocalizedString(string guid)
        {

            return app.FrameWork.Application.LoadLocalizedString(guid);

        }
#endif
        public void b()
        {
            var a = app.FrameWork.CommandBars[0].Controls[0].Parameter;
        }
        public void ShowHideCommandBar(string commandBarName, bool show = true)
        {
            try
            {
                if (!string.IsNullOrEmpty(commandBarName))
                    this.app.FrameWork.CommandBars[commandBarName].Visible = show;
            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Console);
            }

        }
        public void CommandBarMode(string commandBarName, bool show = true)
        {
            try
            {
                if (!string.IsNullOrEmpty(commandBarName)) {
                    CommandBar commandBar = this.app.FrameWork.CommandBars[commandBarName];
                   // var ctr = this.app.FrameWork.Automation.GetControlData("");
                  // commandBar.Controls.Count
                   CommandBarModes modes = commandBar.Modes;
                    core.DispactchNewMessage("CommandBarMode: "+commandBar.Controls.Count.ToString(), MsgType.Console);
                }
            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Console);
            }
        }
        public string GetItemCaption(string guid)
        {
            string caption = "";
            caption = this.app.FrameWork.Automation.GetCaptionText(guid);
            return caption;


        }

        public void RunBindDataSource(string value)
        {
            string o = "";
            try
            {
                string pattern = @"(DataSource=(?<datasource>[a-zA-Z0-9]+);Path=(?<path>[0-9a-zA-Z]{0,}))";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(value);
                string datasource = match.Groups["datasource"].Value;
                string path = match.Groups["path"].Value;
                DataSourceProxy dsp = app.FrameWork.Application.DataContext.GetDataSource(datasource);
                object j = dsp.GetProperty(path);
                o = string.Format("Type:{0} Value:{1}",j.GetType().Name,j);
            }
            catch(Exception erro)
            {
                o = erro.Message;
            }
            core.DispactchNewMessage(o, MsgType.Console);
        }
        
        public void ShowDialog(string guid)
        {
#if !X7
            this.app.FrameWork.ShowDialog(guid);
#endif
        }
        public void c()
        {

        }
        public void RunMacro(string value)
        {
            try
            {
                string module = value.Substring(0, value.LastIndexOf("."));
                string macro = value.Substring(value.LastIndexOf(".") + 1, value.Length - (module.Length+1));
                this.app.GMSManager.RunMacro(module, macro);
            }
            catch(Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Console);
            }
        }
    }
}
