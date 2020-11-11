using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace br.corp.bonus630.DrawUIExplorer.Converters
{
    public class IntAttVisibilityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
           
            if (value == null)
                return System.Windows.Visibility.Collapsed;
            if ((int)value[0] == 0)
                return System.Windows.Visibility.Collapsed;
            if(value.Length<2)
                return System.Windows.Visibility.Collapsed;
            if ((value[1] as System.Windows.Controls.ItemCollection).Count == 0)
                return System.Windows.Visibility.Collapsed;
            else
            {
                foreach (var item in (value[1] as System.Windows.Controls.ItemCollection))
                {
                   DataClass.Attribute att = (item as DataClass.Attribute);
                    if (att.Name != "guid" && att.IsGuid)
                           return System.Windows.Visibility.Visible;
                }
            }
            return System.Windows.Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return new object[] { 0, 0 };
        }
    
    
    }
}
