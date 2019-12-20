using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Museum_Admin
{
    /// <summary>
    /// Interaction logic for ConflictDetails.xaml
    /// </summary>
    public partial class ConflictDetails : Window
    {
        public VetConflictDBInfo ConflictInfo { get; set; }
        private VetConflictDBInfo oldInfo;
        private List<string> conflictList;
        public ObservableCollection<string> Conflicts { get; set; }
        public bool IsOk { get; set; }
        private bool dataPreviouslyChanged;

        // Default Constructor
        public ConflictDetails()
        {
            InitializeComponent();

            ConflictInfo = new VetConflictDBInfo();

            IsOk = false;
            dataPreviouslyChanged = Tools.hasDataChanged;

            DataContext = this;

            LoadData();
        }

        // Overloaded Constructor For Editing
        public ConflictDetails(VetConflictDBInfo record)
        {
            InitializeComponent();

            oldInfo = record;

            ConflictInfo = new VetConflictDBInfo(oldInfo);

            IsOk = false;
            dataPreviouslyChanged = Tools.hasDataChanged;

            DataContext = this;

            LoadData();
        }

        // Set the veteran ID number (if known)
        public void SetId(int idNum)
        {
            ConflictInfo.SetVetId(idNum);
        }

        // Loads data into the bound observable collection for the dropdown
        private void LoadData()
        {
            conflictList = ConflictDBInfo.LoadStringList();

            Conflicts = new ObservableCollection<string>();

            foreach (string item in conflictList)
            {
                Conflicts.Add(item);
            }
        }

        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            IsOk = true;
            Close();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            // No change requested, give back the old version
            ConflictInfo = oldInfo;

            // Nothing changed in this dialog, so set hasDataChanged to how we found it when we opened the dialog
            Tools.hasDataChanged = dataPreviouslyChanged;

            IsOk = false;
            Close();
        }
    }
}
