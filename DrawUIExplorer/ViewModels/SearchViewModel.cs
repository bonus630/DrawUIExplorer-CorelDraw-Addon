using br.corp.bonus630.DrawUIExplorer.DataClass;
using br.corp.bonus630.DrawUIExplorer.Models;
using br.corp.bonus630.DrawUIExplorer.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace br.corp.bonus630.DrawUIExplorer.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private SearchEngine searchEngine;
       
        private ObservableCollection<SearchAdvancedParamsViewModel> advancedSearchListAction = new ObservableCollection<SearchAdvancedParamsViewModel>();
        public ObservableCollection<SearchAdvancedParamsViewModel> AdvancedSearchListAction { get { return advancedSearchListAction; } set { this.advancedSearchListAction = value;NotifyPropertyChanged();  } }

        private List<object> tags;

        public List<object> Tags
        {
            get { return tags; }
            set { tags = value;NotifyPropertyChanged(); }
        }
        private List<object> attributesName;

        public List<object> AttributesName
        {
            get { return attributesName; }
            set { attributesName = value; NotifyPropertyChanged(); }
        }
        private List<object> attributesValue;

        public List<object> AttributesValue
        {
            get { return attributesValue; }
            set { attributesValue = value; NotifyPropertyChanged(); }
        }

        public RoutedCommand<IBasicData> Search { get {return new RoutedCommand<IBasicData>(search); } }
        public RoutedCommand<string> AddParam { get { return new RoutedCommand<string>(addParam); } }
        public RoutedCommand<string> GetParam { get { return new RoutedCommand<string>(getParam); } }
        public RoutedCommand<bool> SetLocalData { get { return new RoutedCommand<bool>(setLocalData); } }
        public RoutedCommand<bool> SetGlobalData { get { return new RoutedCommand<bool>(setGlocalData); } }
        public SimpleCommand CopyGuid { get { return new SimpleCommand(menuItemCopyGuid); } }
        public SimpleCommand PastGuid { get { return new SimpleCommand(menuItemPastGuid); } }
        public RoutedCommand<bool> SetAttributeTag { get { return new RoutedCommand<bool>(setAttributeTag); } }

        protected IBasicData sBasicData;
        public IBasicData SearchBasicData
        {
            get { return sBasicData; }
            set { sBasicData = value; NotifyPropertyChanged(); }
        }
        public SearchViewModel(Core core): base(core)
        {
            this.searchEngine = core.SearchEngineGet;
            searchEngine.GenericSearchResultEvent += SearchEngine_GenericSearchResultEvent;
        }
        private void SearchEngine_GenericSearchResultEvent(List<object> obj)
        {
            AdvancedSearchListAction.Clear();
            for (int i = 0; i < obj.Count; i++)
            {
                AdvancedSearchListAction.Add(obj[i] as SearchAdvancedParamsViewModel);
            }
            
        }

        private void search(IBasicData sBasicData)
        {
            if (sBasicData != null)
            {
                for (int i = 0; i < this.AdvancedSearchListAction.Count; i++)
                {
                    (this.AdvancedSearchListAction[i] as SearchAdvancedParamsViewModel).SearchBasicData = sBasicData;
                }
                //searchEngine.NewSearch();
                ////searchEngine.SearchItemFromGuidRef(currentBasicData.Childrens, txt_guid.Text);

                //searchEngine.SearchAllAttributes(currentBasicData,SearchOrderResult.ASC);

                searchEngine.SearchAdvanced(this.advancedSearchListAction.ToList());
            }
        }
        protected override void Update(IBasicData data)
        {
            if (!data.Equals(this.CurrentBasicData))
            {
                CurrentBasicData = data;
                if (localData)
                    SearchBasicData = this.CurrentBasicData;
                LocalDataName = this.CurrentBasicData.TagName;
                GlobalDataName = core.ListPrimaryItens.TagName;
            }
        }

        private bool isUnique;

        public bool IsUnique
        {
            get { return isUnique; }
            set { isUnique = value;NotifyPropertyChanged(); }
        }
        private bool uniqueName;

        public bool UniqueName
        {
            get { return uniqueName; }
            set { uniqueName = value; NotifyPropertyChanged(); }
        }
        private bool uniqueValue;

        public bool UniqueValue
        {
            get { return uniqueValue; }
            set { uniqueValue = value; NotifyPropertyChanged(); }
        }
        private string tag;

        public string Tag
        {
            get { return tag; }
            set { tag = value;NotifyPropertyChanged(); }
        }
        private string attributeName;

        public string AttributeName
        {
            get { return attributeName; }
            set { attributeName = value; NotifyPropertyChanged(); }
        }
        private string attributeValue;

        public string AttributeValue
        {
            get { return attributeValue; }
            set { attributeValue = value; NotifyPropertyChanged(); }
        }
        private string localDataName;
        public string LocalDataName
        {
            get { return localDataName; }
            set { localDataName = value; NotifyPropertyChanged(); }
        }
        private string globalDataName;
        public string GlobalDataName
        {
            get { return globalDataName; }
            set { globalDataName = value; NotifyPropertyChanged(); }
        }
        private void addParam(string tag)
        {
            try
            {
                SearchAdvancedParamsViewModel sap = new SearchAdvancedParamsViewModel();

                sap.SearchBasicData = this.CurrentBasicData;
               // string tag = (sender as Button).Tag.ToString();

                switch (tag)
                {
                    case "TagName":
                        sap.SearchParam = Tag;
                        sap.SearchAction = searchEngine.GetDataByTagName;
                        sap.Condition = "Tag Name = ";
                        break;
                    case "AttributeName":
                        sap.SearchParam = AttributeName;
                        sap.SearchAction = searchEngine.GetDataByAttributeName;
                        sap.IsUnique = UniqueName;
                        sap.Condition = "Attribute Name = ";
                        break;
                    case "AttributeValue":
                        sap.SearchParam = AttributeValue;
                        sap.SearchAction = searchEngine.GetDataByAttributeValue;
                        sap.IsUnique = UniqueValue;
                        sap.Condition = "Attribute Value = ";
                        break;
                    case "Guid":
                        sap.SearchParam = Guid;
                        sap.SearchAction = searchEngine.GetDataByGuid;
                        sap.Condition = "Guid = ";
                        break;
                    case "AttributeValuePartial":
                        sap.SearchParam = AttributeValue;
                        sap.SearchAction = searchEngine.GetDataByAttributeValuePartial;
                        sap.IsUnique = UniqueValue;
                        sap.Condition = "Attribute Value % ";
                        break;
                }

                this.AdvancedSearchListAction.Add(sap);
                //listView_tags.ItemsSource = null;
                //listView_tags.ItemsSource = this.AdvancedSearchListAction;
            }
            catch (Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Console);
            }

        }





        private void getParam(string tag)
        {
            //string tag = (sender as Button).Tag.ToString();

            switch (tag)
            {
                case "TagName":
                        Tags  = searchEngine.SearchAllTags(this.CurrentBasicData);
                    break;
                case "AttributeName":
                        AttributesName = searchEngine.SearchAllAttributesName(this.CurrentBasicData, SearchOrderResult.ASC);
                    break;
                case "AttributeValue":
                        AttributesValue = searchEngine.SearchAllAttributesValue(this.CurrentBasicData, SearchOrderResult.ASC);
                    break;
            }

        }

        private string attributeValueTag;

        public string AttributeValueTag
        {
            get { return attributeValueTag; }
            set { attributeValueTag = value; }
        }


        private void setAttributeTag(bool like)
        {
            if (like)
            {
                AttributeValueTag = "AttributeValuePartial";
            }
            else
            {
                AttributeValueTag = "AttributeValue";
            }
        }
        public string GetGuid(string text)
        {
            Regex reg = new Regex("[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            Match math = reg.Match(text);
            return math.Value;

        }

        private string guid;

        public string Guid
        {
            get { return guid; }
            set { guid = value; }
        }
        private bool localData = true;

        private void menuItemCopyGuid()
        {
            Clipboard.SetText(GetGuid(Guid));
        }

        private void menuItemPastGuid()
        {
            Guid = GetGuid(Clipboard.GetText());
        }
        private void setLocalData(bool s)
        {
            if (s)
            {
                localData = true;
                SearchBasicData = CurrentBasicData;
            }
        }
        private void setGlocalData(bool s)
        {

            if (s)
            {
                localData = false;
                SearchBasicData = core.ListPrimaryItens;
            }
        }
    }
}
