using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using br.corp.bonus630.DrawUIExplorer.DataClass;
using System.Collections;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;
using System.Runtime.CompilerServices;

namespace br.corp.bonus630.DrawUIExplorer
{
    public enum SearchOrderResult
    {
        ASC,
        DESC,
        None
    }
    public enum SearchAttributeCondition
    {
        NAME,
        VALUE,
        Both,
        None
    }
    public class SearchEngine
    {
        private bool searching = true;
        private IBasicData searchResult;
        private bool Searching { set { searching = value; if (!searching && SearchResultEvent != null) SearchResultEvent(searchResult); } }

        public event Action<IBasicData> SearchResultEvent;
        public event Action<List<object>> GenericSearchResultEvent;

        public Action<IBasicData, string,bool> GetDataByTagName { get { return this.getDataByTagName; } }
        public Action<IBasicData, string,bool> GetDataByAttributeName { get { return this.getDataByAttributeName; } }
        public Action<IBasicData, string,bool> GetDataByAttributeValue { get { return this.getDataByAttributeValue; } }
        public Action<IBasicData, string,bool> GetDataByAttributeValuePartial { get { return this.getDataByAttributeValuePartial; } }
        public Action<IBasicData, string,bool> GetDataByGuid { get { return this.getDataByGuid; } }

        public event Action SearchStarting;
        public event Action SearchFinished;
        public event Action<string> SearchMessage;

        private int countMaxLevel = 0;
        private int maxLevel = 1;
        private List<object> genericList = new List<object>();


        public SearchEngine()
        {

        }

