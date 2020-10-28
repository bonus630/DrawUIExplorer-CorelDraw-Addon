using br.corp.bonus630.DrawUIExplorer.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.corp.bonus630.DrawUIExplorer.DataClass
{
    public abstract class BasicData<T> : ViewModelBase, IComparable, IBasicData 
    {
        public string Guid { get; set; }
        public string GuidRef { get; set; }
        public string TagName { get; set; }
        public int PathIndex { get; private set; }

        public string TagValue { get { if (Childrens == null || Childrens.Count == 0) return TagName; else return string.Format("{0} [{1}]", TagName, Childrens.Count); } }
        public string Caption { get; set; }
        private Type type;
        private int xmlChildreID;
        private int xmlChildreParentID;
        private int treeLevel = 0;
        private bool isSelected = false;
        private bool isSpecialType = false;

        public int XmlChildreID { get { return this.xmlChildreID; } }
        public int XmlChildreParentID { get { return this.xmlChildreParentID; }  }
        public int TreeLevel { get { return this.treeLevel; } }

        public List<Attribute> Attributes { get; set; }
        private ObservableCollection<IBasicData> childrens;
        public ObservableCollection<IBasicData> Childrens { get { 
                return childrens; } 
            set {childrens = value; NotifyPropertyChanged(); } }
        public bool IsSelected { get { return this.isSelected; }  }
        public bool IsSpecialType { get { return this.isSpecialType; } }
        public IBasicData Parent { get; set; }

        public event Action<bool,bool?,bool> SelectedEvent;


        public BasicData()
        {
            this.type = typeof(T);
            if (childrens == null)
                childrens = new ObservableCollection<IBasicData>();
            Attributes = new List<Attribute>();
            setSpecialType();
        }

        private void setSpecialType()
        {
            if (type == typeof(DockerData)|| type == typeof(CommandBarData) || type == typeof(DialogData))
                this.isSpecialType = true;

            
        }

        public void SetTreeLevel(int parentLevel)
        {
            this.treeLevel = parentLevel + 1;
        }
        public void SetPathIndex(int index)
        {
            PathIndex = index;
        }
        public int CompareTo(object obj)
        {
            BasicData<T> basicData = (obj as BasicData<T>);
            if (obj == null)
                return -1;
            if (basicData.Guid == this.Guid)
                return 0;
            return basicData.Guid.CompareTo(basicData.Guid);
            

        }
        public Type GetType(IBasicData basicData)
        {
            return typeof(T);
        }

        public bool ContainsAttribute(string AttributeName)
        {
            if (this.Attributes == null)
                return false;
            foreach (Attribute item in Attributes)
            {
                if (item.Name == AttributeName)
                    return true;
            }
            return false;
        }

        public bool ContainsAttributeValue(string AttributeValue)
        {
            if (this.Attributes == null)
                return false;
            foreach (Attribute item in Attributes)
            {
                if (item.Value == AttributeValue)
                    return true;
            }
            return false;
        }
        public string GetAttribute(string AttributeName)
        {
            if (this.Attributes == null)
                return "";
            foreach (Attribute item in Attributes)
            {
                if (item.Name == AttributeName)
                    return item.Value;
            }
            return "";
        }
        public bool ContainsAttributeValuePartial(string AttributeValue)
        {
            if (this.Attributes == null)
                return false;
            foreach (Attribute item in Attributes)
            {
                if (item.Value.ToLower().Contains(AttributeValue.ToLower()))
                    return true;
            }
            return false;
        }

        public void SetXmlChildreID(int id)
        {
            this.xmlChildreID = id;
        }
        public void SetXmlChildreParentID(int id = -1)
        {
            this.xmlChildreParentID = id;
        }
        public void Add(IBasicData basicData)
        {
            if(childrens == null)
                childrens = new ObservableCollection<IBasicData>();
            this.childrens.Add(basicData);
            NotifyPropertyChanged("Childrens");
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            IBasicData basicData = obj as IBasicData;
            if (basicData == null)
                return false;
            //This comparison can't still work in merge case
            if (this.TagName == basicData.TagName && this.Guid == basicData.Guid && this.XmlChildreID == basicData.XmlChildreID)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -1730927587;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Guid);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(GuidRef);
            return hashCode;
        }

        public void SetSelected(bool isSelected,bool? isExpands, bool update,bool recursive = false)
        {
            this.isSelected = isSelected;
            if (SelectedEvent != null)
                SelectedEvent(this.isSelected,isExpands,update);
            NotifyPropertyChanged("IsSelected");
            if (recursive && this.Parent != null)
                this.Parent.SetSelected(isSelected, isExpands, update, recursive);
        }

        public bool IAmUniqueTag()
        {
            for (int i = 0; i < this.Parent.Childrens.Count; i++)
            {
                if (this != this.Parent.Childrens[i] && this.Parent.Childrens[i].TagName == this.TagName)
                    return false;
            }
            return true;
        }
        public override string ToString()
        {
            return string.Format("{0}[{1}]",this.TagName,this.XmlChildreID);
        }

        //public IEnumerator<T> GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}
    }
  
          
}
