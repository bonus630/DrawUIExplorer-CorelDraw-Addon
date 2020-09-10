using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace br.corp.bonus630.DrawUIExplorer
{
    public static class ExtensionsMethods
    {
        public static BitmapSource GetBitmapSource(this System.Drawing.Bitmap resource)
        {
            var image = resource;
            var bitmap = new System.Drawing.Bitmap(image);
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();
            return  bitmapSource ;

        }

        public static System.Collections.ObjectModel.ObservableCollection<T> ToObservableCollection<T>(this List<T> list)
        {
            System.Collections.ObjectModel.ObservableCollection<T> ob = new System.Collections.ObjectModel.ObservableCollection<T>();
            
            for (int i = 0; i < list.Count; i++)
            {
                ob.Add(list[i]);
            }
            return ob;
        }
        public static bool IsZero(this System.Windows.Rect rect)
        {
            if (rect.Width == 0 && rect.Height == 0)
                return true;
            else
                return false;
        }
        public static System.Windows.Forms.Keys ToWinforms(this System.Windows.Input.ModifierKeys modifier)
        {
            var retVal = System.Windows.Forms.Keys.None;
            if (modifier.HasFlag(System.Windows.Input.ModifierKeys.Alt))
            {
                retVal |= System.Windows.Forms.Keys.Alt;
            }
            if (modifier.HasFlag(System.Windows.Input.ModifierKeys.Control))
            {
                retVal |= System.Windows.Forms.Keys.Control;
            }
            if (modifier.HasFlag(System.Windows.Input.ModifierKeys.None))
            {
                // Pointless I know
                retVal |= System.Windows.Forms.Keys.None;
            }
            if (modifier.HasFlag(System.Windows.Input.ModifierKeys.Shift))
            {
                retVal |= System.Windows.Forms.Keys.Shift;
            }
            if (modifier.HasFlag(System.Windows.Input.ModifierKeys.Windows))
            {
                // Not supported lel
            }
            return retVal;
        }

    }

}
