using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using win = System.Windows;
using br.corp.bonus630.DrawUIExplorer.DataClass;
using Corel.Interop.VGCore;
using System.Threading;

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
            core.DispactchNewMessage(string.Format("X:{0},Y:{1},W:{2},H:{3}", left, top,width, height), MsgType.Console);
            return new System.Windows.Rect() { X = left, Y = top, Width = width, Height = height };

        }
        public string GetCaption(string guid)
        {
            try
            {
                return this.corelApp.FrameWork.Automation.GetCaptionText(guid);
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }
        public void InvokeItem(IBasicData basicData)
        {
            try
            {
                string guid;
                if (!string.IsNullOrEmpty(basicData.Guid))
                    guid = basicData.Guid;
                else
                    guid = basicData.GuidRef;
                this.corelApp.FrameWork.Automation.InvokeItem(guid);
            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Console);
            }
        }

#if X9
        public string LoadLocalizedString(string guid)
        {

            return app.FrameWork.Application.LoadLocalizedString(guid);

        }
#endif
        //public void b()
        //{
        //    var a = corelApp.FrameWork.CommandBars[0].Controls[0].Parameter;
        //}
        public void ShowHideCommandBar(IBasicData basicData, bool show = true)
        {
            string commandBarCaption = GetItemCaption(basicData);

            this.ShowHideCommandBar(commandBarCaption, show);
        }
        public void ShowHideCommandBar(string commandBarName, bool show = true)
        {
            //How close a commandBar in a popup
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
        public void ShowBar(string guid)
        {
            try
            {
                this.corelApp.FrameWork.Automation.ShowBar(guid);
            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Console);
            }

        }
        public void CommandBarMode(IBasicData basicData, bool show = true)
        {
            string commandBarCaption = GetItemCaption(basicData);
            this.CommandBarMode(commandBarCaption, true);
        }
        public void CommandBarMode(string commandBarName, bool show = true)
        {
            try
            {
                if (!string.IsNullOrEmpty(commandBarName))
                {
                    CommandBar commandBar = this.corelApp.FrameWork.CommandBars[commandBarName];

                    // var ctr = this.app.FrameWork.Automation.GetControlData("");
                    // commandBar.Controls.Count
                    CommandBarModes modes = commandBar.Modes;
                    // string guid = corelApp.FrameWork.Automation.GetActiveMenuItemGuid(0);
                    //corelApp.FrameWork.Automation.InvokeDialogItem(GuidDialog, guiditem);
                    core.DispactchNewMessage("CommandBarMode: " + commandBar.Controls.Count.ToString(), MsgType.Console);
                }
            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Console);
            }
        }
        public string GetItemCaption(string guid)
        {
            try
            {
                string caption = "";
                caption = this.corelApp.FrameWork.Automation.GetCaptionText(guid);
                return caption;
            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Console);
            }
            return "";
        }
        public string GetItemCaption(IBasicData basicData)
        {
            try
            {
                string commandBarCaption = "";
                commandBarCaption = this.corelApp.FrameWork.Automation.GetCaptionText(basicData.Guid);
                if (string.IsNullOrEmpty(commandBarCaption))
                    commandBarCaption = core.TryGetAnyCaption(basicData);
                return commandBarCaption;
            }
            catch(Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Console);
            }
            return "";
        }
        public void InvokeItem(string itemGuid)
        {
            corelApp.FrameWork.Automation.InvokeItem(itemGuid);
        }
        public void InvokeDialogItem(string dialogGuid,string itemGuid)
        {
            corelApp.FrameWork.Automation.InvokeDialogItem(dialogGuid, itemGuid);
        }
        public void RunBindDataSource(string value,bool invoke = false)
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
                if (invoke) 
                {
                    dsp.InvokeMethod(path);
                }
                else
                {
                    object j = dsp.GetProperty(path);
                    o = string.Format("Type:{0} Value:{1}", j.GetType().Name, j);
                }
            }
            catch (Exception erro)
            {
                o = erro.Message;
            }
            core.DispactchNewMessage(o, MsgType.Console);
        }

        public void ShowDialog(string guid)
        {
            try
            {
#if !X7
                this.corelApp.FrameWork.ShowDialog(guid);
#endif
            }
            catch (Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Console);
            }


        }
        public void HideDialog(string guid)
        {

            try
            {
#if !X7
                this.corelApp.FrameWork.HideDialog(guid);
#endif
            }
            catch (Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Console);
            }


        }
        public void ShowDocker(string guid)
        {
            try
            {

                this.corelApp.FrameWork.ShowDocker(guid);

            }
            catch (Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Console);
            }


        }
        public void HideDocker(string guid)
        {

            try
            {

                this.corelApp.FrameWork.HideDocker(guid);

            }
            catch (Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Console);
            }


        }
        //public int GetCommandBarIndexByGuid(string guid)
        //{
        //    for (int i = 1; i <= corelApp.FrameWork.CommandBars.Count; i++)
        //    {
        //        CommandBar commandBar = corelApp.FrameWork.CommandBars[i];
        //        if (commandBar.Controls.Count == 0 || commandBar.Controls.Count != this.CommandBarMode()
        //            continue;
        //        string controlId = commandBar.Controls[1].ID;
        //        //var b = core.SearchItemContainsGuidRef(controlId);
        //        var b = core.SearchEngineGet.SearchItemFromGuid(core.ListPrimaryItens, guid, false);
        //        if (b.Childrens.Count == 1)
        //            b = b.Childrens[0];
        //        else
        //            continue;
        //        while (b.GetType() != typeof(CommandBarData))
        //        {
        //            b = b.Parent;
        //        }
        //        if (b.Guid == guid)
        //            return i;
        //    }
        //    return 0;
        //}
        public void RunMacro(string value)
        {
            try
            {
                string module = value.Substring(0, value.LastIndexOf("."));
                string macro = value.Substring(value.LastIndexOf(".") + 1, value.Length - (module.Length + 1));
                this.corelApp.GMSManager.RunMacro(module, macro,null);
            }
            catch (Exception e)
            {
                core.DispactchNewMessage(e.Message, MsgType.Console);

            }
        }
        public void ShowHighLightItem(List<IBasicData> temp)
        {
            //Thread th = new Thread(new ParameterizedThreadStart(showHighLightItem));
            //th.IsBackground = true;
            //th.Start(temp);
            showHighLightItem(temp);
        }
        private Thread RunInBackground(Action action)
        {
            Thread th = new Thread(new ThreadStart(action));
            th.IsBackground = true;
            th.Start();
            return th;
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
        private void showHighLightItem(object list )
        {
            List<IBasicData> temp = (List<IBasicData>)list;
            // string guidParent = "c2b44f69-6dec-444e-a37e-5dbf7ff43dae";
            //string guidItem = "fa65d0c1-879b-4ef5-9465-af09e00e91ab";
            try
            {
                
                string guidItem = temp[temp.Count - 1].Guid;
                string guidParent = "";
                if (string.IsNullOrEmpty(guidItem) && temp[temp.Count -1].TagName.Equals("item"))
                {
                    guidItem = temp[temp.Count - 1].GuidRef;
                }
                IBasicData basicData = core.SearchEngineGet.SearchItemContainsGuidRef(core.ListPrimaryItens, guidItem,false);
                if (basicData != null)
                {
                    while ((string.IsNullOrEmpty(basicData.Guid) && basicData.Parent != null) || basicData.GetType() == typeof(OtherData))
                    {
                        basicData = basicData.Parent;
                    }
                    
                }
                basicData = temp[temp.Count - 1];
                guidParent = basicData.Guid;
                Action<IBasicData,bool> restoration = null;
                if (basicData.GetType() == typeof(CommandBarData))
                {
                    //int index = GetCommandBarIndexByGuid(guidParent);
                    bool visible = true;
                    try
                    {
                        // if (index > 0)
                        // {
                        string commandBarName = GetItemCaption(guidParent);
                            CommandBar commandBar = corelApp.FrameWork.CommandBars[commandBarName];
                            visible = commandBar.Visible;
                        // }

                        //bool visible = corelApp.FrameWork.Automation.ShowBar(guidParent);
                        // Thread th = RunInBackground(new Action(()=> {
                        //    core.DispactchNewMessage(DateTime.Now.ToLongTimeString()+" -  Thread", MsgType.Console);
                        ShowHideCommandBar(commandBarName, true);
                         //   corelApp.FrameWork.Automation.ShowBar(guidParent, true);
                     //   }));
                        //Thread.Sleep(5000);
                        //th.Join();
                        restoration = new Action<IBasicData, bool>(ShowHideCommandBar);
                    }
                    catch { }
                    ShowHighLightItem(guidItem, guidParent,restoration, basicData, visible);
                    return;
                }
                if (basicData.GetType() == typeof(DockerData))
                {
                    bool visible = corelApp.FrameWork.IsDockerVisible(guidParent);
                    corelApp.FrameWork.ShowDocker(guidParent);
                    restoration = new Action<IBasicData, bool>((o,v)=> 
                    {
                        if (!v)
                            corelApp.FrameWork.HideDocker(o.Guid);
                        });
                    ShowHighLightItem(guidItem, guidParent, restoration, basicData, visible);
                    return;
                }
                if (basicData.GetType() == typeof(DialogData))
                {
#if !X7
                    corelApp.FrameWork.ShowDialog(guidParent);

#endif
                }
                ShowHighLightItem(guidItem, guidParent);
            }

            catch (System.Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Console);
            }
        }
        //private void SetCommmandBarVisible(string guid,object visible)
        //{

        //}
        OverlayForm form;
        public void ShowHighLightItem(string itemGuid, string itemParentGuid, Action<IBasicData, bool> restoration = null, IBasicData restorationData = null, bool v = false, bool firstTime = true)
        {
            WinAPI.SetFocus(ControlUI.corelHandle);
            WinAPI.SetForegroundWindow(ControlUI.corelHandle);

            Corel.Interop.VGCore.cdrWindowState state = corelApp.AppWindow.WindowState;

            
            if (state == Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized || state == Corel.Interop.VGCore.cdrWindowState.cdrWindowMinimized)
                corelApp.AppWindow.WindowState = Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized;
            if (state == Corel.Interop.VGCore.cdrWindowState.cdrWindowRestore)
                corelApp.AppWindow.WindowState = Corel.Interop.VGCore.cdrWindowState.cdrWindowRestore;
            if (state == Corel.Interop.VGCore.cdrWindowState.cdrWindowNormal)
                corelApp.AppWindow.WindowState = Corel.Interop.VGCore.cdrWindowState.cdrWindowNormal;
            //corelApp.AppWindow.Activate();
            
            System.Windows.Rect rect = this.GetItemRect(itemParentGuid, itemGuid);
            
            if (rect.IsZero())
            {
                //if (firstTime)
                //    ShowHighLightItem(itemGuid, itemParentGuid, restoration, restorationData, v, false);
                //else
                //{
                    //core.DispactchNewMessage("Don't can find the control, maybe is does not visible", MsgType.Console);
                    return;
                //}
            }

            core.DispactchNewMessage(DateTime.Now.ToLongTimeString(), MsgType.Console);

            core.SetUIVisible = false;
            //tagWindow.Visibility = win.Visibility.Collapsed;
           
            if (corelApp.AppWindow.WindowState == Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized)
                form = new OverlayForm(rect);
            else
            {
                System.Windows.Rect rect2 = new System.Windows.Rect(corelApp.AppWindow.Left, corelApp.AppWindow.Top, corelApp.AppWindow.Width, corelApp.AppWindow.Height);
                form = new OverlayForm(rect, rect2);
            }
            //Thread.Sleep(1000);
            CallForm();
        }
        private void CallForm( Action<IBasicData, bool> restoration = null, IBasicData restorationData = null, bool v = false)
        {
            if (form.InvokeRequired)
            {
                var d = new Action<Action<IBasicData,bool>, IBasicData,bool>(CallForm);
                form.Invoke(restoration,restorationData,v);
            }
            else
            {
                form.Show();
                form.FormClosed += (s, e) =>
                {
                    core.SetUIVisible = true;
                    if (restoration != null)
                        restoration.Invoke(restorationData, v);
                };
            }
            
           
        }
        //public void showHighLightItem(win.DependencyObject dependencyObject, List<IBasicData> temp)
        //{
        //    Views.XMLTagWindow w = Core.FindParentControl<Views.XMLTagWindow>(dependencyObject) as Views.XMLTagWindow;
        //    if (w == null)
        //    {
        //        core.DispactchNewMessage("Error tagWindow not found", MsgType.Console);
        //        return;
        //    }

        //    showHighLightItem(w, temp);
        //}
    }
}
