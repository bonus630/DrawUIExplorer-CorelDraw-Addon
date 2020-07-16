using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Reflection;
using br.corp.bonus630.DrawUIExplorer.DataClass;
using System.Windows;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace br.corp.bonus630.DrawUIExplorer
{
    public class Core : INotifyPropertyChanged
    {
        XMLDecoder xmlDecoder;
        WorkspaceUnzip workspaceUnzip;

        public event Action<string> LoadXmlFinish;
        public event Action LoadListsFinish;
        public event Action<bool, string> LoadStarting;
        public event Action<string> LoadFinish;
        public event Action<int> FilePorcentLoad;
        public event Action<string> ErrorFound;
        public event Action<IBasicData> SearchResultEvent;
        public event Action<List<object>> GenericSearchResultEvent;
        public event Action<string, MsgType> NewMessage;
        public event PropertyChangedEventHandler PropertyChanged;

        public IBasicData ListPrimaryItens { get; set; }
        public IBasicData CurrentBasicData { get { return this.currentData; } set { this.currentData = value; NotifyPropertyChanged(); } }
        public SearchEngine SearchEngineGet { get { return this.searchEngine; } }
        public SearchEngine searchEngine;
        private MethodInfo[] commands;
        public MethodInfo[] Commands { get { return commands; } }
        private InputCommands inputCommands;
        private IBasicData currentData;
        private Corel.Interop.VGCore.Application app;

        public Corel.Interop.VGCore.Application CorelApp
        {
            get { return app; }
        }
        public void StartCore(string filePath, Corel.Interop.VGCore.Application corelApp)
        {
            if (corelApp != null)
            {
                InCorel = true;
                this.app = corelApp;
                corelApp.OnApplicationEvent += CorelApp_OnApplicationEvent;
            }
            FileInfo file = null;
            try
            {
                FileInfo fileOri = new FileInfo(filePath);
                Title = filePath;
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\bonus630";
                try
                {
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                }
                catch (IOException ioE)
                {
                    if (ErrorFound != null)
                        ErrorFound("Erro - " + ioE.Message);
                    return;
                }
                string newPath = folder + "\\" + fileOri.Name;
                if (File.Exists(newPath))
                    File.Delete(newPath);
                file = fileOri.CopyTo(newPath);
            }
            catch (IOException ioErro)
            {
                if (ErrorFound != null)
                    ErrorFound("Erro - " + ioErro.Message);
                return;
            }
            inputCommands = new InputCommands(this);
            commands = (typeof(InputCommands)).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            xmlDecoder = new XMLDecoder();
            xmlDecoder.LoadFinish += XmlDecoder_LoadFinish;
            Thread thread = new Thread(new ParameterizedThreadStart(LoadFile));
            thread.IsBackground = true;
            thread.Start(file);
            autoCompleteInputCommand();
            
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
        private void CorelApp_OnApplicationEvent(string EventName, ref object[] Parameters)
        {
            string eventName = EventName;
            for (int i = 0; i < Parameters.Length; i++)
            {
                EventName += " Param" + i + "|Type:" + Parameters[i].GetType() + "Value:" + Parameters[i].ToString();
            }

            DispactchNewMessage(eventName, MsgType.Event);
        }
        public string RunCommand(string commandName)
        {
            try
            {
                string result = "";
                string[] pierces = commandName.Split(" ".ToCharArray());
                int j = 0;
                List<object> param = new List<object>();
                while (string.IsNullOrEmpty(pierces[j]) || pierces[j] == " ")
                {
                    if (!string.IsNullOrEmpty(pierces[j]) && pierces[j] != " ")
                    {
                        commandName = pierces[j];
                        j++;
                        break;
                    }
                    j++;
                    if (j >= pierces.Length)
                        break;
                }

                while (string.IsNullOrEmpty(pierces[j]) || pierces[j] == " ")
                {
                    if (!string.IsNullOrEmpty(pierces[j]) && pierces[j] != " ")
                    {
                        param.Add(pierces[j]);

                    }
                    j++;
                    if (j >= pierces.Length)
                        break;
                }



                for (int i = 0; i < commands.Length; i++)
                {
                    if (commands[i].Name == commandName)
                        result = commands[i].Invoke(inputCommands, param.ToArray()).ToString();
                }
                return result;
            }
            catch(Exception err)
            {
                return err.Message;
            }
            
        }

        private void XmlDecoder_LoadFinish()
        {
            if (this.LoadXmlFinish != null)
                LoadXmlFinish(xmlDecoder.XmlString);
        }

        /// <summary>
        /// Dispara o evento de pesquisa
        /// </summary>
        /// <param name="list"></param>
        /// <param name="guid"></param>
        public void FindByGuid(ObservableCollection<IBasicData> list, string guid)
        {
            searchEngine.NewSearch();
            searchEngine.SearchItemFromGuidRef(list, guid);
        }
        /// <summary>
        /// Não dispara o evento de pesquisa
        /// </summary>
        /// <param name="list"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public IBasicData SearchItemFromGuidRef(IBasicData list, string guid)
        {

            return searchEngine.SearchItemFromGuidRef(list, guid);
        }
        public IBasicData SearchItemContainsGuidRef(IBasicData list, string guid)
        {
            return searchEngine.SearchItemContainsGuidRef(list, guid);
        }
        public void FindAllTags(IBasicData basicData)
        {
            searchEngine.NewSearch();
            searchEngine.SearchAllTags(basicData);
        }

        public string GetXml(IBasicData basicData)
        {
            return xmlDecoder.GetXml(basicData);
        }
        private void SearchEngine_SearchResultEvent(IBasicData obj)
        {
            if (SearchResultEvent != null)
                SearchResultEvent(obj);


        }

        private void LoadFile(object param)
        {

            FileInfo file = param as FileInfo;
            StreamReader fs;
            if (file == null)
            {
                if (ErrorFound != null)
                    ErrorFound("Load file erro");
                return;
            }
            if (LoadStarting != null)
                LoadStarting(false, "Loading xml file to memory");

            if (file.Extension == ".cdws")
            {
                workspaceUnzip = new WorkspaceUnzip(file);
                fs = workspaceUnzip.XmlStreamReader;

            }
            else
            {
                fs = new StreamReader(file.FullName);
            }
            StringBuilder sb = new StringBuilder();
            string line;
            double length = file.Length;
            double totalPos = 0.1;
            while ((line = fs.ReadLine()) != null)
            {
                sb.Append(line);
                totalPos = sb.Length;
                int porcent = (int)(totalPos * 100 / length);
                if (FilePorcentLoad != null)
                    FilePorcentLoad(porcent);
            }
            xmlDecoder.XmlString = sb.ToString();
            if (LoadStarting != null)
                LoadStarting(true, "Deserializing xml");
            sb = null;
            fs.Close();
            fs.Dispose();

            try
            {
                xmlDecoder.Process();
            }
            catch (Exception erro)
            {
                if (ErrorFound != null)
                    ErrorFound("Erro - " + erro.Message);
                return;
            }

            ListPrimaryItens = xmlDecoder.FirstItens;
            searchEngine = new SearchEngine();
            searchEngine.SearchResultEvent += SearchEngine_SearchResultEvent;
            searchEngine.GenericSearchResultEvent += SearchEngine_GenericSearchResultEvent;
            searchEngine.SearchStarting += SearchEngine_StartSearch;
            searchEngine.SearchFinished += SearchEngine_SearchFinished;
            searchEngine.SearchMessage += SearchEngine_SearchMessage;
            if (LoadListsFinish != null)
                LoadListsFinish();

        }

        public void DispactchNewMessage(string message, MsgType msgType)
        {
            if (NewMessage != null)
                NewMessage(message, msgType);
        }
        private void SearchEngine_SearchMessage(string obj)
        {

        }

        private void SearchEngine_SearchFinished()
        {
            if (LoadFinish != null)
                LoadFinish("Finished");
        }

        private void SearchEngine_GenericSearchResultEvent(List<object> obj)
        {

        }

        private void SearchEngine_StartSearch()
        {
            if (LoadStarting != null)
                LoadStarting(true, "Searching");
        }
        public string TryGetAnyCaption(IBasicData basicData)
        {
            string caption = "";
            if (!string.IsNullOrEmpty(basicData.Caption))
            {
                caption = basicData.Caption;
                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            //if (corelApp != null)
            //{
            //    caption = corelAutomation.GetItemCaption(basicData.Guid);
            //    lba_captionLoc.Content = "Automation GetCaption";
            //    if (!string.IsNullOrEmpty(caption))
            //        return;

            //}
            string[] searchs = new string[] { "captionGuid", "guidRef" };
            for (int i = 0; i < searchs.Length; i++)
            {

                if (basicData.ContainsAttribute(searchs[i]))
                {
                    caption = this.SearchItemFromGuidRef(basicData, basicData.GetAttribute(searchs[i])).Caption;
                }
                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            string search = "nonLocalizableName";
            if (basicData.ContainsAttribute(search))
            {

                caption = basicData.GetAttribute(search);

                if (!string.IsNullOrEmpty(caption))
                    return caption;
            }
            return caption;
        }
        public static DependencyObject FindParentControl<T>(DependencyObject el) where T : DependencyObject
        {
            if (el == null)
                return null;
            DependencyObject parent = VisualTreeHelper.GetParent(el);
            if (parent is T)
            {
                return parent;
            }
            else
            {
                return FindParentControl<T>(parent);
            }
        }
        #region bindings properties
        private bool incorel = false;

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
        System.Windows.Forms.AutoCompleteStringCollection AutoCompleteSource { get; set; }
        public ViewModel.Commands.AttributeCommand RunBindCommand {get;set;}
        public ViewModel.Commands.AttributeCommand RunMacroCommand { get; set; }
        public ViewModel.Commands.AttributeCommand SearchGuidCommand { get; set; }
        public ViewModel.Commands.AttributeCommand CopyCommand { get; set; }
        public BitmapSource HighLightButtonImg {get{ return Properties.Resources.light.GetBitmapSource(); } }
        public BitmapSource ActiveGuidButtonImg { get { return Properties.Resources.copy.GetBitmapSource(); } }
        public BitmapSource ClearConsoleButtonImg { get { return Properties.Resources.trash.GetBitmapSource(); } }
        public BitmapSource CopyMenuItemImg { get { return Properties.Resources.copy.GetBitmapSource(); } }
        public BitmapSource PasteMenuItemImg { get { return Properties.Resources.paste.GetBitmapSource(); } }
        public BitmapSource SearchButtonImg { get { return Properties.Resources.search.GetBitmapSource(); } }
        public BitmapSource ConfigButtonImg { get { return Properties.Resources.setting.GetBitmapSource(); } }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
      
    }
    public enum MsgType
    {
        Console,
        Event
    }
    
}
