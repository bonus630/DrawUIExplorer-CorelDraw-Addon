using br.corp.bonus630.DrawUIExplorer.DataClass;
using br.corp.bonus630.DrawUIExplorer.Models;
using br.corp.bonus630.DrawUIExplorer.ViewModels.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace br.corp.bonus630.DrawUIExplorer.ViewModels
{
    class XMLTagWindowViewModel : ViewModelDataBase
    {
        private bool incorel = false;
        public CorelAutomation CorelCmd { get; set; }
        public event Action<IBasicData> XmlDecode;

        public XMLTagWindowViewModel(Core core) : base(core)
        {
            autoCompleteInputCommand();
            this.mainList = new BlockingCollection<IBasicData>();
            this.refList = new ObservableCollection<IBasicData>();
            this.searchList = new ObservableCollection<IBasicData>();
            core.LoadListsFinish += Core_LoadListsFinish;
            core.SearchResultEvent += Core_SearchResultEvent;
            initializeCommands();
        }

        private void Core_SearchResultEvent(IBasicData obj)
        {
            searchList.Add(obj);
            NotifyPropertyChanged("SearchList");
        }
        private void Core_LoadListsFinish()
        {
            mainList.Add(core.ListPrimaryItens);
            NotifyPropertyChanged("MainList");
        }
        public bool InCorel
        {
            get { return incorel; }
            set
            {
                incorel = value;
                NotifyPropertyChanged();
            }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }
        private bool consoleExpanded;

        public bool ConsoleExpanded
        {
            get { return consoleExpanded; }
            set
            {
                consoleExpanded = value;
                NotifyPropertyChanged();
            }
        }

        // public BitmapSource HighLightButtonImg { get { return Properties.Resources.light.GetBitmapSource(); } }
        System.Windows.Forms.AutoCompleteStringCollection AutoCompleteSource { get; set; }

        //public SimpleCommand configCommand, clearConsoleCommmand, expandConsoleCommand, highLightCommand, activeGuidCommand;

        public SimpleCommand ConfigCommand { get { return new SimpleCommand(config); } }
        public SimpleCommand ExpandConsoleCommand { get { return new SimpleCommand(expandConsole); } }
        public SimpleCommand ActiveGuidCommand { get { return new SimpleCommand(activeGuid); } }
        public SimpleCommand HighLightCommand { get { return new SimpleCommand(showHighLightItem); } }


        public BaseDataCommand FindRefItemCommand { get; protected set; }
        public BaseDataCommand CopyGuidCommand { get; protected set; }
        public BaseDataCommand FindCaptionRefCommand { get; protected set; }
        public BaseDataCommand FindGuidCommand { get; protected set; }
        public BaseDataCommand FindGuidRefCommand { get; protected set; }
        public BaseDataCommand XmlCommand { get; protected set; }
        public BaseDataCommand GetCaptionCommand { get; protected set; }
        public BaseDataCommand ShowCommandBarCommand { get; protected set; }
        public BaseDataCommand HideCommandBarCommand { get; protected set; }
        public BaseDataCommand CommandBarModeCommand { get; protected set; }
        public BaseDataCommand ShowThisCommand { get; protected set; }
        public BaseDataCommand HideThisCommand { get; protected set; }
        public BaseDataCommand ShowDialogCommand { get; protected set; }
        public BaseDataCommand HideDialogCommand { get; protected set; }
        public BaseDataCommand ShowDockerCommand { get; protected set; }
        public BaseDataCommand HideDockerCommand { get; protected set; }
        public BaseDataCommand InvokeItemCommand { get; protected set; }
        public BaseDataCommand GetDockersCaptionCommand { get; protected set; }
        public BaseDataCommand GetDockersGuidCommand { get; protected set; }
        public BaseDataCommand RemoveMeCommand { get; protected set; }

        private bool IsDockers(IBasicData basicData)
        {
            if (basicData.TagName == "dockers")
                return true;
            return false;
        }
        private bool IsTrue(IBasicData basicData)
        {
            return true;
        }
        private bool IsItemData(IBasicData basicData)
        {
            if (!incorel)
                return false;
            return IsTypeOf(basicData, typeof(ItemData));
        }
        private bool IsDockerData(IBasicData basicData)
        {
            return IsTypeOf(basicData, typeof(DockerData));
        }
        private bool IsDialogData(IBasicData basicData)
        {
            return IsTypeOf(basicData, typeof(DialogData));
        }
        private bool IsSearchData(IBasicData basicData)
        {
            return IsTypeOf(basicData, typeof(SearchData));
        }
        private bool IsOtherData(IBasicData basicData)
        {
            return IsTypeOf(basicData, typeof(OtherData));
        }
        private bool IsCommandBarData(IBasicData basicData)
        {
            return IsTypeOf(basicData, typeof(CommandBarData));
        }
        private bool IsTypeOf(IBasicData basicData, Type type)
        {
            if (basicData.GetType() == type)
                return true;
            return false;
        }
        private void ShowDialogExec(IBasicData basicData)
        {
            CorelCmd.ShowDialog(basicData.Guid);
        }
        private void HideDialogExec(IBasicData basicData)
        {
#if !X7
            CorelCmd.HideDialog(basicData.Guid);
#endif
        }
        private void ShowDockerExec(IBasicData basicData)
        {
            CorelCmd.ShowDocker(basicData.Guid);
        }
        private void HideDockerExec(IBasicData basicData)
        {
            CorelCmd.HideDocker(basicData.Guid);
        }
        private void ShowCommandBarExec(IBasicData basicData)
        {
            CorelCmd.ShowHideCommandBar(basicData, true);

        }
        private void HideCommandBarExec(IBasicData basicData)
        {
            CorelCmd.ShowHideCommandBar(basicData, false);
        }
        private void CommandBarModeExec(IBasicData basicData)
        {
            CorelCmd.CommandBarMode(basicData, false);
        }
        private void ShowItemExec(IBasicData basicData)
        {
            CorelCmd.ShowBar(basicData.Guid);
        }
        private void GetCaptionTextExec(IBasicData basicData)
        {
            core.DispactchNewMessage(CorelCmd.GetCaption(basicData.Guid), MsgType.Console);
        }
        private void ItemInvokeExec(IBasicData basicData)
        {
            CorelCmd.InvokeItem(basicData);
        }
        private void GetDockersCaptionExec(IBasicData basicData)
        {

            core.DispactchNewMessage("Corel will open all Dockers, please wait...", MsgType.Console);

            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                IBasicData temp = basicData.Childrens[i];
                if (temp.GetType() == typeof(DockerData))
                {
                    if (string.IsNullOrEmpty(temp.Caption))
                    {
                        temp.Caption = core.CorelApp.FrameWork.Automation.GetCaptionText(temp.Guid);
                    }
                    if (string.IsNullOrEmpty(temp.Caption))
                    {
                        try
                        {
                            core.CorelApp.FrameWork.ShowDocker(temp.Guid);
                            temp.Caption = core.CorelApp.FrameWork.Automation.GetCaptionText(temp.Guid);
                            core.CorelApp.FrameWork.HideDocker(temp.Guid);
                        }
                        catch { }
                    }
                }
            }
            core.DispactchNewMessage("All Dockers crawleds", MsgType.Console);


        }
        private void GetDockersGuidExec(IBasicData basicData)
        {
            string guid = "";
            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                IBasicData temp = basicData.Childrens[i];
                if (temp.GetType() == typeof(DockerData))
                {
                    guid += "\"" + temp.Guid + "\",";
                }
            }
            core.DispactchNewMessage("Dockers Guids copied!", MsgType.Console);
            Clipboard.SetText(guid);
        }
        private bool findRefItemCanExec(IBasicData basicData)
        {
            if (!string.IsNullOrEmpty(basicData.GuidRef))
                return true;
            return false;
        }
        private void findRefItemExec(IBasicData basicData)
        {
            basicData.Caption = CorelCmd.GetCaption(basicData.Guid);
            core.FindByGuid(core.ListPrimaryItens.Childrens, basicData.GuidRef);
        }
        private bool HasGuid(IBasicData basicData)
        {
            if (!string.IsNullOrEmpty(basicData.Guid))
                return true;
            return false;
        }
        private void CopyGuidExec(IBasicData basicData)
        {
            System.Windows.Clipboard.SetText(basicData.Guid);
            core.DispactchNewMessage(string.Format("Copied    {0}", basicData.Guid), MsgType.Console);
        }
        private void RemoveMeExec(IBasicData basicData)
        {
            searchList.Remove(basicData);
            NotifyPropertyChanged("SearchList");
        }
        private void XmlExec(IBasicData basicData)
        {
            //Precisa fixar
            if (XmlDecode != null)
                XmlDecode(basicData);
        }
      
        private void initializeCommands()
        {
            FindRefItemCommand = new BaseDataCommand(findRefItemExec, findRefItemCanExec);
            CopyGuidCommand = new BaseDataCommand(CopyGuidExec, HasGuid);

            FindCaptionRefCommand = new BaseDataCommand(CopyGuidExec, HasGuid);
            FindGuidCommand = new BaseDataCommand(CopyGuidExec, HasGuid);
            FindGuidRefCommand = new BaseDataCommand(findRefItemExec, HasGuid);
            XmlCommand = new BaseDataCommand(XmlExec, IsTrue);
            GetCaptionCommand = new BaseDataCommand(GetCaptionTextExec, HasGuid);
            ShowCommandBarCommand = new BaseDataCommand(ShowCommandBarExec, IsCommandBarData);
            HideCommandBarCommand = new BaseDataCommand(HideCommandBarExec, IsCommandBarData);
            CommandBarModeCommand = new BaseDataCommand(CommandBarModeExec, IsCommandBarData);
            ShowThisCommand = new BaseDataCommand(ShowItemExec, IsItemData);
            HideThisCommand = new BaseDataCommand(CopyGuidExec, IsItemData);
            ShowDialogCommand = new BaseDataCommand(ShowDialogExec, IsDialogData);
            HideDialogCommand = new BaseDataCommand(HideDialogExec, IsDialogData);
            ShowDockerCommand = new BaseDataCommand(ShowDockerExec, IsDockerData);
            HideDockerCommand = new BaseDataCommand(HideDockerExec, IsDockerData);
            InvokeItemCommand = new BaseDataCommand(ItemInvokeExec, IsItemData);
            GetDockersCaptionCommand = new BaseDataCommand(GetDockersCaptionExec, IsDockers);
            GetDockersGuidCommand = new BaseDataCommand(GetDockersGuidExec, IsDockers);
            RemoveMeCommand = new BaseDataCommand(RemoveMeExec, IsSearchData);
        }
        private BlockingCollection<IBasicData> mainList;
        public BlockingCollection<IBasicData> MainList
        {
            get { return mainList; }
            protected set
            {
                mainList = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<IBasicData> refList;
        public ObservableCollection<IBasicData> RefList
        {
            get { return refList; }
            protected set
            {
                refList = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<IBasicData> searchList;
        public ObservableCollection<IBasicData> SearchList
        {
            get { return searchList; }
            protected set
            {
                searchList = value;
                NotifyPropertyChanged();
            }
        }
        protected override void Update(IBasicData basicData)
        {
            CurrentBasicData = basicData;
        }
        private void autoCompleteInputCommand()
        {
            AutoCompleteSource = new System.Windows.Forms.AutoCompleteStringCollection();
            MethodInfo[] m = (typeof(InputCommands)).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < m.Length; i++)
            {
                AutoCompleteSource.Add(m[i].Name);
            }

        }


        private void showHighLightItem()
        {
            core.CorelAutomation.ShowHighLightItem(core.Route);
        }
        private void activeGuid()
        {
            //core.CopyItemCaptionAndGuid();
        }

        private void config()
        {
            Views.Config config = new Views.Config();
            config.ShowDialog();
        }
        private void expandConsole()
        {
            this.ConsoleExpanded = !this.ConsoleExpanded;
        }
    }
  
}
