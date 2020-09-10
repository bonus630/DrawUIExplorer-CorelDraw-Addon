using br.corp.bonus630.DrawUIExplorer.Models;
using br.corp.bonus630.DrawUIExplorer.ViewModels.Commands;
using br.corp.bonus630.DrawUIExplorer.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.corp.bonus630.DrawUIExplorer.ViewModels
{
    public class ConfigViewModel : ViewModelBase
    {
        private bool consoleCounter;
        public event Action CloseEvent;
        private SaveLoadConfig saveLoad;
        public bool ConsoleCounter
        {
            get { return consoleCounter; }
            set { consoleCounter = value;NotifyPropertyChanged(); }
        }
        private string section = "General";

        public string Section
        {
            get { return section ; }
            set { section  = value; NotifyPropertyChanged(); }
        }

        private SimpleCommand saveCommmand;
        private SimpleCommand closeCommand;
        
        public ConfigViewModel()
        {
            saveCommmand = new SimpleCommand(save);
            closeCommand =new  SimpleCommand(close);
            saveLoad = new SaveLoadConfig();
            load();
        }

        public SimpleCommand SaveCommand { get { return saveCommmand; } }
        public SimpleCommand CloseCommand { get { return closeCommand; } }
        private void save()
        {
            saveLoad.ConsoleCounter = consoleCounter;
            saveLoad.Save();
            close();
        }
        private void load()
        {
            ConsoleCounter = saveLoad.ConsoleCounter;
        }
        private void close()
        {
            if (CloseEvent != null)
                CloseEvent();
        }

    }
}
