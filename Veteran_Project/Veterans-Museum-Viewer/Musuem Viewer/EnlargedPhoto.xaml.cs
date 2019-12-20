using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;

namespace Musuem_Viewer
{
    /// <summary>
    /// Interaction logic for EnlargedPhoto.xaml
    /// </summary>
    public partial class EnlargedPhoto : Window
    {
        public EnlargedPhoto(string fileName)
        {
            InitializeComponent();

            WindowState = WindowState.Maximized;

            try
            {
                Img_PhotoLg.Source = new BitmapImage(new Uri(fileName, UriKind.Absolute));
            }
            catch (FileNotFoundException)
            {
                // Don't load missing files.
            }
            catch (DirectoryNotFoundException)
            {
                // Don't load missing files.
            }
        }

        private void Btn_CloseWin_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Img_PhotoLg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
