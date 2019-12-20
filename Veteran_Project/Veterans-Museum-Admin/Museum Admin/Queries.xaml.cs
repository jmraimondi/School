using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Media;

namespace Museum_Admin
{
    /// <summary>
    /// Interaction logic for Queries.xaml
    /// </summary>
    public partial class Queries : UserControl, INotifyPropertyChanged
    {
        public List<string> BranchList { get; }
        public List<string> CemList { get; }
        public List<string> ConflictList { get; }
        public List<string> QueryTypeList { get; private set; }
        private string cemQueryType;
        public string SelectedCemetery { get; set; }
        private string branchQueryType;
        public string SelectedBranch { get; set; }
        private string conflictQueryType;
        public string SelectedConflict { get; set; }
        private string branchCemeteryQueryType;
        public string SelectedBranchBranchCem { get; set; }
        public string SelectedCemeteryBranchCem { get; set; }
        private string branchConflictQueryType;
        public string SelectedBranchBranchConflict { get; set; }
        public string SelectedConflictBranchConflict { get; set; }
        private string conflictCemeteryQueryType;
        public string SelectedConflictConflictCem { get; set; }
        public string SelectedCemeteryConflictCem { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private MainWindow parentWin;

        public string CemQueryType
        {
            get
            {
                return cemQueryType;
            }
            set
            {
                cemQueryType = value;
                NotifyPropertyChanged();
            }
        }

        public string BranchQueryType
        {
            get
            {
                return branchQueryType;
            }
            set
            {
                branchQueryType = value;
                NotifyPropertyChanged();
            }
        }

        public string ConflictQueryType
        {
            get
            {
                return conflictQueryType;
            }
            set
            {
                conflictQueryType = value;
                NotifyPropertyChanged();
            }
        }

        public string BranchCemeteryQueryType
        {
            get
            {
                return branchCemeteryQueryType;
            }
            set
            {
                branchCemeteryQueryType = value;
                NotifyPropertyChanged();
            }
        }

        public string BranchConflictQueryType
        {
            get
            {
                return branchConflictQueryType;
            }
            set
            {
                branchConflictQueryType = value;
                NotifyPropertyChanged();
            }
        }

        public string ConflictCemeteryQueryType
        {
            get
            {
                return conflictCemeteryQueryType;
            }
            set
            {
                conflictCemeteryQueryType = value;
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

        public Queries(MainWindow parent)
        {
            InitializeComponent();

            parentWin = parent;

            DataContext = this;

            // Load lists from the database
            BranchList = BranchDBInfo.LoadStringList();
            CemList = CemeteryDBInfo.LoadStringList();
            ConflictList = ConflictDBInfo.LoadStringList();

            SetupQueryLists();
        }

        // Build up Query List, set comboboxes to defaults
        private void SetupQueryLists()
        {
            QueryTypeList = new List<string>();
            QueryTypeList.Add("How many");
            QueryTypeList.Add("List the");

            CemQueryType = QueryTypeList[0];
            BranchQueryType = QueryTypeList[0];
            ConflictQueryType = QueryTypeList[0];
            BranchCemeteryQueryType = QueryTypeList[0];
            BranchConflictQueryType = QueryTypeList[0];
            ConflictCemeteryQueryType = QueryTypeList[0];

            // Disable buttons because the comboboxes start out blank
            Btn_CemeteryQuery.IsEnabled = false;
            Btn_BranchQuery.IsEnabled = false;
            Btn_ConflictQuery.IsEnabled = false;
            Btn_BranchCemQuery.IsEnabled = false;
            Btn_BranchConflictQuery.IsEnabled = false;
            Btn_ConflictCemQuery.IsEnabled = false;
        }

        // Converts a list of VeteranDBInfo to a list of strings
        private List<string> ConvertVetsToStrings(List<VeteranDBInfo> vets)
        {
            List<string> strings = new List<string>();

            foreach (VeteranDBInfo vet in vets)
            {
                strings.Add(vet.Name);
            }

            return strings;
        }

        // Call converter to convert VeteranDBInfo to strings
        // Builds a results window and opens it, using a query and list of veterans
        private void ShowVetResults(string query, List<VeteranDBInfo> vets)
        {
            ListResults resultsWin;
            List<string> results;

            results = ConvertVetsToStrings(vets);

            resultsWin = new ListResults(query, results);

            // Show the new user view
            parentWin.DataContext = null;
            parentWin.MainWindowContent = resultsWin;
            parentWin.DataContext = parentWin;
        }

        // Builds a results window and opens it, using a query and list of strings
        private void ShowListResults(string query, List<string> results)
        {
            ListResults resultsWin;

            resultsWin = new ListResults(query, results);

            // Show the new user view
            parentWin.DataContext = null;
            parentWin.MainWindowContent = resultsWin;
            parentWin.DataContext = parentWin;
        }

        private void Btn_CemeteryQuery_Click(object sender, RoutedEventArgs e)
        {
            if (CemQueryType == QueryTypeList[0])
            {
                int count;
                count = QueryDBInfo.CountByCemetery(SelectedCemetery);

                ShowVetResults("Number of veterans in " + SelectedCemetery + ": " + count, new List<VeteranDBInfo>());
            }
            else
            {
                List<VeteranDBInfo> results;
                results = QueryDBInfo.ListByCemetery(SelectedCemetery);

                ShowVetResults("List of veterans in " + SelectedCemetery, results);
            }
        }

        private void Btn_CemeteryListQuery_Click(object sender, RoutedEventArgs e)
        {
            List<string> results;
            results = QueryDBInfo.CountedCemeteryList();

            ShowListResults("List of cemeteries with veteran count per cemetery:", results);
        }

        private void Btn_BranchQuery_Click(object sender, RoutedEventArgs e)
        {
            if (BranchQueryType == QueryTypeList[0])
            {
                int count;
                count = QueryDBInfo.CountByBranch(SelectedBranch);

                ShowVetResults("Number of veterans in " + SelectedBranch + ": " + count, new List<VeteranDBInfo>());
            }
            else
            {
                List<VeteranDBInfo> results;
                results = QueryDBInfo.ListByBranch(SelectedBranch);

                ShowVetResults("List of veterans in " + SelectedBranch, results);
            }
        }

        private void Btn_BranchListQuery_Click(object sender, RoutedEventArgs e)
        {
            List<string> results;
            results = QueryDBInfo.CountedBranchList();

            ShowListResults("List of branches with veteran count per branch:", results);
        }

        private void Btn_ConflictQuery_Click(object sender, RoutedEventArgs e)
        {
            if (ConflictQueryType == QueryTypeList[0])
            {
                int count;
                count = QueryDBInfo.CountByConflict(SelectedConflict);

                ShowVetResults("Number of veterans in " + SelectedConflict + ": " + count, new List<VeteranDBInfo>());
            }
            else
            {
                List<VeteranDBInfo> results;
                results = QueryDBInfo.ListByConflict(SelectedConflict);

                ShowVetResults("List of veterans in " + SelectedConflict, results);
            }
        }

        private void Btn_ConflictListQuery_Click(object sender, RoutedEventArgs e)
        {
            List<string> results;
            results = QueryDBInfo.CountedConflictList();

            ShowListResults("List of conflicts with veteran count per conflict:", results);
        }

        private void Btn_TotalCountQuery_Click(object sender, RoutedEventArgs e)
        {
            int count;
            count = QueryDBInfo.CountOfDatabase();

            ShowVetResults("Number of veterans in the database: " + count, new List<VeteranDBInfo>());
        }

        private void Btn_BranchCemQuery_Click(object sender, RoutedEventArgs e)
        {
            if (BranchCemeteryQueryType == QueryTypeList[0])
            {
                int count;
                count = QueryDBInfo.CountByBranchCem(SelectedBranchBranchCem, SelectedCemeteryBranchCem);

                ShowVetResults("Number of " + SelectedBranchBranchCem+ " veterans in " + SelectedCemeteryBranchCem + ": " + count, new List<VeteranDBInfo>());
            }
            else
            {
                List<VeteranDBInfo> results;
                results = QueryDBInfo.ListByBranchCem(SelectedBranchBranchCem, SelectedCemeteryBranchCem);

                ShowVetResults("List of " + SelectedBranchBranchCem + " veterans in " + SelectedCemeteryBranchCem, results);
            }
        }

        private void Btn_BranchConflictQuery_Click(object sender, RoutedEventArgs e)
        {
            if (BranchConflictQueryType == QueryTypeList[0])
            {
                int count;
                count = QueryDBInfo.CountByBranchConflict(SelectedBranchBranchConflict, SelectedConflictBranchConflict);

                ShowVetResults("Number of " + SelectedBranchBranchConflict + " veterans in " + SelectedConflictBranchConflict + ": " + count, new List<VeteranDBInfo>());
            }
            else
            {
                List<VeteranDBInfo> results;
                results = QueryDBInfo.ListByBranchConflict(SelectedBranchBranchConflict, SelectedConflictBranchConflict);

                ShowVetResults("List of " + SelectedBranchBranchConflict + " veterans in " + SelectedConflictBranchConflict, results);
            }
        }

        private void Btn_ConflictCemQuery_Click(object sender, RoutedEventArgs e)
        {
            if (ConflictCemeteryQueryType == QueryTypeList[0])
            {
                int count;
                count = QueryDBInfo.CountByConflictCem(SelectedConflictConflictCem, SelectedCemeteryConflictCem);

                ShowVetResults("Number of " + SelectedConflictConflictCem + " veterans in " + SelectedCemeteryConflictCem + ": " + count, new List<VeteranDBInfo>());
            }
            else
            {
                List<VeteranDBInfo> results;
                results = QueryDBInfo.ListByConflictCem(SelectedConflictConflictCem, SelectedCemeteryConflictCem);

                ShowVetResults("List of " + SelectedConflictConflictCem + " veterans in " + SelectedCemeteryConflictCem, results);
            }
        }

        private void CmbBox_Cemetery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable button if actual cemetery selected
            // Disable on blank cemetery or "()" which is also a blank cemetery
            Btn_CemeteryQuery.IsEnabled = !(string.IsNullOrEmpty(SelectedCemetery) || SelectedCemetery == "()");
        }

        private void CmbBox_Branch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable button if actual branch selected, disable on blank branch item
            Btn_BranchQuery.IsEnabled = !string.IsNullOrEmpty(SelectedBranch);
        }

        private void CmbBox_Conflict_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable button if actual conflict selected, disable on blank conflict item
            Btn_ConflictQuery.IsEnabled = !string.IsNullOrEmpty(SelectedConflict);
        }

