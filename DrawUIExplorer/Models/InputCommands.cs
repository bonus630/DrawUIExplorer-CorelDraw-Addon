using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace br.corp.bonus630.DrawUIExplorer.Models
{
    public class InputCommands
    {
        Core core;
        public InputCommands(Core core)
        {
            this.core = core;
        }
        public string Guid()
        {
            return System.Guid.NewGuid().ToString();
        }

        public string CQL(string cql)
        {
           return core.CorelApp.Evaluate(cql).ToString();
        }
       
    }
}
