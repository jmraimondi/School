using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;
using System.IO;
using System.Configuration;

namespace Musuem_Viewer
{
    /// <summary>
    /// Interaction logic for VeteranInfo.xaml
    /// </summary>
    public partial class VeteranInfo : Window
    {
        public VeteranDBInfo Veteran { get; set; }
        private List<int> recordIds;
        private int currentId;

        // File paths for photos - Used for EnlargedPhoto
        private string miscPicFile = "";
        private string milPicFile = "";
        private string casualPicFile = "";
        private string markerPicFile = "";

        private string branchName = "";

        public VeteranInfo()
        {
            InitializeComponent();
        }

        public void BuildAndShowDialog(int recordId)
        {
            LoadAllData(recordId);

            WindowState = WindowState.Maximized;

            ShowDialog();
        }

        private void LoadAllData(int recordId)
        {
            // If recordIds == null, it's a single record, the buttons are hidden
            if (recordIds != null)
            {
                if (recordIds.IndexOf(recordId) == 0)
                {
                    Btn_Previous.IsEnabled = false;
                    Btn_Next.IsEnabled = true;
                }
                else if (recordIds.IndexOf(recordId) == (recordIds.Count - 1))
                {
                    Btn_Previous.IsEnabled = true;
                    Btn_Next.IsEnabled = false;
                }
                else
                {
                    Btn_Previous.IsEnabled = true;
                    Btn_Next.IsEnabled = true;
                }
            }

            DataContext = null;

            currentId = recordId;
            Veteran = new VeteranDBInfo(currentId);

            DataContext = this;

            ClearFields();

            BuildServiceDetails();
            LoadVeteranPictures();
            BuildConflictsList();
            BuildAwardsList();
        }

        private void Btn_SearchAgain_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Blank boxes to be filled later in the code
        private void ClearFields()
        {
            TxtBlk_ServiceDetails.Text = "";
            TxtBlk_Conflicts.Text = "";
            TxtBlk_Awards.Text = "";

            Img_ServicePhoto.Source = null;
            Img_CasualPhoto.Source = null;
            Img_MarkerPhoto.Source = null;
            Img_MiscPhoto.Source = null;

            miscPicFile = "";
            milPicFile = "";
            casualPicFile = "";
            markerPicFile = "";
            branchName = "";
        }

        // Build up Service Details in the form Branch - Rank - Unit / Ship
        private void BuildServiceDetails()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT Branch,SRank,UnitShip" +
                            " FROM Veterans NATURAL JOIN Services WHERE ID=@idNum";
                        command.Parameters.Add("@idNum", MySqlDbType.Int32).Value = currentId;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            string serviceDetails;

