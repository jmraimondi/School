using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Configuration;
using System.IO;
using Microsoft.Win32;

namespace Museum_Admin
{
    /// <summary>
    /// Interaction logic for CommentRecord.xaml
    /// </summary>
    public partial class CommentRecord : UserControl
    {
        public VeteranDBInfo Veteran { get; set; }
        public VetCommentRecordDBInfo VetComment { get; set; }
        public List<string> CemList { get; }
        private List<int> recordIds;
        private int currentCnum;
        MainWindow mainWin;

        // File paths for photos - Used for EnlargedPhoto
        private string milPicFile = "";
        private string casualPicFile = "";
        private string markerPicFile = "";
        private string miscPicFile = "";

        public CommentRecord(MainWindow main)
        {
            InitializeComponent();

            mainWin = main;

            DataContext = this;

            Tools.hasDataChanged = false;

            CemList = CemeteryDBInfo.LoadStringList();
        }

        // Sets dialog settings and loads data. Used if record already exists.
        public void BuildAndShowDialog(int recordId)
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

            currentCnum = recordId;
            VetComment = new VetCommentRecordDBInfo(currentCnum);

            int currentId = VetComment.ID;
            Veteran = new VeteranDBInfo(currentId);

            // Set the Listboxes to the Lists in Veteran
            ListBox_ServiceDetails.DataContext = Veteran.ServiceDetails;
            ListBox_AwardDetails.DataContext = Veteran.AwardDetails;
            ListBox_ConflictDetails.DataContext = Veteran.ConflictDetails;

            LoadMilPic();
            LoadCasualPic();
            LoadMarkerPic();
            LoadMiscPic();
        }

        private void LoadMiscPic()
        {
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
        }

        private void LoadMilPic()
        {
            milPicFile = ConfigurationManager.AppSettings["InstallDirectory"];
            milPicFile += ConfigurationManager.AppSettings["MilPicDirectory"];
            milPicFile += Veteran.MilPicLoc;
            try
            {
                Img_ServicePhoto.Source = Tools.LoadBitmap(milPicFile);
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

        private void LoadCasualPic()
        {
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
        }

        private void LoadMarkerPic()
        {
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
        }

        // Sets the record viewer up for multi record browsing.
        // Pass this a list of IDs to browse before creating window.
        public void SetMultiRecord(List<int> matchIds)
        {
            recordIds = matchIds;
            Btn_Next.Visibility = Visibility.Visible;
            Btn_Previous.Visibility = Visibility.Visible;
        }

        // Sets the record viewer up for single record browsing.
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

        private void Img_CasualPhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EnlargedPhoto EnlargedWin = new EnlargedPhoto(casualPicFile);
            EnlargedWin.ShowDialog();
        }

        private void Img_MarkerPhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EnlargedPhoto EnlargedWin = new EnlargedPhoto(markerPicFile);
            EnlargedWin.ShowDialog();
        }

