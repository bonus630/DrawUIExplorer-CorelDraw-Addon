using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using br.corp.bonus630.DrawUIExplorer.DataClass;

namespace br.corp.bonus630.DrawUIExplorer
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : UserControl
    {
        private SearchEngine searchEngine;
        private IBasicData currentBasicData;
        private List<SearchAdvancedParams> AdvancedSearchListAction = new List<SearchAdvancedParams>();
        private Core core;
        //private ObservableCollection<object> itemSourceTags = new ObservableCollection<object>();
        //private ObservableCollection<object> itemSourceAttName = new ObservableCollection<object>();
        //private ObservableCollection<object> itemSourceAttValue = new ObservableCollection<object>();


        public Search(SearchEngine searchEngine,Core core)
        {
            InitializeComponent();
            //iconCopy.Source = Properties.Resources.copy.GetBitmapSource();
            //iconPaste.Source = Properties.Resources.paste.GetBitmapSource();
            this.searchEngine = searchEngine;
            searchEngine.GenericSearchResultEvent += SearchEngine_GenericSearchResultEvent;
            //cb_tags.DataContext = itemSourceTags;
            //cb_attributeName.DataContext = itemSourceAttName;
            //cb_attributeValue.DataContext = itemSourceAttValue;
            this.DataContext = core;
            this.core = core;
            listView_tags.ItemsSource = AdvancedSearchListAction;
        }

        private void SearchEngine_GenericSearchResultEvent(List<object> obj)
        {
            listView_tags.ItemsSource = obj;
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            IBasicData sBasicData = null;
            if (rb_allTag.IsChecked == true)
                sBasicData = core.ListPrimaryItens;
            if (rb_currentTag.IsChecked == true)
                sBasicData = currentBasicData;
            if (sBasicData != null)
            {
                for (int i = 0; i < this.AdvancedSearchListAction.Count; i++)
                {
                    this.AdvancedSearchListAction[i].SearchBasicData = sBasicData;
                }
                //searchEngine.NewSearch();
                ////searchEngine.SearchItemFromGuidRef(currentBasicData.Childrens, txt_guid.Text);

                //searchEngine.SearchAllAttributes(currentBasicData,SearchOrderResult.ASC);

                searchEngine.SearchAdvanced(this.AdvancedSearchListAction);
            }
        }
        public void Update(IBasicData data)
        {
            if (!data.Equals(this.currentBasicData))
            {
                this.currentBasicData = data;
               // itemSourceTags = searchEngine.SearchAllTags(this.currentBasicData).ToObservableCollection<object>();
               // itemSourceAttName = searchEngine.SearchAllAttributesName(this.currentBasicData, SearchOrderResult.ASC).ToObservableCollection<object>();
              //  itemSourceAttValue = searchEngine.SearchAllAttributesValue(this.currentBasicData, SearchOrderResult.ASC).ToObservableCollection<object>();
            }
        }
        private void btn_set_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SearchAdvancedParams sap = new SearchAdvancedParams();

                sap.SearchBasicData = this.currentBasicData;
                string tag = (sender as Button).Tag.ToString();
                
                switch (tag)
                {
                    case "TagName":
                        sap.SearchParam = cb_tags.SelectedItem.ToString();
                        sap.SearchAction = searchEngine.GetDataByTagName;
                        sap.Condition = "Tag Name = ";
                        break;
                    case "AttributeName":
                        sap.SearchParam = cb_attributeName.SelectedItem.ToString();
                        sap.SearchAction = searchEngine.GetDataByAttributeName;
                        sap.IsUnique = (bool)cb_uniqueName.IsChecked;
                        sap.Condition = "Attribute Name = ";
                        break;
                    case "AttributeValue":
                        sap.SearchParam = cb_attributeValue.SelectedItem.ToString();
                        sap.SearchAction = searchEngine.GetDataByAttributeValue;
                        sap.IsUnique = (bool)cb_uniqueValue.IsChecked;
                        sap.Condition = "Attribute Value = ";
                        break;
                    case "Guid":
                        sap.SearchParam = txt_guid.Text;
                        sap.SearchAction = searchEngine.GetDataByGuid;
                        sap.Condition = "Guid = ";
                        break;
                    case "AttributeValuePartial":
                        sap.SearchParam = textBox_attributeValue.Text;
                        sap.SearchAction = searchEngine.GetDataByAttributeValuePartial;
                        sap.IsUnique = (bool)cb_uniqueValue.IsChecked;
                        sap.Condition = "Attribute Value % ";
                        break;
                }

                this.AdvancedSearchListAction.Add(sap);
                listView_tags.ItemsSource = null;
                listView_tags.ItemsSource = this.AdvancedSearchListAction;
            }
            catch(Exception erro)
            {
                core.DispactchNewMessage(erro.Message, MsgType.Console);
            }
          
        }

      



        private void btn_get_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();

            switch (tag)
            {
                case "TagName":
                    cb_tags.ItemsSource = null;
                    cb_tags.ItemsSource = searchEngine.SearchAllTags(this.currentBasicData);
                    break;
                case "AttributeName":
                    cb_attributeName.ItemsSource = null;
                    cb_attributeName.ItemsSource = searchEngine.SearchAllAttributesName(this.currentBasicData, SearchOrderResult.ASC);
                    break;
                case "AttributeValue":
                    cb_attributeValue.ItemsSource = null;
                    cb_attributeValue.ItemsSource = searchEngine.SearchAllAttributesValue(this.currentBasicData, SearchOrderResult.ASC);
                    break;
            }
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            //DependencyObject el1 = VisualTreeHelper.GetParent(sender as Button);
            //DependencyObject el2 = VisualTreeHelper.GetParent(el1);
            //DependencyObject el3 = VisualTreeHelper.GetParent(el2);
            //DependencyObject el = VisualTreeHelper.GetParent(el3);

            DependencyObject el = Core.FindParentControl<ListViewItem>(sender as Button);

            this.AdvancedSearchListAction.Remove((SearchAdvancedParams)(el as ListViewItem).DataContext);

            listView_tags.ItemsSource = null;
            listView_tags.ItemsSource = this.AdvancedSearchListAction;

        }
        private void btnDisableSearchItem(object sender, RoutedEventArgs e)
        {
            DependencyObject el = Core.FindParentControl<ListViewItem>(sender as Button);

            SearchAdvancedParams searchItem = ((SearchAdvancedParams)(el as ListViewItem).DataContext);

            searchItem.Enable = !searchItem.Enable;

        }

        private void rb_att_like_Click(object sender, RoutedEventArgs e)
        {
            if((bool)rb_att_like.IsChecked)
            {
                cb_attributeValue.Visibility = Visibility.Collapsed;
                btn_getAttributeValue.Visibility = Visibility.Collapsed;
                textBox_attributeValue.Visibility = Visibility.Visible;
                btn_setAttributeValue.Tag = "AttributeValuePartial";
            }
            else
            {
                cb_attributeValue.Visibility = Visibility.Visible;
                btn_getAttributeValue.Visibility = Visibility.Visible;
                textBox_attributeValue.Visibility = Visibility.Collapsed;
                btn_setAttributeValue.Tag = "AttributeValue";
            }
        }
        public string GetGuid(string text)
        {
            Regex reg = new Regex("[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            Match math = reg.Match(text);
            return math.Value;

        }

        private void menuItemCopyGuid_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(GetGuid(txt_guid.Text));
        }

        private void menuItemPastGuid_Click(object sender, RoutedEventArgs e)
        {
            txt_guid.Text = GetGuid(Clipboard.GetText());
        }
    }
}
