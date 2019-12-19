using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace ObsXaml
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private string cfname;
        public obsData = new DataObservations();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoadFile_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog(); //http://www.wpf-tutorial.com/dialogs/the-openfiledialog/
            if (ofd.ShowDialog() == true) //ensure user selects file
            {
                obsData.LoadFromFile(ofd.FileName);
                List<obsEntry> lbitems = new List<obsEntry>();
                foreach (KeyValuePair<string, List<int>> e in obsData)
                {
                    lbitems.Add(new obsEntry() { name = e.Key, vals = e.Value });
                }
                LBObs.ItemsSource = lbitems;
            }
        }

        public class obsEntry
        {
            public string name { get; set; }
            public List<int> vals { get; set; }
        }

        private void btnSaveFile_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog(); //http://www.wpf-tutorial.com/dialogs/the-openfiledialog/
            if (ofd.ShowDialog() == true) //ensure user selects file
            {
                obsData.SaveToFile(ofd.FileName);
            }
        }

        private void btnQuit_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
