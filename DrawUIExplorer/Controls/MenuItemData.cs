using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace br.corp.bonus630.DrawUIExplorer
{
    public class MenuItemData : MenuItem
    {
        private DataClass.IBasicData data;
        public DataClass.IBasicData Data { get { return this.data; } set { this.data = value; } }
       

    }
}
