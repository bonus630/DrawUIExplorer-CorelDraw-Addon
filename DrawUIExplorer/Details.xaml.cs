using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using br.corp.bonus630.DrawUIExplorer.DataClass;


namespace br.corp.bonus630.DrawUIExplorer
{
    /// <summary>
    /// Interaction logic for Details.xaml
    /// </summary>
    public partial class Details : UserControl
    {

        private IBasicData basicData;
        List<IBasicData> temp;
        CorelAutomation corelAutomation;
        string caption = "";
        private Corel.Interop.VGCore.Application corelApp;


        Core core;
        public Details(Core core)
        {
            InitializeComponent();

            this.DataContext = core;
            corelAutomation = new CorelAutomation(core.CorelApp, core);
            this.core = core;
            corelApp = core.CorelApp;
            core.RunBindCommand = new ViewModel.Commands.AttributeCommand(attributeContentExec, attributeContentCanExec);
            core.RunMacroCommand = new ViewModel.Commands.AttributeCommand(attributeMacro, attributeCanMacro);
            core.SearchGuidCommand = new ViewModel.Commands.AttributeCommand(attributeSerchGuid, attributeCanSerchGuid);
            core.CopyCommand = new ViewModel.Commands.AttributeCommand(attributeCopy, attributeCanCopy);
        }
        private void attributeContentExec(Attribute attribute)
        {
            corelAutomation.RunBindDataSource(attribute.Value);
        }
        private void attributeMacro(Attribute attribute)
        {
            corelAutomation.RunMacro(attribute.Value);
        }
        private void attributeSerchGuid(Attribute attribute)
        {
            core.FindByGuid(core.ListPrimaryItens.Childrens, attribute.Value);
        }
        private void attributeCopy(Attribute attribute)
        {
            Clipboard.SetText(attribute.ToString());
        }
        private bool attributeContentCanExec(Attribute attribute)
        {
            if (attribute.Value.Contains("*Bind"))
                return true;
            return false;
        }
        private bool attributeCanMacro(Attribute attribute)
        {
            return true;
        }
        private bool attributeCanSerchGuid(Attribute attribute)
        {
            return attribute.IsGuid;
        }
        private bool attributeCanCopy(Attribute attribute)
        {
            return true;
        }
        public void Update(IBasicData basicData)
        {
            this.basicData = basicData;
            lba_guid.Content = basicData.Guid;
            lba_guidref.Content = basicData.GuidRef;
            lba_treeLevel.Content = basicData.TreeLevel;
            list_attributes.ItemsSource = basicData.Attributes;
            caption = "";
            TryGetAnyCaption();
            lba_caption.Content = caption;
            lba_index.Content = basicData.XmlChildreID;
            lba_indexRef.Content = basicData.XmlChildreParentID;

            string route = "";

            temp = new List<IBasicData>();
            temp.Add(this.basicData);
            GetRoute(this.basicData);

            temp.Reverse();
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].XmlChildreID > 0 && !temp[i].IAmUniqueTag())
                {
                    if (!string.IsNullOrEmpty(temp[i].Guid))
                        route += string.Format("/{0}[@guid='{1}']", temp[i].TagName, temp[i].Guid);
                    else
                        route += string.Format("/{0}[{1}]", temp[i].TagName, temp[i].XmlChildreID);
                }
                else
                    route += string.Format("/{0}", temp[i].TagName);
            }
            lba_route.Content = route;
        }
        private void GetRoute(IBasicData basic)
        {
            if (basic.Parent != null)
            {
                temp.Add(basic.Parent);
                GetRoute(basic.Parent);
            }
        }
        public string GetCaptionDocker(string guid)
        {
            corelApp.FrameWork.ShowDocker(guid);
            string caption = corelApp.FrameWork.Automation.GetCaptionText(guid);
            corelApp.FrameWork.HideDocker(guid);
            this.Focus();
            return caption;
        }
        //private void btn_showTreeView_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        showHighLightItem();
        //    }
        //    catch (System.Exception erro)
        //    {
        //        core.DispactchNewMessage(erro.Message,MsgType.Console);
        //    }

        //}
        private void showHighLight()
        {

        }
        //private void showHighLightItem()
        //{
        //    try
        //    {
        //        IBasicData itemData = this.basicData;
        //        IBasicData parentItemData = parentItemData = basicData.Parent;
        //        IBasicData specialData = null;

        //        if (basicData.IsSpecialType)
        //        {
        //            specialData = basicData;
        //        }
        //        else
        //        {
        //            while ((specialData.GetType() != typeof(CommandBarData) || specialData.GetType() == typeof(DockerData)
        //           || specialData.GetType() == typeof(DialogData)) && specialData.Parent != null)
        //            {
        //                specialData = specialData.Parent;
        //            }
        //        }
        //        //string guidItem = temp[temp.Count - 1].Guid;
        //        //DataClass.IBasicData basicData = core.SearchItemContainsGuidRef(core.ListPrimaryItens, guidItem);
        //        while (string.IsNullOrEmpty(parentItemData.Guid) && parentItemData.Parent != null)
        //        {
        //            parentItemData = parentItemData.Parent;
        //        }
        //        if (specialData.GetType() == typeof(CommandBarData))
        //        {
        //            corelApp.FrameWork.Automation.ShowBar(specialData.Guid, true);
        //            specialData = new CommandBarData();
        //            specialData.Guid = specialData.Guid;
        //        }
        //        if (specialData.GetType() == typeof(DockerData))
        //        {
        //            if (!corelApp.FrameWork.IsDockerVisible(specialData.Guid))
        //            {
        //                corelApp.FrameWork.ShowDocker(specialData.Guid);
        //                specialData = new DockerData();
        //                specialData.Guid = specialData.Guid;
        //            }
        //        }
        //        if (specialData.GetType() == typeof(DialogData))
        //        {

        //            corelApp.FrameWork.ShowDialog(specialData.Guid)
        //            specialData = new DialogData();
        //            specialData.Guid = specialData.Guid;
        //        }
        //        LoadHighLightForm(basicData, parentItemData, specialData);
        //    }
        //    catch (System.Exception erro)
        //    {
        //        core.DispactchNewMessage(erro.Message);
        //    }
        //}
        private void LoadHighLightForm(IBasicData itemData, IBasicData parentItemData, IBasicData specialData)
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

            System.Windows.Rect rect = corelAutomation.GetItemRect(parentItemData.Guid, itemData.Guid);


            if (rect.IsZero())
                return;
            XMLTagWindow w = Core.FindParentControl<XMLTagWindow>(this.Parent) as XMLTagWindow;
            w.Visibility = Visibility.Collapsed;
            OverlayForm form;
            if (corelApp.AppWindow.WindowState == Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized)
                form = new OverlayForm(rect);
            else
            {
                System.Windows.Rect rect2 = new Rect(corelApp.AppWindow.Left, corelApp.AppWindow.Top, corelApp.AppWindow.Width, corelApp.AppWindow.Height);
                form = new OverlayForm(rect, rect2);
            }

            form.Show();
            form.FormClosed += (s, e) =>
            {
                w.Visibility = Visibility.Visible;
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

        public void showHighLightItem()
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
                System.Windows.Rect rect = corelAutomation.GetItemRect(guidParent, guidItem);
                if (rect.IsZero())
                    return;
                XMLTagWindow w = Core.FindParentControl<XMLTagWindow>(this.Parent) as XMLTagWindow;
                w.Visibility = Visibility.Collapsed;
                OverlayForm form;
                if (corelApp.AppWindow.WindowState == Corel.Interop.VGCore.cdrWindowState.cdrWindowMaximized)
                    form = new OverlayForm(rect);
                else
                {
                    System.Windows.Rect rect2 = new Rect(corelApp.AppWindow.Left, corelApp.AppWindow.Top, corelApp.AppWindow.Width, corelApp.AppWindow.Height);
                    form = new OverlayForm(rect, rect2);
                }
                form.Show();
                form.FormClosed += (s, e) => { w.Visibility = Visibility.Visible; };
            }

            catch (System.Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Console);
            }
        }

        private void TryGetAnyCaption()
        {

            if (!string.IsNullOrEmpty(this.basicData.Caption))
            {
                caption = basicData.Caption;
                lba_captionLoc.Content = "Caption";
                if (!string.IsNullOrEmpty(caption))
                    return;
            }
            if (corelApp != null)
            {
                caption = corelAutomation.GetItemCaption(basicData.Guid);
                lba_captionLoc.Content = "Automation GetCaption";
                if (!string.IsNullOrEmpty(caption))
                    return;

            }
            string[] searchs = new string[] { "captionGuid", "guidRef" };



            for (int i = 0; i < searchs.Length; i++)
            {


                if (this.basicData.ContainsAttribute(searchs[i]))
                {
                    string t = "";
                    //caption = core.SearchItemFromGuidRef(core.ListPrimaryItens, basicData.GetAttribute(searchs[i])).Caption;
                    caption = tryGetAnyCaption(core.SearchItemFromGuidRef(core.ListPrimaryItens, basicData.GetAttribute(searchs[i])), out t);
                    lba_captionLoc.Content = searchs[i];

                }
                if (!string.IsNullOrEmpty(caption))
                    return;
            }
            string search = "nonLocalizableName";
            if (this.basicData.ContainsAttribute(search))
            {

                caption = basicData.GetAttribute(search);
                lba_captionLoc.Content = search;

                if (!string.IsNullOrEmpty(caption))
                    return;
            }
        }
        private string tryGetAnyCaption(IBasicData basicData, out string method)
        {
            string caption = "";
            method = "";
            if (!string.IsNullOrEmpty(this.basicData.Caption))
            {
                caption = basicData.Caption;
                lba_captionLoc.Content = "Caption";
                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            if (corelApp != null)
            {
                caption = corelAutomation.GetItemCaption(basicData.Guid);
                method = "Automation GetCaption";
                if (!string.IsNullOrEmpty(caption))
                    return caption;

            }
            string[] searchs = new string[] { "captionGuid", "guidRef" };



            for (int i = 0; i < searchs.Length; i++)
            {


                if (this.basicData.ContainsAttribute(searchs[i]))
                {
                    string guid = basicData.GetAttribute(searchs[i]);
                    if (!string.IsNullOrEmpty(guid))
                    {
                        caption = tryGetAnyCaption(core.SearchItemFromGuidRef(core.ListPrimaryItens, guid), out method);
                        method = searchs[i];
                    }

                }
                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            string search = "nonLocalizableName";
            if (this.basicData.ContainsAttribute(search))
            {

                caption = basicData.GetAttribute(search);
                method = search;

                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            return caption;
        }


        private void list_attributes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            DataClass.Attribute li = (sender as ListView).SelectedItem as DataClass.Attribute;
            if (li != null)
                Clipboard.SetText(li.ToString());
        }

        //private void btn_getActiveGuid_Click(object sender, RoutedEventArgs e)
        //{
        //    copyItemCaptionAndGuid();
        //}
        public void copyItemCaptionAndGuid()
        {
            
            Clipboard.SetText(string.Format("{0} - {1}", this.caption, this.basicData.Guid));
        }
        private void selectItem()
        {
            for (int i = 0; i < temp.Count; i++)
            {
                if (i != temp.Count - 1)
                    temp[i].SetSelected(false, true, true);
                else
                    temp[i].SetSelected(true, false, true);
            }
        }

        private void lba_route_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string content = (sender as Label).Content.ToString();
            Clipboard.SetText(content);
        }
    }

}
