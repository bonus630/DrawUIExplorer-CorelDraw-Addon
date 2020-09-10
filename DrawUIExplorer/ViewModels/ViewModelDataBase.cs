using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using br.corp.bonus630.DrawUIExplorer.DataClass;

namespace br.corp.bonus630.DrawUIExplorer.ViewModels
{
    public abstract class ViewModelDataBase : ViewModelBase
    {
        private IBasicData basicData;
        BitmapSource copy = Properties.Resources.copy.GetBitmapSource();
        BitmapSource paste = Properties.Resources.paste.GetBitmapSource();
        BitmapSource highLight = Properties.Resources.light.GetBitmapSource();
        public IBasicData CurrentBasicData
        {
            get { return basicData; }
            set { basicData = value; NotifyPropertyChanged(); }
        }
        public ViewModelDataBase(Core core)
        {
            this.core = core;
            core.CurrentBasicDataChanged += Update;
         
        }
        protected Core core;

        protected abstract void Update(IBasicData basicData);

        public BitmapSource CopyMenuItemImg { get { return copy; } }
        public BitmapSource PasteMenuItemImg { get { return paste; } }
        public BitmapSource HighLightButtonImg { get { return highLight; } }
        
    }
}
