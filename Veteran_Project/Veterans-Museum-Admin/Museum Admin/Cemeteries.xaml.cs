using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Configuration;
using Microsoft.Win32;

namespace Museum_Admin
{
    /// <summary>
    /// Interaction logic for Cemeteries.xaml
    /// </summary>
    public partial class Cemeteries : UserControl, INotifyPropertyChanged
    {
        // Help with INotifyPropertyChanged
        // Source: https://stackoverflow.com/questions/14823119/updatesourcetrigger-not-working
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=netframework-4.8

        private List<CemeteryDBInfo> cemList;
        private CemeteryDBInfo currentCemetery;
        private string dirPicFile;
        private string aerialPicFile;
        public event PropertyChangedEventHandler PropertyChanged;

        public CemeteryDBInfo CurrentCemetery
        {
            get
            {
                return currentCemetery;
            }
            set
            {
                currentCemetery = value;
                Img_CemDirectionPhoto.Source = null;
                Img_CemeteryAerialPhoto.Source = null;
                LoadDirectionsPic();
                LoadAerialPic();
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public Cemeteries()
        {
            InitializeComponent();

            cemList = CemeteryDBInfo.LoadObjectList();

            ListBox_CemeteryDetails.DataContext = cemList;

            DataContext = this;
        }

        private void HideControls()
        {
            Lbl_CemName.Visibility = Visibility.Hidden;
            TxtBox_CemName.Visibility = Visibility.Hidden;
            Lbl_CemAddress.Visibility = Visibility.Hidden;
            TxtBox_CemAddress.Visibility = Visibility.Hidden;
            Lbl_CemCity.Visibility = Visibility.Hidden;
            TxtBox_CemCity.Visibility = Visibility.Hidden;
            Lbl_CemGPS.Visibility = Visibility.Hidden;
            TxtBox_CemGPS.Visibility = Visibility.Hidden;
            Lbl_DirPhoto.Visibility = Visibility.Hidden;
            Img_CemDirectionPhoto.Visibility = Visibility.Hidden;
            Btn_SetDirPhoto.Visibility = Visibility.Hidden;
            Btn_DeleteDirPhoto.Visibility = Visibility.Hidden;
            Lbl_AerialPhoto.Visibility = Visibility.Hidden;
            Img_CemeteryAerialPhoto.Visibility = Visibility.Hidden;
            Btn_SetAerialPhoto.Visibility = Visibility.Hidden;
            Btn_DeleteAerialPhoto.Visibility = Visibility.Hidden;
            Btn_CemSave.Visibility = Visibility.Hidden;
            Btn_CemCancel.Visibility = Visibility.Hidden;
        }

        private void ShowControls()
        {
            Lbl_CemName.Visibility = Visibility.Visible;
            TxtBox_CemName.Visibility = Visibility.Visible;
            Lbl_CemAddress.Visibility = Visibility.Visible;
            TxtBox_CemAddress.Visibility = Visibility.Visible;
            Lbl_CemCity.Visibility = Visibility.Visible;
            TxtBox_CemCity.Visibility = Visibility.Visible;
            Lbl_CemGPS.Visibility = Visibility.Visible;
            TxtBox_CemGPS.Visibility = Visibility.Visible;
            Lbl_DirPhoto.Visibility = Visibility.Visible;
            Img_CemDirectionPhoto.Visibility = Visibility.Visible;
            Btn_SetDirPhoto.Visibility = Visibility.Visible;
            Btn_DeleteDirPhoto.Visibility = Visibility.Visible;
            Lbl_AerialPhoto.Visibility = Visibility.Visible;
            Img_CemeteryAerialPhoto.Visibility = Visibility.Visible;
            Btn_SetAerialPhoto.Visibility = Visibility.Visible;
            Btn_DeleteAerialPhoto.Visibility = Visibility.Visible;
            Btn_CemSave.Visibility = Visibility.Visible;
            Btn_CemCancel.Visibility = Visibility.Visible;
        }

        private void Save()
        {
            CurrentCemetery.WriteDataToDatabase();

            cemList = CemeteryDBInfo.LoadObjectList();

            ListBox_CemeteryDetails.DataContext = cemList;

            HideControls();

            CurrentCemetery = null;
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

        private void LoadDirectionsPic()
        {
            dirPicFile = ConfigurationManager.AppSettings["InstallDirectory"];
            dirPicFile += ConfigurationManager.AppSettings["CemDirectionsDirectory"];

            if (currentCemetery != null)
            {
                dirPicFile += currentCemetery.DirPicLoc;
            }
            try
            {
                Img_CemDirectionPhoto.Source = Tools.LoadBitmap(dirPicFile);
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

        private void LoadAerialPic()
        {
            aerialPicFile = ConfigurationManager.AppSettings["InstallDirectory"];
            aerialPicFile += ConfigurationManager.AppSettings["CemAirPicDirectory"];

            if (currentCemetery != null)
            {
                aerialPicFile += currentCemetery.AirPicLoc;
            }
            try
            {
                Img_CemeteryAerialPhoto.Source = Tools.LoadBitmap(aerialPicFile);
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

        private void Btn_AddCemetery_Click(object sender, RoutedEventArgs e)
        {
            CurrentCemetery = new CemeteryDBInfo();

            ShowControls();
        }

        private void Btn_EditCemetery_Click(object sender, RoutedEventArgs e)
        {
            string selected;

            selected = Convert.ToString(ListBox_CemeteryDetails.SelectedValue);

            // Verify the user selected a record to edit
            if (!string.IsNullOrEmpty(selected))
            {
                CurrentCemetery = new CemeteryDBInfo(selected);

                ShowControls();
            }
            else
            {
                MessageBox.Show(Tools.RecordSelectMessage, Tools.RecordSelectTitle);
            }
        }

        private void Btn_DeleteCemetery_Click(object sender, RoutedEventArgs e)
        {
            string selected;

            selected = Convert.ToString(ListBox_CemeteryDetails.SelectedValue);

            // Verify the user selected a record to delete
            if (!string.IsNullOrEmpty(selected))
            {
                MessageBoxResult result;
                result = MessageBox.Show(Tools.deleteMessage, Tools.deleteTitle, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (CemeteryDBInfo cemetery in cemList)
                    {
                        if (cemetery.Details == selected)
                        {
                            // Remove from the database
                            cemetery.DeleteFromDatabase();
                        }
                    }

                    cemList = CemeteryDBInfo.LoadObjectList();

                    ListBox_CemeteryDetails.DataContext = cemList;
                }
            }
            else
            {
                MessageBox.Show(Tools.RecordDeleteMessage, Tools.RecordSelectTitle);
            }
        }

        private void Img_CemDirectionPhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EnlargedPhoto EnlargedWin = new EnlargedPhoto(dirPicFile);
            EnlargedWin.ShowDialog();
        }

        private void Btn_SetDirPhoto_Click(object sender, RoutedEventArgs e)
        {
            string newPic;
            newPic = SetPhoto(Img_CemDirectionPhoto);

            if (!string.IsNullOrEmpty(newPic))
            {
                CurrentCemetery.DirPicLoc = newPic;
            }
        }

        private void Btn_DeleteDirPhoto_Click(object sender, RoutedEventArgs e)
        {
            Img_CemDirectionPhoto.Source = null;
            CurrentCemetery.DirPicLoc = "";
        }

        private void Img_CemeteryAirPhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EnlargedPhoto EnlargedWin = new EnlargedPhoto(aerialPicFile);
            EnlargedWin.ShowDialog();
        }

        private void Btn_SetAerialPhoto_Click(object sender, RoutedEventArgs e)
        {
            string newPic;
            newPic = SetPhoto(Img_CemeteryAerialPhoto);

            if (!string.IsNullOrEmpty(newPic))
            {
                CurrentCemetery.AirPicLoc = newPic;
            }
        }

        private void Btn_DeleteAerialPhoto_Click(object sender, RoutedEventArgs e)
        {
            Img_CemeteryAerialPhoto.Source = null;
            CurrentCemetery.AirPicLoc = "";
        }

        private void Btn_CemSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Btn_CemCancel_Click(object sender, RoutedEventArgs e)
        {
            HideControls();

            CurrentCemetery = null;
        }

        private void ListBox_CemeteryDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideControls();

            CurrentCemetery = null;
        }
    }
}
