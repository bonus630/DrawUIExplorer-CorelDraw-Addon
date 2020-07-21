using br.corp.bonus630.DrawUIExplorer.DataClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace br.corp.bonus630.DrawUIExplorer.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private IBasicData basicData;
        public IBasicData CurrentBasicData{
            get { return basicData; }
            set { basicData = value; NotifyPropertyChanged(); }
            }

        protected Core core;
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase(Core core)
        {
            this.core = core;
            core.CurrentBasicDataChanged += Update;
        }

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected abstract void Update(IBasicData basicData);
        
    }
}
