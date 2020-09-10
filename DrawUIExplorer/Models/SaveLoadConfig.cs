using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.corp.bonus630.DrawUIExplorer.Models
{
    public class SaveLoadConfig
    {
        private bool consoleCounter;
        public bool ConsoleCounter { 
            get { consoleCounter = Properties.Settings.Default.ConsoleCounter; return consoleCounter; } 
            set{ consoleCounter = value; } }
        public void Save()
        {
            Properties.Settings.Default.ConsoleCounter = consoleCounter;
            Properties.Settings.Default.Save();
        }
    }
}