        private void CmbBox_BranchBranchCem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable button if actual conflict selected, disable on blank conflict item
            bool buttonEnable;

            buttonEnable = !(string.IsNullOrEmpty(SelectedBranchBranchCem) || (string.IsNullOrEmpty(SelectedCemeteryBranchCem) || SelectedCemeteryBranchCem == "()"));

            Btn_BranchCemQuery.IsEnabled = buttonEnable;
        }

        private void CmbBox_CemeteryBranchCem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable button if actual conflict selected, disable on blank conflict item
            bool buttonEnable;

            buttonEnable = !(string.IsNullOrEmpty(SelectedBranchBranchCem) || (string.IsNullOrEmpty(SelectedCemeteryBranchCem) || SelectedCemeteryBranchCem == "()"));

            Btn_BranchCemQuery.IsEnabled = buttonEnable;
        }

        private void CmbBox_BranchBranchConflict_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable button if actual conflict selected, disable on blank conflict item
            bool buttonEnable;

            buttonEnable = !(string.IsNullOrEmpty(SelectedBranchBranchConflict) || string.IsNullOrEmpty(SelectedConflictBranchConflict));

            Btn_BranchConflictQuery.IsEnabled = buttonEnable;
        }

        private void CmbBox_ConflictBranchConflict_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable button if actual conflict selected, disable on blank conflict item
            bool buttonEnable;

            buttonEnable = !(string.IsNullOrEmpty(SelectedBranchBranchConflict) || string.IsNullOrEmpty(SelectedConflictBranchConflict));

            Btn_BranchConflictQuery.IsEnabled = buttonEnable;
        }

        private void CmbBox_ConflictConflictCem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable button if actual conflict selected, disable on blank conflict item
            bool buttonEnable;

            buttonEnable = !(string.IsNullOrEmpty(SelectedConflictConflictCem) || (string.IsNullOrEmpty(SelectedCemeteryConflictCem) || SelectedCemeteryConflictCem == "()"));

            Btn_ConflictCemQuery.IsEnabled = buttonEnable;
        }

        private void CmbBox_CemeteryConflictCem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable button if actual conflict selected, disable on blank conflict item
            bool buttonEnable;

            buttonEnable = !(string.IsNullOrEmpty(SelectedConflictConflictCem) || (string.IsNullOrEmpty(SelectedCemeteryConflictCem) || SelectedCemeteryConflictCem == "()"));

            Btn_ConflictCemQuery.IsEnabled = buttonEnable;
        }
    }
}
