using System.ComponentModel;
using System.Windows;


namespace Museum_Admin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Help with swapping the XAML files in the same window
        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/60446b64-b954-421e-a52e-63dfd9e65011/how-to-dynamically-load-user-controls-in-the-mainwindow-in-wpf?forum=wpf
        public FrameworkElement MainWindowContent { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            WindowState = WindowState.Maximized;

            Tools.hasDataChanged = false;

            // Just go ahead and load the Veteran Edit, that's the main item
            NavBtn_Veterans_Click(this, null);
        }

        private void NavBtn_Veterans_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.No;

            if (Tools.hasDataChanged)
            {
                result = MessageBox.Show(Tools.unsavedMessage, Tools.unsavedTitle, MessageBoxButton.YesNo);
            }

            if (!Tools.hasDataChanged || result == MessageBoxResult.Yes)
            {
                DataContext = null;
                MainWindowContent = new Veterans(this);
                DataContext = this;

                // User has changed the page, discarding the changed data
                Tools.hasDataChanged = false;
            }
        }

        private void NavBtn_Cemeteries_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.No;

            if (Tools.hasDataChanged)
            {
                result = MessageBox.Show(Tools.unsavedMessage, Tools.unsavedTitle, MessageBoxButton.YesNo);
            }

            if (!Tools.hasDataChanged || result == MessageBoxResult.Yes)
            {
                DataContext = null;
                MainWindowContent = new Cemeteries();
                DataContext = this;

                // User has changed the page, discarding the changed data
                Tools.hasDataChanged = false;
            }
        }

        private void NavBtn_Branches_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.No;

            if (Tools.hasDataChanged)
            {
                result = MessageBox.Show(Tools.unsavedMessage, Tools.unsavedTitle, MessageBoxButton.YesNo);
            }

            if (!Tools.hasDataChanged || result == MessageBoxResult.Yes)
            {
                DataContext = null;
                MainWindowContent = new Branches();
                DataContext = this;

                // User has changed the page, discarding the changed data
                Tools.hasDataChanged = false;
            }
        }

        private void NavBtn_Ranks_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.No;

            if (Tools.hasDataChanged)
            {
                result = MessageBox.Show(Tools.unsavedMessage, Tools.unsavedTitle, MessageBoxButton.YesNo);
            }

            if (!Tools.hasDataChanged || result == MessageBoxResult.Yes)
            {
                DataContext = null;
                MainWindowContent = new Ranks();
                DataContext = this;

                // User has changed the page, discarding the changed data
                Tools.hasDataChanged = false;
            }
        }

        private void NavBtn_Conflicts_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.No;

            if (Tools.hasDataChanged)
            {
                result = MessageBox.Show(Tools.unsavedMessage, Tools.unsavedTitle, MessageBoxButton.YesNo);
            }

            if (!Tools.hasDataChanged || result == MessageBoxResult.Yes)
            {
                DataContext = null;
                MainWindowContent = new Conflicts();
                DataContext = this;

                // User has changed the page, discarding the changed data
                Tools.hasDataChanged = false;
            }
        }

        private void NavBtn_Awards_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.No;

            if (Tools.hasDataChanged)
            {
                result = MessageBox.Show(Tools.unsavedMessage, Tools.unsavedTitle, MessageBoxButton.YesNo);
            }

            if (!Tools.hasDataChanged || result == MessageBoxResult.Yes)
            {
                DataContext = null;
                MainWindowContent = new Awards();
                DataContext = this;

                // User has changed the page, discarding the changed data
                Tools.hasDataChanged = false;
            }
        }

        private void NavBtn_Comments_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.No;

            if (Tools.hasDataChanged)
            {
                result = MessageBox.Show(Tools.unsavedMessage, Tools.unsavedTitle, MessageBoxButton.YesNo);
            }

            if (!Tools.hasDataChanged || result == MessageBoxResult.Yes)
            {
                DataContext = null;
                MainWindowContent = new Comments(this);
                DataContext = this;

                // User has changed the page, discarding the changed data
                Tools.hasDataChanged = false;
            }
        }

        private void NavBtn_Queries_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.No;

            if (Tools.hasDataChanged)
            {
                result = MessageBox.Show(Tools.unsavedMessage, Tools.unsavedTitle, MessageBoxButton.YesNo);
            }

            if (!Tools.hasDataChanged || result == MessageBoxResult.Yes)
            {
                DataContext = null;
                MainWindowContent = new Queries(this);
                DataContext = this;

                // User has changed the page, discarding the changed data
                Tools.hasDataChanged = false;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.No;

            if (Tools.hasDataChanged)
            {
                result = MessageBox.Show(Tools.unsavedClosingMessage, Tools.unsavedTitle, MessageBoxButton.YesNo);

                // Prompt user "Are you sure you want to close the program?"
                if (result == MessageBoxResult.No)
                {
                    // Cancel closing the program
                    e.Cancel = true;
                }
            }
        }
    }
}
