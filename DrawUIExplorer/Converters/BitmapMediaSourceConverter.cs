using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace TesteXSLT_RemoveItem
{
    public class IconsImages : IValueConverter
    {
        //public Image CopyIcon { get { return new Image() { Source = Properties.Resources.copy.GetBitmapSource() }; } }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as System.Drawing.Bitmap).GetBitmapSource();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
