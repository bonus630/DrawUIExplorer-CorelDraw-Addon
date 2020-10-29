﻿using br.corp.bonus630.DrawUIExplorer.DataClass;
using br.corp.bonus630.DrawUIExplorer.Models;
using br.corp.bonus630.DrawUIExplorer.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace br.corp.bonus630.DrawUIExplorer.Views
{

    public partial class XMLTagWindow : System.Windows.Window
    {


        Core core;
        Details details;
        Search search;
        XSLTEster xslTester;
        CorelAutomation corelCmd;
        int msgCount = 1;
        //bool cancelTreeGeneration = false;
        Thread th = null;
        XMLTagWindowViewModel dataContext;
        SaveLoadConfig saveLoad;
        //public static bool inCorel = true;
        private Corel.Interop.VGCore.Application app;

        //public XMLTagWindow(Corel.Interop.VGCore.Application app, string filePath)
        //{
        //    InitializeComponent();
        //    this.app = app;
        //    core = new Core();

        //}
        public XMLTagWindow(object app, string filePath)
        {
            InitializeComponent();
            this.app = app as Corel.Interop.VGCore.Application;
            core = new Core();

        }
        public XMLTagWindow(string filePath)
        {
            InitializeComponent();
            core = new Core();
          
        }
        public void StartProcess(string filePath)
        {
            //treeView_Nodes.Items.Clear();
            //treeView_Search.Items.Clear();
            msgCount = 1;
            dataContext = new XMLTagWindowViewModel(core);
            this.DataContext = dataContext;
            core.StartCore(filePath, this.app);
            dataContext.Title = filePath;
            corelCmd = new CorelAutomation(this.app, core);
            dataContext.CorelCmd = corelCmd;
            core.LoadXmlFinish += Core_LoadFinish;
            core.FilePorcentLoad += Core_FilePorcentLoad;
            core.LoadListsFinish += Core_LoadListsFinish;
            core.LoadStarting += Core_LoadStarting;
            core.ErrorFound += Core_ErrorFound;
            core.SearchResultEvent += Core_SearchResultEvent;
            core.LoadFinish += Core_LoadFinish1;
            core.NewMessage += Core_NewMessage;
            core.RequestUIHideVisibleChanged += Core_RequestUIHideVisibleChanged;
            dataContext.InCorel = core.InCorel;
            //treeView_Nodes.GotFocus += (s, e) => { if (treeView_Nodes.SelectedItem != null) UpdateDetails(treeView_Nodes.SelectedItem, e); };
            //treeView_Ref.GotFocus += (s, e) => { if (treeView_Ref.SelectedItem != null) UpdateDetails(treeView_Ref.SelectedItem, e); };
            //treeView_Search.GotFocus += (s, e) => { if (treeView_Search.SelectedItem != null) UpdateDetails(treeView_Search.SelectedItem, e); };
            treeView_Nodes.SelectedItemChanged += (s, e) => { if (treeView_Nodes.SelectedItem != null) UpdateDetails(treeView_Nodes.SelectedItem, e); };
            treeView_Ref.SelectedItemChanged += (s, e) => { if (treeView_Ref.SelectedItem != null) UpdateDetails(treeView_Ref.SelectedItem, e); };
            treeView_Search.SelectedItemChanged += (s, e) => { if (treeView_Search.SelectedItem != null) UpdateDetails(treeView_Search.SelectedItem, e); };
            dataContext.XmlDecode += DataContext_XmlDecode;
            inputControl.Core = core;
            saveLoad = new SaveLoadConfig();
        }

        private void DataContext_XmlDecode(IBasicData obj)
        {
            this.Dispatcher.Invoke(new Action(()=>
            {
                XmlEncoder xmlEncoder = new XmlEncoder();
                string startString = txt_xmlViewer.Text.Substring(0, txt_xmlViewer.CaretIndex);
                string finalString = txt_xmlViewer.Text.Substring(txt_xmlViewer.CaretIndex);
                txt_xmlViewer.Text = string.Format("{0}{1}{2}\n\r", startString, xmlEncoder.xmlEncode1(obj), finalString);
                //txt_xmlViewer.AppendText(XmlEncode.xmlEncode((sender as MenuItemData).Data)+"\r\n");
            }));
        }

        public void MergeProcess(string filePath)
        {
            core.MergeProcess(filePath);
        }
        private void Core_RequestUIHideVisibleChanged(bool obj)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (obj)
                    this.Visibility = Visibility.Visible;
                else
                    this.Visibility = Visibility.Hidden;
            }
            ));
       
        }

        private void Core_LoadFinish1(string msg)
        {
            SetProgressBar(false, false, msg);
        }

        private void Core_SearchResultEvent(IBasicData obj)
        {
            //Vou testar para salvar as pesquisar
           // treeView_Search.Items.Clear();
            gridRef.Visibility = Visibility.Visible;
            treeView_Search.ItemsSource = dataContext.SearchList;
            //InflateTreeView(obj, treeView_Search);
        }

        private void Core_ErrorFound(string obj)
        {
            SetProgressBar(false, false, obj);
        }

        private void Core_LoadStarting(bool obj, string msg)
        {
            SetProgressBar(obj, true, msg);
        }
        private void Core_NewMessage(string message, MsgType msgType)
        {
            SetProgressBar(false, false, message, msgType);
        }
        private void Core_LoadListsFinish()
        {
            SetProgressBar(false, false);
             this.Dispatcher.Invoke(new Action(() =>
            {
                //InflateTreeView(core.ListPrimaryItens, treeView_Nodes);
                tabControl_details.Visibility = Visibility.Visible;
                search = new Search(core);
                grid_search.Children.Add(search);
                details = new Details(core);
                grid_details.Children.Add(details);
                xslTester = new XSLTEster(core);
                grid_xslTester.Children.Add(xslTester);
                core.ListPrimaryItens.SetSelected(true, true, true);
                dockPanel_treeViews.Visibility = Visibility.Visible;
                treeView_Nodes.ItemsSource = dataContext.MainList;
            }));

        }

        private void Core_FilePorcentLoad(int obj)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                pb_load.Value = obj;
                //Debug.WriteLine(obj);
            }
                ));
        }

        private void Core_LoadFinish(string obj)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                SetProgressBar(false, false, "Finish decode...");
            }
            ));
        }

        private void SetProgressBar(bool isIndeterminate, bool visible, string msg = "", MsgType msgType = MsgType.Console)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (visible)
                    pb_load.Visibility = Visibility.Visible;
                else
                    pb_load.Visibility = Visibility.Collapsed;
                pb_load.IsIndeterminate = isIndeterminate;

                if (!string.IsNullOrEmpty(msg))
                {
                    switch (msgType)
                    {
                        case MsgType.Console:
                            string s = "";
                            if(saveLoad.ConsoleCounter)
                                s = (string.Format("{0}. {1}\r\n", msgCount, msg));
                            else
                               s = (string.Format("{0}\r\n", msg));
                            txt_console.Text = s;
                            txt_consoleFull.AppendText(s);
                            try
                            {
                                txt_consoleFull.ScrollToLine(txt_consoleFull.LineCount - 1);
                            }
                            catch { }
                            msgCount++;
                            break;
                        case MsgType.Event:
                            txt_CorelEventViewer.AppendText(string.Format("{0}\r\n", msg));
                            try
                            {
                                txt_CorelEventViewer.ScrollToLine(txt_CorelEventViewer.LineCount - 1);
                            }
                            catch { }
                            break;
                    }
                }

            }
          ));
        }



     

   

        private void UpdateDetails(object sender, RoutedEventArgs args)
        {
            // IBasicData data = (sender as TreeViewItemData).Data;
            IBasicData data = (sender as IBasicData);
            //var parent = Core.FindParentControl<TreeView>(sender as TreeViewItemData);
            string parent = (args.Source as TreeView).Name;
            if (core.CurrentBasicData != null)
                core.CurrentBasicData.SetSelected(false, false, false,true);
            core.CurrentBasicData = data;
           
            data.SetSelected(true, true, false,true);
            if (parent != null && (parent == "treeView_Nodes" || parent == "treeView_Search"))
            {
                //treeView_Ref.Items.Clear();
                dataContext.RefList.Clear();
                if (!string.IsNullOrEmpty(data.GuidRef))
                {

                    IBasicData refBasicData = core.SearchItemFromGuidRef(core.ListPrimaryItens, data.GuidRef);
                    // InflateTreeView(refBasicData, treeView_Ref);
                    //TreeViewItemData treeViewItem = GenerateTreeViewItem(refBasicData);
                    //GenerateTreeViewItemList(treeViewItem);
                    // treeView_Ref.Items.Add(treeViewItem);
                    dataContext.RefList.Add(refBasicData);
                    treeView_Ref.ItemsSource = dataContext.RefList;
                }
                if (treeView_Ref.Items.Count == 0 && treeView_Search.Items.Count == 0)
                    gridRef.Visibility = Visibility.Collapsed;
                else
                    gridRef.Visibility = Visibility.Visible;
            }
            //details.Update(data);
            //search.Update(data);
            //xslTester.Update(data);
            //lba_tagName.Content = data.TagName;
            args.Handled = true;
        }
        private void GenerateContextMenu(TreeViewItemData treeViewItemData)
        {
            //ContextMenu contextMenu = new ContextMenu();
            //contextMenu.AddHandler(ContextMenu.ContextMenuOpeningEvent, new ContextMenuEventHandler(ContextMenu_ContextMenuOpening));

            // itens que não precisão do app 
            //if (!string.IsNullOrEmpty(treeViewItemData.Data.GuidRef))
            //{
            //    MenuItemData menuItem = new MenuItemData();
            //    menuItem.Data = treeViewItemData.Data;
            //    menuItem.Header = "Find Ref Item";
            //    menuItem.Click += MenuFindRef_Click;
            //    contextMenu.Items.Add(menuItem);


            //}
            //if (!string.IsNullOrEmpty(treeViewItemData.Data.Guid))
            //{
            //    MenuItemData menuItem = new MenuItemData();
            //    menuItem.Data = treeViewItemData.Data;
            //    menuItem.Header = "Copy Guid";
            //    menuItem.Icon = new System.Windows.Controls.Image() { Source = dataContext.CopyMenuItemImg };
            //    menuItem.Click += MenuCopyGuid_Click;
            //    contextMenu.Items.Add(menuItem);


            //}
            if (treeViewItemData.Data.ContainsAttribute("captionRef"))
            {
                MenuItemData menuItem = new MenuItemData();
                menuItem.Data = treeViewItemData.Data;
                menuItem.Header = "Find Caption Ref";
                menuItem.Click += MenuItemFindCaptionRef_Click;
                //contextMenu.Items.Add(menuItem);
            }
            //if (treeViewItemData.Data.ContainsAttribute("itemRef"))
            //{
            //    MenuItemData menuItem = new MenuItemData();
            //    menuItem.Data = treeViewItemData.Data;
            //    menuItem.Header = "Find Item Ref";
            //    menuItem.Click += MenuItemFindCaptionRef_Click;
            //    contextMenu.Items.Add(menuItem);
            //}
            for (int i = 0; i < treeViewItemData.Data.Attributes.Count; i++)
            {
                DataClass.Attribute att = treeViewItemData.Data.Attributes[i];
                if (att.Name != "captionRef" && att.Name != "guid" && att.Name != "guidRef" && att.IsGuid)
                {
                    MenuItemData menuItem = new MenuItemData();
                    menuItem.Data = treeViewItemData.Data;
                    menuItem.Header = string.Format("Find {0}", att.Name);
                    menuItem.Tag = att.Name;
                    menuItem.Click += MenuItemFindGenericRef_Click;
                    //contextMenu.Items.Add(menuItem);
                }
            }


            //MenuItemData menuItem4 = new MenuItemData();
            //menuItem4.Data = treeViewItemData.Data;
            //menuItem4.Header = "Xml";
            //menuItem4.Click += MenuItemXmlEncode_Click;
            //contextMenu.Items.Add(menuItem4);

            //if (!dataContext.InCorel)
            //    return contextMenu;



            //MenuItemData menuItem2 = new MenuItemData();
            //menuItem2.Data = treeViewItemData.Data;
            //menuItem2.Header = "Try Get Caption Text";
            //menuItem2.Click += MenuItemGetCaptionText_Click;
            //contextMenu.Items.Add(menuItem2);

            //MenuItemData menuItem6 = new MenuItemData();
            //menuItem6.Data = treeViewItemData.Data;
            //menuItem6.Header = "Try highlight this";
            //menuItem6.Click += MenuItemTryHighlight_Click;
            //menuItem6.Icon = new System.Windows.Controls.Image() { Source = dataContext.HighLightButtonImg };
            //contextMenu.Items.Add(menuItem6);

            //if (treeViewItemData.Data.GetType() == typeof(DataClass.CommandBarData))
            //{
            //    MenuItemData menuItem = new MenuItemData();
            //    menuItem.Data = treeViewItemData.Data;
            //    menuItem.Header = "Try Show Command Bar";
            //    menuItem.Click += MenuItemshowCommandBar_Click;
            //    contextMenu.Items.Add(menuItem);

            //    MenuItemData menuItem3 = new MenuItemData();
            //    menuItem3.Data = treeViewItemData.Data;
            //    menuItem3.Header = "Try Hide Command Bar";
            //    menuItem3.Click += MenuItemHideCommandBar_Click;
            //    contextMenu.Items.Add(menuItem3);

            //    MenuItemData menuItem5 = new MenuItemData();
            //    menuItem5.Data = treeViewItemData.Data;
            //    menuItem5.Header = "Command Bar Mode";
            //    menuItem5.Click += MenuItemCommandBarMode_Click;
            //    contextMenu.Items.Add(menuItem5);

            //}
            //if (treeViewItemData.Data.GetType() == typeof(DataClass.OtherData))
            //{
            //    MenuItemData menuItem = new MenuItemData();
            //    menuItem.Data = treeViewItemData.Data;
            //    menuItem.Header = "Try Show this";
            //    menuItem.Click += MenuItemshowItem_Click;
            //    contextMenu.Items.Add(menuItem);
            //}
            //if (treeViewItemData.Data.GetType() == typeof(DataClass.DialogData))
            //{
            //    MenuItemData menuItem = new MenuItemData();
            //    menuItem.Data = treeViewItemData.Data;
            //    menuItem.Header = "Try Show Dialog";
            //    menuItem.Click += MenuItemshowDialog_Click;
            //    contextMenu.Items.Add(menuItem);

            //    MenuItemData menuItem3 = new MenuItemData();
            //    menuItem3.Data = treeViewItemData.Data;
            //    menuItem3.Header = "Try Hide Dialog";
            //    menuItem3.Click += MenuItemHideDialog_Click;
            //    contextMenu.Items.Add(menuItem3);
            //}
            //if (treeViewItemData.Data.GetType() == typeof(DataClass.DockerData))
            //{
            //    MenuItemData menuItem = new MenuItemData();
            //    menuItem.Data = treeViewItemData.Data;
            //    menuItem.Header = "Try Show Docker";
            //    menuItem.Click += MenuItemshowDocker_Click;
            //    contextMenu.Items.Add(menuItem);

            //    MenuItemData menuItem3 = new MenuItemData();
            //    menuItem3.Data = treeViewItemData.Data;
            //    menuItem3.Header = "Try Hide Docker";
            //    menuItem3.Click += MenuItemHideDocker_Click;
            //    contextMenu.Items.Add(menuItem3);
            //}


            //if (treeViewItemData.Data.GetType() == typeof(DataClass.ItemData))
            //{
            //    MenuItemData menuItem = new MenuItemData();
            //    menuItem.Data = treeViewItemData.Data;
            //    menuItem.Header = "Try Invoke Item";
            //    menuItem.Click += MenuItemInvoke_Click;
            //    contextMenu.Items.Add(menuItem);
            //}
            //if (treeViewItemData.Data.TagName == "dockers")
            //{
            //    MenuItemData menuItem = new MenuItemData();
            //    menuItem.Data = treeViewItemData.Data;
            //    menuItem.Header = "Get Dockers Caption";
            //    menuItem.Click += MenuItemGetDockerCaption_Click;
            //    contextMenu.Items.Add(menuItem);

            //    //MenuItemData menuItem12 = new MenuItemData();
            //    //menuItem12.Data = treeViewItemData.Data;
            //    //menuItem12.Header = "Get Dockers Guids";
            //    //menuItem12.Click += MenuItemGetDockersGuid_Click;
            //    //contextMenu.Items.Add(menuItem12);
            //}
        


            //return contextMenu;
        }

    

    
        #region ContextMenu Items Click Events
        private void MenuItemFindGenericRef_Click(object sender, RoutedEventArgs e)
        {
            string guid = "";
            MenuItemData MenuData = (sender as MenuItemData);
            IBasicData basicData = MenuData.Data;
            for (int i = 0; i < basicData.Attributes.Count; i++)
            {
                if ((string)MenuData.Tag == basicData.Attributes[i].Name)
                {
                    guid = basicData.Attributes[i].Value;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(guid))
            {
                //core.SearchItemFromGuidRef(core.ListPrimaryItens, guid);
                core.FindByGuid(core.ListPrimaryItens.Childrens, guid);
            }
        }
        private void MenuItemFindCaptionRef_Click(object sender, RoutedEventArgs e)
        {
            IBasicData basicData = (sender as MenuItemData).Data;
            FindAnyRef("captionRef", basicData);
        }
     
        private void FindAnyRef(string genericRef, IBasicData basicData)
        {
            string guid = "";

            for (int i = 0; i < basicData.Attributes.Count; i++)
            {
                if (basicData.Attributes[i].Name == genericRef)
                    guid = basicData.Attributes[i].Value;
            }
            if (!string.IsNullOrEmpty(guid))
                core.FindByGuid(core.ListPrimaryItens.Childrens, guid);
        }

    
        //private void MenuItemXmlEncode_Click(object sender, RoutedEventArgs e)
        //{
        //    XmlEncoder xmlEncoder = new XmlEncoder();
        //    string startString = txt_xmlViewer.Text.Substring(0, txt_xmlViewer.CaretIndex);
        //    string finalString = txt_xmlViewer.Text.Substring(txt_xmlViewer.CaretIndex);
        //    txt_xmlViewer.Text = string.Format("{0}{1}{2}\n\r", startString, xmlEncoder.xmlEncode1((sender as MenuItemData).Data), finalString);
        //    //txt_xmlViewer.AppendText(XmlEncode.xmlEncode((sender as MenuItemData).Data)+"\r\n");
        //}
 

      
     
      
        #endregion
        private void txt_inputCommand_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Debug.WriteLine(textBox.CaretIndex);
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                string command = "";
                command = textBox.GetLineText(textBox.GetLineIndexFromCharacterIndex(textBox.CaretIndex));
                command.Trim(" ".ToCharArray());
                Debug.WriteLine(command);
                textBox.AppendText(Environment.NewLine);
                //textBox.GetLineText()
                textBox.AppendText(core.RunCommand(command));
                textBox.AppendText(Environment.NewLine);

                textBox.CaretIndex = textBox.Text.Length - 1;

            }
            e.Handled = false;


        }

        private void btn_clearConsole_Click(object sender, RoutedEventArgs e)
        {
            int index = tabControl_Details.SelectedIndex;
            switch (index)
            {
                case 0:
                    inputControl.Text = "";
                    break;
                case 1:
                    txt_xmlViewer.Text = "";
                    break;
                case 2:
                    txt_consoleFull.Text = "";
                    break;
                case 3:
                    txt_CorelEventViewer.Text = "";
                    break;
            }
        }
        private void btn_showTreeView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                core.CorelAutomation.ShowHighLightItem( core.Route);
            }
            catch (System.Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Console);
            }

        }
        private void tabControl_Details_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!dataContext.ConsoleExpanded)
                dataContext.ConsoleExpanded = true;
        }

        
    }
}
