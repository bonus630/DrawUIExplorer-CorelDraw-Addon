using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using win = System.Windows;
using br.corp.bonus630.DrawUIExplorer.DataClass;
using Corel.Interop.VGCore;

namespace br.corp.bonus630.DrawUIExplorer.Models
{
    public class CorelAutomation
    {
        private Application corelApp;
        private Core core;
        public CorelAutomation(Application app, Core core)
        {
            this.corelApp = app;
            this.core = core;
        }
        public string GetCaptionDocker(string guid)
        {
            corelApp.FrameWork.ShowDocker(guid);
            string caption = corelApp.FrameWork.Automation.GetCaptionText(guid);
            corelApp.FrameWork.HideDocker(guid);
            return caption;
        }
      
        public string GetActiveMenuItemGuid(int index)
        {
#if X7
            return "";
#else
            return corelApp.FrameWork.Automation.GetActiveMenuItemGuid(index);
#endif
        }
        public string a(string guid, string propertieName)
        {
            ICUIControlData data = corelApp.FrameWork.Automation.GetControlData(guid);
            return data.GetValue(propertieName).ToString();
        }
        public string CommandBarModeName()
        {
            CommandBarModes modes = corelApp.FrameWork.CommandBars[0].Modes;
            return modes[0].Name;
        }
        public System.Windows.Rect GetItemRect(string guidParent, string guidItem)
        {
            int left, top, width, height = 0;
            bool data = corelApp.FrameWork.Automation.GetItemScreenRect(guidParent, guidItem, out left, out top, out width, out height);
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
            var a = corelApp.FrameWork.CommandBars[0].Controls[0].Parameter;
        }
        public void ShowHideCommandBar(string commandBarName, bool show = true)
        {
            try
            {
                if (!string.IsNullOrEmpty(commandBarName))
                    this.corelApp.FrameWork.CommandBars[commandBarName].Visible = show;
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
                    CommandBar commandBar = this.corelApp.FrameWork.CommandBars[commandBarName];
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
            caption = this.corelApp.FrameWork.Automation.GetCaptionText(guid);
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
                DataSourceProxy dsp = corelApp.FrameWork.Application.DataContext.GetDataSource(datasource);
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
            this.corelApp.FrameWork.ShowDialog(guid);
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
                this.corelApp.GMSManager.RunMacro(module, macro);
            }
            catch(Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Console);
          
            }
        }
        private void LoadHighLightForm(IBasicData itemData, IBasicData parentItemData, IBasicData specialData, win.DependencyObject dependencyObject)
        {
            WinAPI.SetFocus(ControlUI.corelHandle);
            WinAPI.SetForegroundWindow(ControlUI.corelHandle);

            Corel.Interop.VGCore.cdrWindowState state = corelApp.AppWindow.WindowState;

            corelApp.AppWindow.Activate();
            if (state == Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized || state == Corel.Interop.VGCore.cdrWindowState.cdrWindowMinimized)
                corelApp.AppWindow.WindowState = Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized;
            if (state == Corel.Interop.VGCore.cdrWindowState.cdrWindowRestore)
                corelApp.AppWindow.WindowState = Corel.Interop.VGCore.cdrWindowState.cdrWindowRestore;
            if (state == Corel.Interop.VGCore.cdrWindowState.cdrWindowNormal)
                corelApp.AppWindow.WindowState = Corel.Interop.VGCore.cdrWindowState.cdrWindowNormal;

            System.Windows.Rect rect = this.GetItemRect(parentItemData.Guid, itemData.Guid);


            if (rect.IsZero())
                return;
            Views.XMLTagWindow w = Core.FindParentControl<Views.XMLTagWindow>(dependencyObject) as Views.XMLTagWindow;
            w.Visibility = win.Visibility.Collapsed;
            OverlayForm form;
            if (corelApp.AppWindow.WindowState == Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized)
                form = new OverlayForm(rect);
            else
            {
                System.Windows.Rect rect2 = new System.Windows.Rect(corelApp.AppWindow.Left, corelApp.AppWindow.Top, corelApp.AppWindow.Width, corelApp.AppWindow.Height);
                form = new OverlayForm(rect, rect2);
            }

            form.Show();
            form.FormClosed += (s, e) =>
            {
                w.Visibility = win.Visibility.Visible;
                if (specialData != null)
                {
                    if (specialData.GetType() == typeof(CommandBarData))
                    {

                    }
                    if (specialData.GetType() == typeof(DockerData))
                    {
                        corelApp.FrameWork.HideDocker(specialData.Guid);
                    }
                    if (specialData.GetType() == typeof(DialogData))
                    {
#if !X7
                        corelApp.FrameWork.HideDialog(specialData.Guid);
#endif
                    }
                }
            };

        }
        public void showHighLightItem(Views.XMLTagWindow tagWindow, List<IBasicData> temp)
        {
            // string guidParent = "c2b44f69-6dec-444e-a37e-5dbf7ff43dae";
            //string guidItem = "fa65d0c1-879b-4ef5-9465-af09e00e91ab";
            try
            {
                string guidItem = temp[temp.Count - 1].Guid;
                IBasicData basicData = core.SearchItemContainsGuidRef(core.ListPrimaryItens, guidItem);

                while (string.IsNullOrEmpty(basicData.Guid) && basicData.Parent != null)
                {
                    basicData = basicData.Parent;
                }
                string guidParent = basicData.Guid;

                if (basicData.GetType() == typeof(CommandBarData))
                {
                    corelApp.FrameWork.Automation.ShowBar(guidParent, true);

                }
                if (basicData.GetType() == typeof(DockerData))
                {
                    corelApp.FrameWork.ShowDocker(guidParent);
                }
                if (basicData.GetType() == typeof(DialogData))
                {
#if !X7
                    corelApp.FrameWork.ShowDialog(guidParent);
#endif
                }
                WinAPI.SetFocus(ControlUI.corelHandle);
                WinAPI.SetForegroundWindow(ControlUI.corelHandle);

                Corel.Interop.VGCore.cdrWindowState state = corelApp.AppWindow.WindowState;

                corelApp.AppWindow.Activate();
                if (state == Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized || state == Corel.Interop.VGCore.cdrWindowState.cdrWindowMinimized)
                    corelApp.AppWindow.WindowState = Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized;
                else
                    corelApp.AppWindow.WindowState = Corel.Interop.VGCore.cdrWindowState.cdrWindowRestore;
                System.Windows.Rect rect = this.GetItemRect(guidParent, guidItem);
                if (rect.IsZero())
                    return;
                tagWindow.Visibility = win.Visibility.Collapsed;
                OverlayForm form;
                if (corelApp.AppWindow.WindowState == Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized)
                    form = new OverlayForm(rect);
                else
                {
                    System.Windows.Rect rect2 = new System.Windows.Rect(corelApp.AppWindow.Left, corelApp.AppWindow.Top, corelApp.AppWindow.Width, corelApp.AppWindow.Height);
                    form = new OverlayForm(rect, rect2);
                }
                form.Show();
                form.FormClosed += (s, e) => { tagWindow.Visibility = win.Visibility.Visible; };
            }

            catch (System.Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Console);
            }
        }
        public void showHighLightItem(win.DependencyObject dependencyObject,List<IBasicData> temp)
        {
            Views.XMLTagWindow w = Core.FindParentControl<Views.XMLTagWindow>(dependencyObject) as Views.XMLTagWindow;
            if (w == null)
            {
                core.DispactchNewMessage("Error tagWindow not found", MsgType.Console);
                return;
            }
            
            showHighLightItem(w, temp);
        }
    }
}
