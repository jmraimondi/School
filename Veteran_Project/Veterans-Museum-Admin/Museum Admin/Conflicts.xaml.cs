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
    /// Interaction logic for Conflicts.xaml
    /// </summary>
    public partial class Conflicts : UserControl, INotifyPropertyChanged
    {
        // Help with INotifyPropertyChanged
        // Source: https://stackoverflow.com/questions/14823119/updatesourcetrigger-not-working
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=netframework-4.8

        private List<ConflictDBInfo> conflictList;
        private ConflictDBInfo currentConflict;
        public event PropertyChangedEventHandler PropertyChanged;

        public ConflictDBInfo CurrentConflict
        {
            get
            {
                return currentConflict;
            }
            set
            {
                currentConflict = value;
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

        public Conflicts()
        {
            InitializeComponent();

            conflictList = ConflictDBInfo.LoadObjectList();

            ListBox_ConflictDetails.DataContext = conflictList;

            DataContext = this;
        }

        private void HideControls()
        {
            Lbl_Conflicts.Visibility = Visibility.Hidden;
            TxtBox_Conflicts.Visibility = Visibility.Hidden;
            Btn_ConflictSave.Visibility = Visibility.Hidden;
            Btn_ConflictCancel.Visibility = Visibility.Hidden;
        }

        private void ShowControls()
        {
            Lbl_Conflicts.Visibility = Visibility.Visible;
            TxtBox_Conflicts.Visibility = Visibility.Visible;
            Btn_ConflictSave.Visibility = Visibility.Visible;
            Btn_ConflictCancel.Visibility = Visibility.Visible;
        }

        private void Save()
        {
            CurrentConflict.WriteDataToDatabase();

            conflictList = ConflictDBInfo.LoadObjectList();

            ListBox_ConflictDetails.DataContext = conflictList;

            HideControls();

            CurrentConflict = null;
        }

        private void Btn_AddConflict_Click(object sender, RoutedEventArgs e)
        {
            CurrentConflict = new ConflictDBInfo();

            ShowControls();
        }

        private void Btn_EditConflict_Click(object sender, RoutedEventArgs e)
        {
            string selected;

            selected = Convert.ToString(ListBox_ConflictDetails.SelectedValue);

            // Verify the user selected a record to edit
            if (!string.IsNullOrEmpty(selected))
            {
                CurrentConflict = new ConflictDBInfo(selected);

                ShowControls();
            }
            else
            {
                MessageBox.Show(Tools.RecordSelectMessage, Tools.RecordSelectTitle);
            }
        }

        private void Btn_DeleteConflict_Click(object sender, RoutedEventArgs e)
        {
            string selected;

            selected = Convert.ToString(ListBox_ConflictDetails.SelectedValue);

            // Verify the user selected a record to delete
            if (!string.IsNullOrEmpty(selected))
            {
                MessageBoxResult result;
                result = MessageBox.Show(Tools.deleteMessage, Tools.deleteTitle, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (ConflictDBInfo conflict in conflictList)
                    {
                        if (conflict.Conflict == selected)
                        {
                            // Remove from the database
                            conflict.DeleteFromDatabase();
                        }
                    }

                    conflictList = ConflictDBInfo.LoadObjectList();

                    ListBox_ConflictDetails.DataContext = conflictList;
                }
            }
            else
            {
                MessageBox.Show(Tools.RecordDeleteMessage, Tools.RecordSelectTitle);
            }
        }

        private void Btn_ConflictSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Btn_ConflictCancel_Click(object sender, RoutedEventArgs e)
        {
            HideControls();

            CurrentConflict = null;
        }

        private void TxtBox_Conflicts_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Save();
            }
        }

        private void ListBox_ConflictDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideControls();

            CurrentConflict = null;
        }
    }
}
