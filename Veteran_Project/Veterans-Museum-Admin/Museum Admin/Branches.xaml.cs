using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Museum_Admin
{
    /// <summary>
    /// Interaction logic for Branches.xaml
    /// </summary>
    public partial class Branches : UserControl, INotifyPropertyChanged
    {
        // Help with INotifyPropertyChanged
        // Source: https://stackoverflow.com/questions/14823119/updatesourcetrigger-not-working
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=netframework-4.8

        private List<BranchDBInfo> branchList;
        private BranchDBInfo currentBranch;
        private string logoPicFile;
        public event PropertyChangedEventHandler PropertyChanged;

        public BranchDBInfo CurrentBranch
        {
            get
            {
                return currentBranch;
            }
            set
            {
                currentBranch = value;
                Img_BranchLogo.Source = null;
                LoadLogoPic();
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

        public Branches()
        {
            InitializeComponent();

            branchList = BranchDBInfo.LoadObjectList();

            ListBox_BranchDetails.DataContext = branchList;

            DataContext = this;
        }

        private void HideControls()
        {
            Lbl_Branches.Visibility = Visibility.Hidden;
            TxtBox_Branches.Visibility = Visibility.Hidden;
            Btn_BranchSave.Visibility = Visibility.Hidden;
            Btn_BranchCancel.Visibility = Visibility.Hidden;
            Img_BranchLogo.Visibility = Visibility.Hidden;
            Btn_SetLogo.Visibility = Visibility.Hidden;
            Btn_DeleteLogo.Visibility = Visibility.Hidden;
        }

        private void ShowControls()
        {
            Lbl_Branches.Visibility = Visibility.Visible;
            TxtBox_Branches.Visibility = Visibility.Visible;
            Btn_BranchSave.Visibility = Visibility.Visible;
            Btn_BranchCancel.Visibility = Visibility.Visible;
            Img_BranchLogo.Visibility = Visibility.Visible;
            Btn_SetLogo.Visibility = Visibility.Visible;
            Btn_DeleteLogo.Visibility = Visibility.Visible;
        }

        private void Save()
        {
            CurrentBranch.WriteDataToDatabase();

            branchList = BranchDBInfo.LoadObjectList();

            ListBox_BranchDetails.DataContext = branchList;

            HideControls();

            CurrentBranch = null;
        }

        private void LoadLogoPic()
        {
            logoPicFile = ConfigurationManager.AppSettings["InstallDirectory"];
            logoPicFile += ConfigurationManager.AppSettings["BranchPicDirectory"];

            if (CurrentBranch != null)
            {
                logoPicFile += CurrentBranch.BranchPicLoc;
            }

            try
            {
                Img_BranchLogo.Source = Tools.LoadBitmap(logoPicFile);
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

        private void Btn_AddBranch_Click(object sender, RoutedEventArgs e)
        {
            CurrentBranch = new BranchDBInfo();

            ShowControls();
        }

        private void Btn_EditBranch_Click(object sender, RoutedEventArgs e)
        {
            string selected;

            selected = Convert.ToString(ListBox_BranchDetails.SelectedValue);

            // Verify the user selected a record to edit
            if (!string.IsNullOrEmpty(selected))
            {
                CurrentBranch = new BranchDBInfo(selected);

                ShowControls();
            }
            else
            {
                MessageBox.Show(Tools.RecordSelectMessage, Tools.RecordSelectTitle);
            }
        }

        private void Btn_DeleteBranch_Click(object sender, RoutedEventArgs e)
        {
            string selected;

            selected = Convert.ToString(ListBox_BranchDetails.SelectedValue);

            // Verify the user selected a record to delete
            if (!string.IsNullOrEmpty(selected))
            {
                MessageBoxResult result;
                result = MessageBox.Show(Tools.deleteMessage, Tools.deleteTitle, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (BranchDBInfo branch in branchList)
                    {
                        if (branch.Branch == selected)
                        {
                            // Remove from the database
                            branch.DeleteFromDatabase();
                        }
                    }

                    branchList = BranchDBInfo.LoadObjectList();

                    ListBox_BranchDetails.DataContext = branchList;
                }
            }
            else
            {
                MessageBox.Show(Tools.RecordDeleteMessage, Tools.RecordSelectTitle);
            }
        }

        private void Btn_BranchSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Btn_BranchCancel_Click(object sender, RoutedEventArgs e)
        {
            HideControls();

            CurrentBranch = null;
        }

        private void ListBox_BranchDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideControls();
        }

        private void Btn_SetLogo_Click(object sender, RoutedEventArgs e)
        {
            string newLogoPath = "";
            bool? result;       // Nullable boolean
            bool isPicLoaded = false;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Images Files (*.png;*.jpeg;*.gif;*.jpg;*.bmp;*.tiff;*.tif)" +
                "|*.png;*.jpeg;*.gif;*.jpg;*.bmp;*.tiff;*.tif" +
                "|All files (*.*)|*.*";

            result = dlg.ShowDialog();

            if (result == true)
            {
                newLogoPath = dlg.FileName;

                try
                {
                    Img_BranchLogo.Source = Tools.LoadBitmap(newLogoPath);
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

            if (isPicLoaded)
            {
                CurrentBranch.BranchPicLoc = newLogoPath;
            }
        }

        private void Btn_DeleteLogo_Click(object sender, RoutedEventArgs e)
        {
            Img_BranchLogo.Source = null;
            CurrentBranch.BranchPicLoc = "";
        }

        private void Img_BranchLogo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EnlargedPhoto EnlargedWin = new EnlargedPhoto(logoPicFile);
            EnlargedWin.ShowDialog();
        }
    }
}