                            while (reader.Read())
                            {
                                serviceDetails = "";

                                if (!reader.IsDBNull(0))
                                {
                                    serviceDetails += reader.GetString(0);

                                    // Only grab the first branch name from the results
                                    if (branchName == "")
                                    {
                                        branchName = reader.GetString(0);
                                    }
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    serviceDetails += " - ";
                                    serviceDetails += reader.GetString(1);
                                }

                                if (!reader.IsDBNull(2))
                                {
                                    serviceDetails += " - ";
                                    serviceDetails += reader.GetString(2);
                                }

                                TxtBlk_ServiceDetails.Text += serviceDetails;
                                TxtBlk_ServiceDetails.Text += Environment.NewLine;
                            }
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(Tools.DBErrorMessage, Tools.DBErrorTitle);
            }
            catch (MySqlException e)
            {
                Tools.HandleSQLExceptions(e);
            }
        }

        private void BuildConflictsList()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT ConflictName" +
                            " FROM Veterans NATURAL JOIN Conflicts WHERE ID=@idNum";
                        command.Parameters.Add("@idNum", MySqlDbType.Int32).Value = currentId;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    TxtBlk_Conflicts.Text += reader.GetString(0);
                                    TxtBlk_Conflicts.Text += Environment.NewLine;
                                }
                            }
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(Tools.DBErrorMessage, Tools.DBErrorTitle);
            }
            catch (MySqlException e)
            {
                Tools.HandleSQLExceptions(e);
            }
        }

        private void BuildAwardsList()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT AwardName" +
                            " FROM Veterans NATURAL JOIN Awards WHERE ID=@idNum";
                        command.Parameters.Add("@idNum", MySqlDbType.Int32).Value = currentId;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    TxtBlk_Awards.Text += reader.GetString(0);
                                    TxtBlk_Awards.Text += Environment.NewLine;
                                }
                            }
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(Tools.DBErrorMessage, Tools.DBErrorTitle);
            }
            catch (MySqlException e)
            {
                Tools.HandleSQLExceptions(e);
            }
        }

        // Loads the veterans pictures to the UI
        private void LoadVeteranPictures()
        {
            bool isMilPicLoaded = false;

            miscPicFile = ConfigurationManager.AppSettings["InstallDirectory"];
            miscPicFile += ConfigurationManager.AppSettings["MiscPicDirectory"];
            miscPicFile += Veteran.MiscPicLoc;
            try
            {
                Img_MiscPhoto.Source = Tools.LoadBitmap(miscPicFile);
            }
            catch (FileNotFoundException)
            {
                // Don't load missing files.
            }
            catch (DirectoryNotFoundException)
            {
                // Don't load missing files.
            }

            milPicFile = ConfigurationManager.AppSettings["InstallDirectory"];
            milPicFile += ConfigurationManager.AppSettings["MilPicDirectory"];
            milPicFile += Veteran.MilPicLoc;
            try
            {
                Img_ServicePhoto.Source = Tools.LoadBitmap(milPicFile);
                isMilPicLoaded = true;
            }
            catch (FileNotFoundException)
            {
                // Don't load missing files.
            }
            catch (DirectoryNotFoundException)
            {
                // Don't load missing files.
            }

            casualPicFile = ConfigurationManager.AppSettings["InstallDirectory"];
            casualPicFile += ConfigurationManager.AppSettings["CasualPicDirectory"];
            casualPicFile += Veteran.CasualPicLoc;
            try
            {
                Img_CasualPhoto.Source = Tools.LoadBitmap(casualPicFile);
            }
            catch (FileNotFoundException)
            {
                // Don't load missing files.
            }
            catch (DirectoryNotFoundException)
            {
                // Don't load missing files.
            }

            markerPicFile = ConfigurationManager.AppSettings["InstallDirectory"];
            markerPicFile += ConfigurationManager.AppSettings["MarkerPicDirectory"];
            markerPicFile += Veteran.MarkerPicLoc;
            try
            {
                Img_MarkerPhoto.Source = Tools.LoadBitmap(markerPicFile);
            }
            catch (FileNotFoundException)
            {
                // Don't load missing files.
            }
            catch (DirectoryNotFoundException)
            {
                // Don't load missing files.
            }


            if (!isMilPicLoaded)
            {
                LoadBranchPic();
            }
        }

        private void LoadBranchPic()
        {
            string branchPicFile;
            string fileName = "";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT BranchPicLoc FROM BranchesList WHERE BranchName=@bName";
                        command.Parameters.Add("@bName", MySqlDbType.VarChar).Value = branchName;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    fileName = reader.GetString(0);
                                }
                            }
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(Tools.DBErrorMessage, Tools.DBErrorTitle);
            }
            catch (MySqlException e)
            {
                Tools.HandleSQLExceptions(e);
            }

            branchPicFile = ConfigurationManager.AppSettings["InstallDirectory"];
            branchPicFile += ConfigurationManager.AppSettings["BranchPicDirectory"];
            branchPicFile += fileName;

            try
            {
                Img_ServicePhoto.Source = new BitmapImage(new Uri(branchPicFile, UriKind.Absolute));
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

        // Sets the record viewer up for multi record browsing.
        // Pass this a list of IDs to browse before creating window.
        public void SetMultiRecord(List<int> matchIds)
        {
            recordIds = matchIds;
            Btn_Next.Visibility = Visibility.Visible;
            Btn_Previous.Visibility = Visibility.Visible;
        }

        public void SetSingleRecord()
        {
            recordIds = null;
            Btn_Next.Visibility = Visibility.Hidden;
            Btn_Previous.Visibility = Visibility.Hidden;
        }

        private void Img_ServicePhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EnlargedPhoto EnlargedWin = new EnlargedPhoto(milPicFile);
            EnlargedWin.ShowDialog();
        }

        private void Img_MiscPhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EnlargedPhoto EnlargedWin = new EnlargedPhoto(casualPicFile);
            EnlargedWin.ShowDialog();
        }

        private void Img_MarkerPhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EnlargedPhoto EnlargedWin = new EnlargedPhoto(markerPicFile);
            EnlargedWin.ShowDialog();
        }

        private void Img_MiscPhoto_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            EnlargedPhoto EnlargedWin = new EnlargedPhoto(miscPicFile);
            EnlargedWin.ShowDialog();
        }

        private void Btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = recordIds.IndexOf(currentId);

            if (currentIndex > 0)
            {
                LoadAllData(recordIds[currentIndex - 1]);
            }
        }

        private void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = recordIds.IndexOf(currentId);

            if (currentIndex < (recordIds.Count - 1))
            {
                LoadAllData(recordIds[currentIndex + 1]);
            }
        }

        private void Btn_CemeteryInfo_Click(object sender, RoutedEventArgs e)
        {
            CemeteryInformation cemeteryWin = new CemeteryInformation(Veteran);
            cemeteryWin.ShowDialog();
        }

        private void Btn_AddComments_Click(object sender, RoutedEventArgs e)
        {
            UserComments commentWin = new UserComments(currentId);
            commentWin.ShowDialog();
        }
    }
}
