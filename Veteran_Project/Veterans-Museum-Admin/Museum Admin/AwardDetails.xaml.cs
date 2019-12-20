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
    /// Interaction logic for AwardDetails.xaml
    /// </summary>
    public partial class AwardDetails : Window, INotifyPropertyChanged
    {
        public VetAwardDBInfo AwardInfo { get; set; }
        private VetAwardDBInfo oldInfo;
        public List<string> BranchList { get; }
        public List<string> awardList;
        public bool IsOk { get; set; }
        private bool dataPreviouslyChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> AwardList
        {
            get
            {
                return awardList;
            }
            set
            {
                awardList = value;
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

        // Default Constructor
        public AwardDetails()
        {
            InitializeComponent();

            AwardInfo = new VetAwardDBInfo();

            IsOk = false;
            dataPreviouslyChanged = Tools.hasDataChanged;

            DataContext = this;

            BranchList = BranchDBInfo.LoadStringList();
        }

        // Overloaded Constructor For Editing
        public AwardDetails(VetAwardDBInfo record)
        {
            InitializeComponent();

            oldInfo = record;

            AwardInfo = new VetAwardDBInfo(oldInfo);

            IsOk = false;
            dataPreviouslyChanged = Tools.hasDataChanged;

            DataContext = this;

            BranchList = BranchDBInfo.LoadStringList();
        }

        // Set the veteran ID number (if known)
        public void SetId(int idNum)
        {
            AwardInfo.SetVetId(idNum);
        }

        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            IsOk = true;
            Close();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            // No change requested, give back the old version
            AwardInfo = oldInfo;

            // Nothing changed in this dialog, so set hasDataChanged to how we found it when we opened the dialog
            Tools.hasDataChanged = dataPreviouslyChanged;

            IsOk = false;
            Close();
        }

        // Loads data into the ranks observable collection for the dropdown for the selected branch
        private void CmbBox_Service_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AwardList = AwardDBInfo.LoadStringList(AwardInfo.Branch);
        }
    }
}