        public void NewSearch()
        {
            searching = true;
            countMaxLevel = 0;
            maxLevel = 1;
            searchResult = new OtherData() { TagName = "Search" };
            genericList.Clear();
            if (SearchStarting != null)
                SearchStarting();
        }
        //uso na pesquisa avançada
        private void getDataByTagName(IBasicData basicData, string tagName,bool unique = false)
        {
            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                if (basicData.Childrens[i].TagName == tagName)
                    searchResult.Add(basicData.Childrens[i]);
                if (basicData.Childrens[i].Childrens.Count > 0)
                {
                    getDataByTagName(basicData.Childrens[i],tagName);

                }
            }
        }
        //uso na pesquisa avançada
        private void getDataByAttributeName(IBasicData basicData, string attributeName,bool unique = false)
        {
            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                
                if (basicData.Childrens[i].ContainsAttribute(attributeName))
                {
                    if (unique && basicData.Childrens[i].Attributes.Count == 1)
                        searchResult.Add(basicData.Childrens[i]);
                    if(!unique)
                        searchResult.Add(basicData.Childrens[i]);
                }
                if (basicData.Childrens[i].Childrens.Count > 0)
                {
                    getDataByAttributeName(basicData.Childrens[i],attributeName,unique);

                }
            }
        }
        //uso na pesquisa avançada
        private void getDataByAttributeValue(IBasicData basicData, string attributeValue, bool unique = false)
        {
            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                if (basicData.Childrens[i].ContainsAttributeValue(attributeValue))
                {
                    if (unique && basicData.Childrens[i].Attributes.Count == 1)
                        searchResult.Add(basicData.Childrens[i]);
                    if (!unique)
                        searchResult.Add(basicData.Childrens[i]);
                }
                if (basicData.Childrens[i].Childrens.Count > 0)
                {
                    getDataByAttributeValue(basicData.Childrens[i], attributeValue,unique);

                }
            }
        }
        //uso na pesquisa avançada
        private void getDataByAttributeValuePartial(IBasicData basicData, string attributeValue, bool unique = false)
        {
            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                if (basicData.Childrens[i].ContainsAttributeValuePartial(attributeValue))
                {
                    if (unique && basicData.Childrens[i].Attributes.Count == 1)
                        searchResult.Add(basicData.Childrens[i]);
                    if (!unique)
                        searchResult.Add(basicData.Childrens[i]);
                }
                if (basicData.Childrens[i].Childrens.Count > 0)
                {
                    getDataByAttributeValuePartial(basicData.Childrens[i], attributeValue,unique);

                }
            }
        }
        public void SearchAdvanced(List<SearchAdvancedParams> actions)
        {
            NewSearch();
            var actionsEnable = actions.Where(c => c.Enable).ToList(); 
            for (int i = 0; i < actionsEnable.Count; i++)
            {
                if (actionsEnable[i].Enable)
                {
                    actionsEnable[i].SearchAction.Invoke(actionsEnable[i].SearchBasicData, actionsEnable[i].SearchParam, actionsEnable[i].IsUnique);
                    if (i + 1 < actionsEnable.Count)
                    {
                        actionsEnable[i + 1].SearchBasicData = searchResult;
                        searchResult = new OtherData() { TagName = "Search" };
                    }
                }
            }
            if (SearchResultEvent != null)
                SearchResultEvent(searchResult);
            if (SearchFinished != null)
                SearchFinished();
            //return this.searchResult;
        }
        public int CountMax(IBasicData basicData, int count = 0)
        {
            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                if (basicData.Childrens[i].Childrens.Count == 0 && basicData.Childrens[i].TreeLevel == this.maxLevel)
                {
                    count++;
                    this.countMaxLevel = count;
                }
                else
                {

                    CountMax(basicData.Childrens[i], count);
                }
            }
            return count;
        }
        public void GetMaxLevel(IBasicData basicData, int level = 0)
        {
            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                if (basicData.Childrens[i].Childrens.Count == 0)
                {
                    if (basicData.Childrens[i].TreeLevel > level)
                    {
                        level = basicData.Childrens[i].TreeLevel;
                        this.maxLevel = level;
                    }
                }
                else
                {

                    GetMaxLevel(basicData.Childrens[i], this.maxLevel);
                }
            }

        }

        public List<object> SearchAllAttributesValue(IBasicData currentBasicData, SearchOrderResult aSC)
        {
            NewSearch();
            searchAllAttributesValue(currentBasicData);
            this.genericList.Sort();
            if (SearchFinished != null)
                SearchFinished();
            return this.genericList;
        }

        private void searchAllAttributesValue(IBasicData currentBasicData)
        {
            for (int j = 0; j < currentBasicData.Attributes.Count; j++)
            {

                if (!this.genericList.Contains(currentBasicData.Attributes[j].Value))
                {
                    this.genericList.Add(currentBasicData.Attributes[j].Value);
                }
            }
            for (int i = 0; i < currentBasicData.Childrens.Count; i++)
            {
                for (int j = 0; j < currentBasicData.Childrens[i].Attributes.Count; j++)
                {

                    if (!this.genericList.Contains(currentBasicData.Childrens[i].Attributes[j].Value))
                    {
                        this.genericList.Add(currentBasicData.Childrens[i].Attributes[j].Value);
                    }
                }
                if (currentBasicData.Childrens[i].Childrens.Count > 0)
                {
                    searchAllAttributesValue(currentBasicData.Childrens[i]);

                }

            }
        }
        private void searchAllAttributesName(IBasicData currentBasicData)
        {
            for (int j = 0; j < currentBasicData.Attributes.Count; j++)
            {

                if (!this.genericList.Contains(currentBasicData.Attributes[j].Name))
                {
                    this.genericList.Add(currentBasicData.Attributes[j].Name);
                }
            }
            for (int i = 0; i < currentBasicData.Childrens.Count; i++)
            {
              
                for (int j = 0; j < currentBasicData.Childrens[i].Attributes.Count; j++)
                {

                    if (!this.genericList.Contains(currentBasicData.Childrens[i].Attributes[j].Name))
                    {
                        this.genericList.Add(currentBasicData.Childrens[i].Attributes[j].Name);
                    }
                }
                if (currentBasicData.Childrens[i].Childrens.Count > 0)
                {
                    searchAllAttributesName(currentBasicData.Childrens[i]);

                }

            }
        }

        public List<object> SearchAllAttributesName(IBasicData currentBasicData, SearchOrderResult aSC)
        {
            NewSearch();
            searchAllAttributesName(currentBasicData);
            this.genericList.Sort();
            if (SearchFinished != null)
                SearchFinished();
            return this.genericList;
        }

        private void searchAllTags(IBasicData basicData)
        {
            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                if (!genericList.Contains(basicData.Childrens[i].TagName))
                    genericList.Add(basicData.Childrens[i].TagName);
                if (basicData.Childrens[i].Childrens.Count > 0)
                {
                    searchAllTags(basicData.Childrens[i]);

                }
            }
        }
        private void searchAllAttributes(IBasicData basicData, SearchAttributeCondition attributeCondition = SearchAttributeCondition.None,
            string condition = "")
        {
            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                for (int j = 0; j < basicData.Childrens[i].Attributes.Count; j++)
                {
                    bool add = true;
                    for (int k = 0; k < this.genericList.Count; k++)
                    {
                        if (this.genericList[k].Equals(basicData.Childrens[i].Attributes[j]))
                        {
                            add = false;
                            break;
                        }

                    }
                    if (!string.IsNullOrEmpty(condition))
                    {
                        if (attributeCondition == SearchAttributeCondition.NAME && basicData.Childrens[i].Attributes[j].Name != condition)
                            add = false;
                        if (attributeCondition == SearchAttributeCondition.VALUE && basicData.Childrens[i].Attributes[j].Value != condition)
                            add = false;
                    }
                    if (add)
                    {

                        genericList.Add(basicData.Childrens[i].Attributes[j]);
                    }
                }
                if (basicData.Childrens[i].Childrens.Count > 0)
                {
                    searchAllAttributes(basicData.Childrens[i], attributeCondition, condition);

                }
            }
        }
        public void SearchAllAttributes(IBasicData basicData, SearchOrderResult searchOrderResult = SearchOrderResult.None)
        {
            NewSearch();
            this.searchAllAttributes(basicData);
            if (searchOrderResult == SearchOrderResult.DESC)
                this.genericList.Sort();
            if (searchOrderResult == SearchOrderResult.ASC)
            {
                this.genericList.Sort();
                this.genericList.Reverse();
            }
            if (GenericSearchResultEvent != null)
                GenericSearchResultEvent(genericList);
            if (SearchFinished != null)
                SearchFinished();
        }
        public List<object> SearchAllTags(IBasicData basicData)
        {
            NewSearch();
            this.searchAllTags(basicData);
            //if (GenericSearchResultEvent != null)
            //    GenericSearchResultEvent(genericList);
            this.genericList.Sort();
            if (SearchFinished != null)
                SearchFinished();
            return this.genericList;


        }
        private void getDataByGuid(IBasicData basicData, string guid,bool unique = false)
        {
            //NewSearch();
            this.searchItensFromGuid(basicData, guid);
            //if (SearchResultEvent != null)
            //    SearchResultEvent(searchResult);
        }
        private void searchItensFromGuid(IBasicData basicData, string guid)
        {
            for (int i = 0; i < basicData.Childrens.Count; i++)
            {
                if (basicData.Childrens[i].Guid == guid)
                {
                    this.searchResult.Add(basicData.Childrens[i]);

                }
                else
                {
                    if (basicData.Childrens[i].Childrens.Count > 0)
                        searchItensFromGuid(basicData.Childrens[i], guid);
                }
            }
        }

        /// <summary>
        /// Dispara o evento de pesquisa
        /// </summary>
        /// <param name="list"></param>
        /// <param name="guid"></param>
        public void SearchItemFromGuidRef(ObservableCollection<IBasicData> list, string guid)
        {
            if (!searching)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Guid == guid)
                {
                    this.searchResult.Add(list[i]);
                    Searching = false;
                    break;
                }
                else
                {
                    if (list[i].Childrens.Count > 0)
                        SearchItemFromGuidRef(list[i].Childrens, guid);
                }
            }
           
                if (SearchFinished != null)
                    SearchFinished();
           
        }
        /// <summary>
        /// Não dispara o evento de pesquisa
        /// </summary>
        /// <param name="list"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public IBasicData SearchItemFromGuidRef(IBasicData list, string guid)
        {
            searchResult = new OtherData() {TagName = "Search" };
            searching = true;
            searchItemFromGuidRef(list, guid);
            //if (SearchFinished != null)
            //    SearchFinished();
            return searchResult;
        }
        private void searchItemFromGuidRef(IBasicData list, string guid)
        {
            if (!searching)
                return;
            for (int i = 0; i < list.Childrens.Count; i++)
            {
                if (list.Childrens[i].Guid == guid)
                {
                    searchResult = list.Childrens[i];
                    searching = false;
                    break;
                }
                else
                {
                    if (list.Childrens[i].Childrens.Count > 0)
                         searchItemFromGuidRef(list.Childrens[i], guid);
                }
            }
            

        }
        public IBasicData SearchItemContainsGuidRef(IBasicData list, string guid)
        {
            //searchResult = new OtherData() { TagName = "Search" };
            searching = true;
            searchItemContainsGuidRef(list, guid);
            //if (SearchFinished != null)
            //    SearchFinished();
            return searchResult;
        }
        private void searchItemContainsGuidRef(IBasicData list, string guid)
        {
            if (!searching)
                return;
            for (int i = 0; i < list.Childrens.Count; i++)
            {
                if (list.Childrens[i].GuidRef == guid)
                {
                    searchResult = list.Childrens[i];
                    searching = false;
                    break;
                }
                else
                {
                    if (list.Childrens[i].Childrens.Count > 0)
                        searchItemContainsGuidRef(list.Childrens[i], guid);
                }
            }


        }



    }
    public class SearchAdvancedParams : System.Windows.Data.IValueConverter,INotifyPropertyChanged
    {
        private Action<IBasicData,string,bool> searchAction;

        public Action<IBasicData,string,bool> SearchAction
        {
            get { return searchAction; }
            set { searchAction = value; }
        }

        private IBasicData basicData;

        public IBasicData SearchBasicData
        {
            get { return basicData; }
            set { basicData = value; }
        }

        private string searchParam;

        public string SearchParam
        {
            get { return searchParam; }
            set { searchParam = value; }
        }
        private string condition;

        public string Condition
        {
            get { return condition; }
            set { condition = value; }
        }
        private bool unique;

        public bool IsUnique
        {
            get { return unique; }
            set { unique = value; }
        }
        private bool enable = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Enable
        {
            get { return enable; }
            set {
                enable = value;
                NotifyPropertyChanged();
                DisableButtonImage = null;
            }
        }

        public Image DisableButtonImage
        {
            get
            {
                if (this.enable)
                    return new Image() { Source = Properties.Resources.visibleEnable.GetBitmapSource() };
                else
                    return new Image() { Source = Properties.Resources.visibleDisable.GetBitmapSource() };
            }
            set
            {
                NotifyPropertyChanged();
            }
        }

        public SearchAdvancedParams(Action<IBasicData,string,bool> action,IBasicData basic, string param,bool unique = false)
        {
            this.searchAction = action;
            this.basicData = basic;
            this.searchParam = param;
            this.unique = unique;
        }
        public SearchAdvancedParams()
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            if ((bool)value)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if ((Visibility)value == Visibility.Visible)
                return true;
            return false;
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        

    }
}
