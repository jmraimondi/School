using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Museum_Admin
{
    /// <summary>
    /// Interaction logic for Ranks.xaml
    /// </summary>
    public partial class Ranks : UserControl, INotifyPropertyChanged
    {
        // Help with INotifyPropertyChanged
        // Source: https://stackoverflow.com/questions/14823119/updatesourcetrigger-not-working
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=netframework-4.8

        private List<RankDBInfo> rankList;
        public List<string> BranchList { get; }
        private string currentBranch;
        private RankDBInfo currentRank;
        public event PropertyChangedEventHandler PropertyChanged;

        public RankDBInfo CurrentRank
        {
            get
            {
                return currentRank;
            }
            set
            {
                currentRank = value;
                NotifyPropertyChanged();
            }
        }

        public string CurrentBranch
        {
            get
            {
                return currentBranch;
            }
            set
            {
                currentBranch = value;
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

        public Ranks()
        {
            InitializeComponent();

            BranchList = BranchDBInfo.LoadStringList();

            DataContext = this;
        }

        private void HideControls()
        {
            Lbl_RankName.Visibility = Visibility.Hidden;
            TxtBox_RankName.Visibility = Visibility.Hidden;
            Lbl_RankAbrev.Visibility = Visibility.Hidden;
            TxtBox_RankAbrev.Visibility = Visibility.Hidden;
            Btn_RankSave.Visibility = Visibility.Hidden;
            Btn_RankCancel.Visibility = Visibility.Hidden;
        }

        private void ShowControls()
        {
            Lbl_RankName.Visibility = Visibility.Visible;
            TxtBox_RankName.Visibility = Visibility.Visible;
            Lbl_RankAbrev.Visibility = Visibility.Visible;
            TxtBox_RankAbrev.Visibility = Visibility.Visible;
            Btn_RankSave.Visibility = Visibility.Visible;
            Btn_RankCancel.Visibility = Visibility.Visible;
        }

        private void Save()
        {
            CurrentRank.WriteDataToDatabase();

            rankList = RankDBInfo.LoadObjectList(CurrentBranch);

            ListBox_RankDetails.DataContext = rankList;

            HideControls();

            CurrentRank = null;
        }

        private void CmbBox_Service_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable buttons if actual branch selected, disable on blank branch item
            Btn_AddRank.IsEnabled = !string.IsNullOrEmpty(CurrentBranch);
            Btn_EditRank.IsEnabled = !string.IsNullOrEmpty(CurrentBranch);
            Btn_DeleteRank.IsEnabled = !string.IsNullOrEmpty(CurrentBranch);

            rankList = RankDBInfo.LoadObjectList(CurrentBranch);

            ListBox_RankDetails.DataContext = rankList;
        }

        private void Btn_AddRank_Click(object sender, RoutedEventArgs e)
        {
            CurrentRank = new RankDBInfo(CurrentBranch);

            ShowControls();
        }

        private void Btn_EditRank_Click(object sender, RoutedEventArgs e)
        {
            string selectedRank;

            selectedRank = Convert.ToString(ListBox_RankDetails.SelectedValue);

            // Verify the user selected a record to edit
            if (!string.IsNullOrEmpty(selectedRank))
            {
                CurrentRank = new RankDBInfo(CurrentBranch, selectedRank);

                ShowControls();
            }
            else
            {
                MessageBox.Show(Tools.RecordSelectMessage, Tools.RecordSelectTitle);
            }
        }


        private void Btn_DeleteRank_Click(object sender, RoutedEventArgs e)
        {
            string selectedRank;

            selectedRank = Convert.ToString(ListBox_RankDetails.SelectedValue);

            // Verify the user selected a record to delete
            if (!string.IsNullOrEmpty(selectedRank))
            {
                MessageBoxResult result;
                result = MessageBox.Show(Tools.deleteMessage, Tools.deleteTitle, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (RankDBInfo rank in rankList)
                    {
                        if (rank.Rank == selectedRank)
                        {
                            // Remove from the database
                            rank.DeleteFromDatabase();
                        }
                    }

                    rankList = RankDBInfo.LoadObjectList(CurrentBranch);

                    ListBox_RankDetails.DataContext = rankList;
                }
            }
            else
            {
                MessageBox.Show(Tools.RecordDeleteMessage, Tools.RecordSelectTitle);
            }
        }

        private void Btn_RankSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Btn_RankCancel_Click(object sender, RoutedEventArgs e)
        {
            HideControls();

            CurrentRank = null;
        }

        private void ListBox_RankDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideControls();
        }
    }
}
