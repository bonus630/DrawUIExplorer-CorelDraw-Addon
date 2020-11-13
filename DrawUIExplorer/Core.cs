using br.corp.bonus630.DrawUIExplorer.DataClass;
using br.corp.bonus630.DrawUIExplorer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace br.corp.bonus630.DrawUIExplorer
{
    public class Core 
    {
        XMLDecoder xmlDecoder;
        WorkspaceUnzip workspaceUnzip;
        string workerFolder;

       

        public event Action<string> LoadXmlFinish;
        public event Action<bool> RequestUIHideVisibleChanged;
        public event Action LoadListsFinish;
        public event Action<bool, string> LoadStarting;
        public event Action<string> LoadFinish;
        public event Action<int> FilePorcentLoad;
        public event Action<string> ErrorFound;
        public event Action<IBasicData> SearchResultEvent;
        //public event Action<List<object>> GenericSearchResultEvent;
        public event Action<string, MsgType> NewMessage;
        //public event PropertyChangedEventHandler PropertyChanged;
        public event Action<IBasicData> CurrentBasicDataChanged;

        public IBasicData ListPrimaryItens { get; set; }
        public IBasicData CurrentBasicData
        {
            get { return this.currentData; }
            set
            {
                this.currentData = value;
                if (CurrentBasicDataChanged != null) CurrentBasicDataChanged(this.currentData);
            }
        }
        public SearchEngine SearchEngineGet { get { return this.searchEngine; } }
        private SearchEngine searchEngine;
        private MethodInfo[] commands;
        public MethodInfo[] Commands { get { return commands; } }
        private InputCommands inputCommands;
        private IBasicData currentData;
        private Corel.Interop.VGCore.Application app;

        public Corel.Interop.VGCore.Application CorelApp
        {
            get { return app; }
        }
        public bool InCorel { get; private set; }
        public string Title { get; private set; }
        public CorelAutomation CorelAutomation { get; private set; }
        public List<IBasicData> Route { get { return getRoute(); } }
        public bool SetUIVisible { set { if (RequestUIHideVisibleChanged != null) RequestUIHideVisibleChanged(value); } }
        public Core()
        {
            workerFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\bonus630";
        }
        private List<IBasicData> getRoute()
        {
            List<IBasicData> temp = new List<IBasicData>();
            temp.Add(CurrentBasicData);
            iRoute(temp);
            temp.Reverse();
            return temp;
        }
        private void iRoute(List<IBasicData> c)
        {
            try
            {
                IBasicData basicData = c[c.Count - 1];
                if (basicData != null && basicData.Parent !=null )
                {

                    c.Add(basicData.Parent);
                    iRoute(c);
                }
            }
            catch { }
        }
        public void StartCore(string filePath, Corel.Interop.VGCore.Application corelApp)
        {
            if (corelApp != null)
            {
                InCorel = true;
                CorelAutomation = new CorelAutomation(corelApp, this);
                this.app = corelApp;
                corelApp.OnApplicationEvent += CorelApp_OnApplicationEvent;
            }
            FileInfo file = null;
            try
            {
                FileInfo fileOri = new FileInfo(filePath);
                Title = filePath;
                try
                {
                    if (!Directory.Exists(workerFolder))
                        Directory.CreateDirectory(workerFolder);
                }
                catch (IOException ioE)
                {
                    if (ErrorFound != null)
                        ErrorFound("Erro - " + ioE.Message);
                    return;
                }
                string newPath = workerFolder + "\\" + fileOri.Name;
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


        }
        public void MergeProcess(string filePath)
        {
            FileInfo file = null;
            try
            {
                FileInfo fileOri = new FileInfo(filePath);
                Title = filePath;
                try
                {
                    if (!Directory.Exists(workerFolder))
                        Directory.CreateDirectory(workerFolder);
                }
                catch (IOException ioE)
                {
                    if (ErrorFound != null)
                        ErrorFound("Erro - " + ioE.Message);
                    return;
                }
                string newPath = workerFolder + "\\" + fileOri.Name;
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
            Thread thread = new Thread(new ParameterizedThreadStart(LoadFile));
            thread.IsBackground = true;
            thread.Start(file);
        }
        private void CorelApp_OnApplicationEvent(string EventName, ref object[] Parameters)
        {
            try
            {
                string eventName = EventName;
                for (int i = 0; i < Parameters.Length; i++)
                {
                    EventName += " Param" + i + "|Type:" + Parameters[i].GetType() + "Value:" + Parameters[i].ToString();
                }
                DispactchNewMessage(eventName, MsgType.Event);
            }
            catch(Exception erro)
            {
                DispactchNewMessage(erro.Message, MsgType.Console);
            }
        }
        public string RunCommand(string commandName)
        {
            try
            {
                string result = "";
                string[] pierces = commandName.Split(" ".ToCharArray());
                int j = 0;
                List<object> param = new List<object>();
                while (!string.IsNullOrEmpty(pierces[j]) || pierces[j] == " ")
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
                while (j < pierces.Length && (!string.IsNullOrEmpty(pierces[j]) || pierces[j] == " "))
                {
                    if (!string.IsNullOrEmpty(pierces[j]) && pierces[j] != " ")
                    {
                        param.Add(pierces[j]);
                    }
                    j++;
                }
                for (int i = 0; i < commands.Length; i++)
                {
                    if (commands[i].Name == commandName)
                        result = commands[i].Invoke(inputCommands, param.ToArray()).ToString();
                }
                return result;
            }
            catch (Exception err)
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
        /// Dispara o evento de pesquisa
        /// </summary>
        /// <param name="list"></param>
        /// <param name="guid"></param>
        public void FindItemContainsGuidRef(IBasicData list,string guid)
        {
            searchEngine.NewSearch();
            searchEngine.SearchItemContainsGuidRefEvent(list, guid);
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
        public IBasicData SearchItemContainsGuidRef(string guid)
        {
            return searchEngine.SearchItemContainsGuidRef(this.ListPrimaryItens, guid);
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
                xmlDecoder.Process(file.FullName);
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
    }

    public enum MsgType
    {
        Console,
        Event
    }

}