        private void Img_MiscPhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EnlargedPhoto EnlargedWin = new EnlargedPhoto(miscPicFile);
            EnlargedWin.ShowDialog();
        }

        private void Btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = recordIds.IndexOf(currentCnum);

            if (currentIndex > 0)
            {
                MessageBoxResult result = MessageBoxResult.No;

                if (Tools.hasDataChanged)
                {
                    result = MessageBox.Show(Tools.unsavedMessage, Tools.unsavedTitle, MessageBoxButton.YesNo);
                }

                if (!Tools.hasDataChanged || result == MessageBoxResult.Yes)
                {
                    CommentRecord vetWin;
                    vetWin = new CommentRecord(mainWin);
                    vetWin.SetMultiRecord(recordIds);
                    vetWin.BuildAndShowDialog(recordIds[currentIndex - 1]);

                    mainWin.DataContext = null;
                    mainWin.MainWindowContent = vetWin;
                    mainWin.DataContext = mainWin;

                    // User has changed the page, discarding the changed data
                    Tools.hasDataChanged = false;
                }
            }
        }

        private void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = recordIds.IndexOf(currentCnum);

            if (currentIndex < (recordIds.Count - 1))
            {
                MessageBoxResult result = MessageBoxResult.No;

                if (Tools.hasDataChanged)
                {
                    result = MessageBox.Show(Tools.unsavedMessage, Tools.unsavedTitle, MessageBoxButton.YesNo);
                }

                if (!Tools.hasDataChanged || result == MessageBoxResult.Yes)
                {
                    CommentRecord vetWin;
                    vetWin = new CommentRecord(mainWin);
                    vetWin.SetMultiRecord(recordIds);
                    vetWin.BuildAndShowDialog(recordIds[currentIndex + 1]);

                    mainWin.DataContext = null;
                    mainWin.MainWindowContent = vetWin;
                    mainWin.DataContext = mainWin;

                    // User has changed the page, discarding the changed data
                    Tools.hasDataChanged = false;
                }
            }
        }

        private void Btn_SaveRecord_Click(object sender, RoutedEventArgs e)
        {
            Veteran.WriteDataToDatabase();
        }

        private void Btn_DeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result;
            result = MessageBox.Show(Tools.deleteMessage, Tools.deleteTitle, MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                Veteran.DeleteFromDatabase();

                mainWin.DataContext = null;
                mainWin.MainWindowContent = new Veterans(mainWin);
                mainWin.DataContext = mainWin;

                // User has deleted the record, discarding the changed data
                Tools.hasDataChanged = false;
            }
        }

        private void Btn_AddService_Click(object sender, RoutedEventArgs e)
        {
            ServiceDetails serviceWin = new ServiceDetails();

            // If we know the veteran ID, set it
            if (Veteran.Id != 0)
            {
                serviceWin.SetId(Veteran.Id);
            }

            serviceWin.ShowDialog();

            if (serviceWin.IsOk)
            {
                Veteran.ServiceDetails.Add(serviceWin.ServiceInfo);
            }

            ListBox_ServiceDetails.Items.Refresh();
        }

        private void Btn_EditService_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex;
            VetServiceDBInfo selectedItem = null;

            // Get the index from the UI
            selectedIndex = ListBox_ServiceDetails.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < Veteran.ServiceDetails.Count)
            {

                selectedItem = Veteran.ServiceDetails[selectedIndex];

                ServiceDetails serviceWin = new ServiceDetails(selectedItem);

                serviceWin.ShowDialog();

                Veteran.ServiceDetails[selectedIndex] = serviceWin.ServiceInfo;

                ListBox_ServiceDetails.Items.Refresh();
            }
            else
            {
                MessageBox.Show(Tools.RecordSelectMessage, Tools.RecordSelectTitle);
            }
        }

        private void Btn_DeleteService_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex;
            VetServiceDBInfo removeItem = null;

            // Get the index from the UI
            selectedIndex = ListBox_ServiceDetails.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < Veteran.ServiceDetails.Count)
            {

                MessageBoxResult result;
                result = MessageBox.Show(Tools.deleteMessage, Tools.deleteTitle, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    // Get the index from the UI
                    selectedIndex = ListBox_ServiceDetails.SelectedIndex;

                    removeItem = Veteran.ServiceDetails[selectedIndex];

                    // Remove from the database
                    removeItem.DeleteFromDatabase();

                    // Remove from the list
                    Veteran.ServiceDetails.Remove(removeItem);

                    ListBox_ServiceDetails.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show(Tools.RecordDeleteMessage, Tools.RecordSelectTitle);
            }
        }

        private void Btn_AddConflict_Click(object sender, RoutedEventArgs e)
        {
            ConflictDetails conflictWin = new ConflictDetails();

            // If we know the veteran ID, set it
            if (Veteran.Id != 0)
            {
                conflictWin.SetId(Veteran.Id);
            }

            conflictWin.ShowDialog();

            if (conflictWin.IsOk)
            {
                Veteran.ConflictDetails.Add(conflictWin.ConflictInfo);
            }

            ListBox_ConflictDetails.Items.Refresh();
        }

        private void Btn_EditConflict_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex;
            VetConflictDBInfo selectedItem = null;

            // Get the index from the UI
            selectedIndex = ListBox_ConflictDetails.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < Veteran.ConflictDetails.Count)
            {
                selectedItem = Veteran.ConflictDetails[selectedIndex];

                ConflictDetails conflictWin = new ConflictDetails(selectedItem);

                conflictWin.ShowDialog();

                Veteran.ConflictDetails[selectedIndex] = conflictWin.ConflictInfo;

                ListBox_ConflictDetails.Items.Refresh();
            }
            else
            {
                MessageBox.Show(Tools.RecordSelectMessage, Tools.RecordSelectTitle);
            }
        }

        private void Btn_DeleteConflict_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex;
            VetConflictDBInfo removeItem = null;

            // Get the index from the UI
            selectedIndex = ListBox_ConflictDetails.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < Veteran.ConflictDetails.Count)
            {
                MessageBoxResult result;
                result = MessageBox.Show(Tools.deleteMessage, Tools.deleteTitle, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    removeItem = Veteran.ConflictDetails[selectedIndex];

                    // Remove from the database
                    removeItem.DeleteFromDatabase();

                    // Remove from the list
                    Veteran.ConflictDetails.Remove(removeItem);

                    ListBox_ConflictDetails.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show(Tools.RecordDeleteMessage, Tools.RecordSelectTitle);
            }
        }

        private void Btn_AddAward_Click(object sender, RoutedEventArgs e)
        {
            AwardDetails awardWin = new AwardDetails();

            // If we know the veteran ID, set it
            if (Veteran.Id != 0)
            {
                awardWin.SetId(Veteran.Id);
            }

            awardWin.ShowDialog();

            if (awardWin.IsOk)
            {
                Veteran.AwardDetails.Add(awardWin.AwardInfo);
            }

            ListBox_AwardDetails.Items.Refresh();
        }

        private void Btn_EditAward_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex;
            VetAwardDBInfo selectedItem = null;

            // Get the index from the UI
            selectedIndex = ListBox_AwardDetails.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < Veteran.AwardDetails.Count)
            {
                selectedItem = Veteran.AwardDetails[selectedIndex];

                AwardDetails awardWin = new AwardDetails(selectedItem);

                awardWin.ShowDialog();

                Veteran.AwardDetails[selectedIndex] = awardWin.AwardInfo;

                ListBox_AwardDetails.Items.Refresh();
            }
            else
            {
                MessageBox.Show(Tools.RecordSelectMessage, Tools.RecordSelectTitle);
            }
        }

        private void Btn_DeleteAward_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex;
            VetAwardDBInfo removeItem = null;

            // Get the index from the UI
            selectedIndex = ListBox_AwardDetails.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < Veteran.AwardDetails.Count)
            {
                MessageBoxResult result;
                result = MessageBox.Show(Tools.deleteMessage, Tools.deleteTitle, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    removeItem = Veteran.AwardDetails[selectedIndex];

                    // Remove from the database
                    removeItem.DeleteFromDatabase();

                    // Remove from the list
                    Veteran.AwardDetails.Remove(removeItem);

                    ListBox_AwardDetails.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show(Tools.RecordDeleteMessage, Tools.RecordSelectTitle);
            }
        }

        // Opens a file dialog to select a new photo.
        // Returns the photo path if a photo is selected. Returns a blank string otherwise.
        private string SetPhoto(Image viewImage)
        {
            string newPicPath = "";
            bool? result;       // Nullable boolean
            bool isPicLoaded = false;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Images Files (*.png;*.jpeg;*.gif;*.jpg;*.bmp;*.tiff;*.tif)" +
                "|*.png;*.jpeg;*.gif;*.jpg;*.bmp;*.tiff;*.tif" +
                "|All files (*.*)|*.*";

            result = dlg.ShowDialog();

            if (result == true)
            {
                newPicPath = dlg.FileName;

                try
                {
                    viewImage.Source = Tools.LoadBitmap(newPicPath);
                    isPicLoaded = true;
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show(Tools.fileMissingMessage, Tools.fileMissingTitle);
                    isPicLoaded = false;
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show(Tools.directoryMissingMessage, Tools.directoryMissingTitle);
                    isPicLoaded = false;
                }
            }

            if (!isPicLoaded)
            {
                newPicPath = "";
            }

            return newPicPath;
        }

        private void Btn_SetMilPhoto_Click(object sender, RoutedEventArgs e)
        {
            string newPic;
            newPic = SetPhoto(Img_ServicePhoto);

            if (!string.IsNullOrEmpty(newPic))
            {
                Veteran.MilPicLoc = newPic;
            }
        }

        private void Btn_SetCasualPhoto_Click(object sender, RoutedEventArgs e)
        {
            string newPic;
            newPic = SetPhoto(Img_CasualPhoto);

            if (!string.IsNullOrEmpty(newPic))
            {
                Veteran.CasualPicLoc = newPic;
            }
        }

        private void Btn_SetMarkerPhoto_Click(object sender, RoutedEventArgs e)
        {
            string newPic;
            newPic = SetPhoto(Img_MarkerPhoto);

            if (!string.IsNullOrEmpty(newPic))
            {
                Veteran.MarkerPicLoc = newPic;
            }
        }

        private void Btn_SetMiscPhoto_Click(object sender, RoutedEventArgs e)
        {
            string newPic;
            newPic = SetPhoto(Img_MiscPhoto);

            if (!string.IsNullOrEmpty(newPic))
            {
                Veteran.MiscPicLoc = newPic;
            }
        }

        private void Btn_DeleteMilPhoto_Click(object sender, RoutedEventArgs e)
        {
            Img_ServicePhoto.Source = null;
            Veteran.MilPicLoc = "";
        }

        private void Btn_DeleteCasualPhoto_Click(object sender, RoutedEventArgs e)
        {
            Img_CasualPhoto.Source = null;
            Veteran.CasualPicLoc = "";
        }

        private void Btn_DeleteMarkerPhoto_Click(object sender, RoutedEventArgs e)
        {
            Img_MarkerPhoto.Source = null;
            Veteran.MarkerPicLoc = "";
        }

        private void Btn_DeleteMiscPhoto_Click(object sender, RoutedEventArgs e)
        {
            Img_MiscPhoto.Source = null;
            Veteran.MiscPicLoc = "";
        }

        private void Btn_DeleteComment_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result;
            result = MessageBox.Show(Tools.deleteCommentMessage, Tools.deleteTitle, MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                VetComment.DeleteFromDatabase();
            }
        }
    }
}
