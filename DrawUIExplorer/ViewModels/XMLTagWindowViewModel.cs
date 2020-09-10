using br.corp.bonus630.DrawUIExplorer.DataClass;
using br.corp.bonus630.DrawUIExplorer.Models;
using br.corp.bonus630.DrawUIExplorer.ViewModels.Commands;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace br.corp.bonus630.DrawUIExplorer.ViewModels
{
    class XMLTagWindowViewModel : ViewModelDataBase
    {
        private bool incorel = false;

        public bool InCorel
        {
            get { return incorel; }
            set
            {
                incorel = value;
                NotifyPropertyChanged();
            }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }
        private bool consoleExpanded;

        public bool ConsoleExpanded
        {
            get { return consoleExpanded; }
            set
            {
                consoleExpanded = value;
                NotifyPropertyChanged();
            }
        }
        
        public BitmapSource HighLightButtonImg { get { return Properties.Resources.light.GetBitmapSource(); } }
        System.Windows.Forms.AutoCompleteStringCollection AutoCompleteSource { get; set; }

        //public SimpleCommand configCommand, clearConsoleCommmand, expandConsoleCommand, highLightCommand, activeGuidCommand;

        public SimpleCommand ConfigCommand { get { return new SimpleCommand(config); } }
        public SimpleCommand ExpandConsoleCommand { get { return new SimpleCommand(expandConsole); } }
        public SimpleCommand ActiveGuidCommand { get { return new SimpleCommand(activeGuid); } }
        public SimpleCommand HighLightCommand { get { return new SimpleCommand(showHighLightItem); } }

        public XMLTagWindowViewModel(Core core):base(core)
        {
            autoCompleteInputCommand();
          
        }
        protected override void Update(IBasicData basicData)
        {
            CurrentBasicData = basicData;
        }
        private void autoCompleteInputCommand()
        {
            AutoCompleteSource = new System.Windows.Forms.AutoCompleteStringCollection();
            MethodInfo[] m = (typeof(InputCommands)).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < m.Length; i++)
            {
                AutoCompleteSource.Add(m[i].Name);
            }

        }
     

        private void showHighLightItem()
        {
            core.CorelAutomation.ShowHighLightItem(core.Route);
        }
        private void activeGuid()
        {
             //core.CopyItemCaptionAndGuid();
        }

        private void config()
        {
            Views.Config config = new Views.Config();
            config.ShowDialog();
        }
        private void expandConsole()
        {
            this.ConsoleExpanded = !this.ConsoleExpanded;
        }
    }
}
