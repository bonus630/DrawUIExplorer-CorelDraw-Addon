using System.Windows.Controls;

namespace br.corp.bonus630.DrawUIExplorer
{
    public class ButtonImage : Button
    {
        public string ContentImage { get { return ""; } set { this.Content = new Image() { Source = Properties.Resources.copy.GetBitmapSource() }; } }

    }
}
