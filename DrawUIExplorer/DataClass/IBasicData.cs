using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.corp.bonus630.DrawUIExplorer.DataClass
{
    public interface IBasicData
    {
        string Guid { get; set; }
        string GuidRef { get; set; }
        string TagName { get; set; }
        bool IsSelected { get;  }
        void SetSelected(bool isSelected,bool? isExpands, bool update);
        string Caption { get; set; }
        int XmlChildreID { get; }
        int XmlChildreParentID { get; }
        int TreeLevel {  get; }
        List<Attribute> Attributes { get; set; }
        IBasicData Parent { get; set; }
        ObservableCollection<IBasicData> Childrens { get; set; }

        Type GetType(IBasicData basicData);
         bool ContainsAttribute(string AttributeName);
         bool ContainsAttributeValue(string AttributeValue);
        bool ContainsAttributeValuePartial(string AttributeValue);
        string GetAttribute(string AttributeName);
        void SetXmlChildreID(int id);
         void SetXmlChildreParentID(int id = -1);
        void SetTreeLevel(int parentLevel);
         event Action<bool,bool?,bool> SelectedEvent;
        void Add(IBasicData basicData);
        bool IAmUniqueTag();
        bool IsSpecialType { get; }
    }
}
