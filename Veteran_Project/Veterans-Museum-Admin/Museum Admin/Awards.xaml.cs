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
    /// Interaction logic for Awards.xaml
    /// </summary>
    public partial class Awards : UserControl, INotifyPropertyChanged
    {
        // Help with INotifyPropertyChanged
        // Source: https://stackoverflow.com/questions/14823119/updatesourcetrigger-not-working
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=netframework-4.8

        private List<AwardDBInfo> awardList;
        public List<string> BranchList { get; }
        private string currentBranch;
        private AwardDBInfo currentAward;
        public event PropertyChangedEventHandler PropertyChanged;

        public AwardDBInfo CurrentAward
        {
            get
            {
                return currentAward;
            }
            set
            {
                currentAward = value;
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

        public Awards()
        {
            InitializeComponent();

            BranchList = BranchDBInfo.LoadStringList();

            DataContext = this;
        }

        private void HideControls()
        {
            Lbl_Award.Visibility = Visibility.Hidden;
            TxtBox_Award.Visibility = Visibility.Hidden;
            Btn_AwardSave.Visibility = Visibility.Hidden;
            Btn_AwardCancel.Visibility = Visibility.Hidden;
        }

        private void ShowControls()
        {
            Lbl_Award.Visibility = Visibility.Visible;
            TxtBox_Award.Visibility = Visibility.Visible;
            Btn_AwardSave.Visibility = Visibility.Visible;
            Btn_AwardCancel.Visibility = Visibility.Visible;
        }

        private void Save()
        {
            CurrentAward.WriteDataToDatabase();

            awardList = AwardDBInfo.LoadObjectList(CurrentBranch);

            ListBox_AwardDetails.DataContext = awardList;

            HideControls();

            CurrentAward = null;
        }

        private void CmbBox_Service_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable buttons if actual branch selected, disable on blank branch item
            Btn_AddAward.IsEnabled = !string.IsNullOrEmpty(CurrentBranch);
            Btn_EditAward.IsEnabled = !string.IsNullOrEmpty(CurrentBranch);
            Btn_DeleteAward.IsEnabled = !string.IsNullOrEmpty(CurrentBranch);

            awardList = AwardDBInfo.LoadObjectList(CurrentBranch);

            ListBox_AwardDetails.DataContext = awardList;
        }

        private void Btn_AddAward_Click(object sender, RoutedEventArgs e)
        {
            CurrentAward = new AwardDBInfo(currentBranch);

            ShowControls();
        }

        private void Btn_EditAward_Click(object sender, RoutedEventArgs e)
        {
            string selectedAward;

            selectedAward = Convert.ToString(ListBox_AwardDetails.SelectedValue);

            // Verify the user selected a record to edit
            if (!string.IsNullOrEmpty(selectedAward))
            {
                CurrentAward = new AwardDBInfo(CurrentBranch, selectedAward);

                ShowControls();
            }
            else
            {
                MessageBox.Show(Tools.RecordSelectMessage, Tools.RecordSelectTitle);
            }
        }

        private void Btn_DeleteAward_Click(object sender, RoutedEventArgs e)
        {
            string selectedAward;

            selectedAward = Convert.ToString(ListBox_AwardDetails.SelectedValue);

            // Verify the user selected a record to delete
            if (!string.IsNullOrEmpty(selectedAward))
            {
                MessageBoxResult result;
                result = MessageBox.Show(Tools.deleteMessage, Tools.deleteTitle, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (AwardDBInfo award in awardList)
                    {
                        if (award.Award == selectedAward)
                        {
                            // Remove from the database
                            award.DeleteFromDatabase();
                        }
                    }

                    awardList = AwardDBInfo.LoadObjectList(CurrentBranch);

                    ListBox_AwardDetails.DataContext = awardList;
                }
            }
            else
            {
                MessageBox.Show(Tools.RecordDeleteMessage, Tools.RecordSelectTitle);
            }
        }

        private void TxtBox_Award_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Save();
            }
        }

        private void Btn_AwardSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Btn_AwardCancel_Click(object sender, RoutedEventArgs e)
        {
            HideControls();

            CurrentAward = null;
        }

        private void ListBox_AwardDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideControls();
        }
    }
}
