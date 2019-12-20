using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Museum_Admin
{
    /// <summary>
    /// Interaction logic for ServiceDetails.xaml
    /// </summary>
    public partial class ServiceDetails : Window
    {
        public VetServiceDBInfo ServiceInfo { get; set; }
        public VetServiceDBInfo oldInfo;
        public List<string> BranchList { get; }
        public List<string> RankList { get; set; }
        public ObservableCollection<string> Ranks { get; set; }
        public bool IsOk { get; set; }
        private bool dataPreviouslyChanged;

        // Default Constructor
        public ServiceDetails()
        {
            InitializeComponent();

            ServiceInfo = new VetServiceDBInfo();

            IsOk = false;
            dataPreviouslyChanged = Tools.hasDataChanged;

            DataContext = this;

            BranchList = BranchDBInfo.LoadStringList();
            Ranks = new ObservableCollection<string>();
        }

        // Overloaded Constructor For Editing
        public ServiceDetails(VetServiceDBInfo record)
        {
            InitializeComponent();

            oldInfo = record;

            ServiceInfo = new VetServiceDBInfo(oldInfo);

            IsOk = false;
            dataPreviouslyChanged = Tools.hasDataChanged;

            DataContext = this;

            BranchList = BranchDBInfo.LoadStringList();
            Ranks = new ObservableCollection<string>();
        }

        // Set the veteran ID number (if known)
        public void SetId(int idNum)
        {
            ServiceInfo.SetVetId(idNum);
        }

        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            IsOk = true;
            Close();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            // No change requested, give back the old version
            ServiceInfo = oldInfo;

            // Nothing changed in this dialog, so set hasDataChanged to how we found it when we opened the dialog
            Tools.hasDataChanged = dataPreviouslyChanged;

            IsOk = false;
            Close();
        }

        // Loads data into the ranks observable collection for the dropdown for the selected branch
        private void CmbBox_Service_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RankList = RankDBInfo.LoadStringList(ServiceInfo.Branch);

            Ranks.Clear();
            
            foreach (string rank in RankList)
            {
                Ranks.Add(rank);
            }
        }

        private void CmbBox_Service_DropDownOpened(object sender, EventArgs e)
        {
            CmbBox_Service.Items.Refresh();
        }
    }
}
