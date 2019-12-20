using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.Configuration;

namespace Musuem_Viewer
{
    /// <summary>
    /// Interaction logic for CemeteryInformation.xaml
    /// </summary>
    public partial class CemeteryInformation : Window
    {
        public VeteranDBInfo Veteran { get; set; }
        private string cemeteryAirPicFile = "";
        private string directionsFile = "";

        public CemeteryInformation(VeteranDBInfo record)
        {
            InitializeComponent();

            WindowState = WindowState.Maximized;

            DataContext = this;

            Veteran = record;

            LoadCemeteryPictures();
        }

        // Loads the cemetery pictures to the UI
        private void LoadCemeteryPictures()
        {
            directionsFile = ConfigurationManager.AppSettings["InstallDirectory"];
            directionsFile += ConfigurationManager.AppSettings["CemDirectionsDirectory"];
            directionsFile += Veteran.CemDirectionsPicLoc;
            try
            {
                Img_CemDirectionPhoto.Source = new BitmapImage(new Uri(directionsFile, UriKind.Absolute));
            }
            catch (FileNotFoundException)
            {
                // Don't load missing files.
            }
            catch (DirectoryNotFoundException)
            {
                // Don't load missing files.
            }

            cemeteryAirPicFile = ConfigurationManager.AppSettings["InstallDirectory"];
            cemeteryAirPicFile += ConfigurationManager.AppSettings["CemAirPicDirectory"];
            cemeteryAirPicFile += Veteran.CemAirPicLoc;
            try
            {
                Img_CemeteryAirPhoto.Source = new BitmapImage(new Uri(cemeteryAirPicFile, UriKind.Absolute));
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

        private void Img_CemDirectionPhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EnlargedPhoto EnlargedWin = new EnlargedPhoto(directionsFile);
            EnlargedWin.ShowDialog();
        }

        private void Img_CemeteryAirPhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EnlargedPhoto EnlargedWin = new EnlargedPhoto(cemeteryAirPicFile);
            EnlargedWin.ShowDialog();
        }

        private void Btn_CloseWin_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
