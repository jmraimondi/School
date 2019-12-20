using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Museum_Admin
{
    /// <summary>
    /// Interaction logic for Veterans.xaml
    /// </summary>
    public partial class Veterans : UserControl
    {
        private VeteranRecord vetWin;
        private List<int> searchResultIds;
        private MainWindow parentWin;
        public string CemeteryDetails { get; set; }
        public List<string> CemList { get; }

        public Veterans(MainWindow parent)
        {
            InitializeComponent();

            parentWin = parent;

            DataContext = this;

            CemList = CemeteryDBInfo.LoadStringList();
        }

        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            LoadSearchResults();
        }

        private void TxtBox_Search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LoadSearchResults();
            }
        }

        private void LoadSearchResults()
        {
            string searchTerm = TxtBox_Search.Text;
            int recordCount = 0;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT COUNT(*) FROM Veterans WHERE LName=@searchTerm";
                        command.Parameters.Add("@searchTerm", MySqlDbType.VarChar).Value = searchTerm;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                recordCount = reader.GetInt32(0);
                            }
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(Tools.DBErrorMessage, Tools.DBErrorTitle);
            }
            catch (MySqlException e)
            {
                Tools.HandleSQLExceptions(e);
            }

            if (recordCount == 1)
            {
                Lbl_SearchResults.Content = "";
                ListBox_SearchResults.Visibility = Visibility.Hidden;
                Btn_ViewResults.Visibility = Visibility.Hidden;
                LoadSingleSearchResult(searchTerm);
            }
            else if (recordCount > 1)
            {
                Lbl_SearchResults.Content = "Search Results";
                ListBox_SearchResults.Visibility = Visibility.Visible;
                Btn_ViewResults.Visibility = Visibility.Visible;
                LoadMultiSearchResult(searchTerm, recordCount);
            }
            else
            {
                Lbl_SearchResults.Content = "";
                ListBox_SearchResults.Visibility = Visibility.Hidden;
                Btn_ViewResults.Visibility = Visibility.Hidden;

                string message = "No record found for last name " + searchTerm + ".";
                string title = "Record Not Found";

                MessageBox.Show(message, title);
            }
        }

        private void LoadSingleSearchResult(string searchTerm)
        {
            int veteranNumber = 0;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT ID FROM Veterans WHERE LName=@searchTerm";
                        command.Parameters.Add("@searchTerm", MySqlDbType.VarChar).Value = searchTerm;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                veteranNumber = reader.GetInt32(0);
                            }
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(Tools.DBErrorMessage, Tools.DBErrorTitle);
            }
            catch (MySqlException e)
            {
                Tools.HandleSQLExceptions(e);
            }

            vetWin = new VeteranRecord(parentWin);
            vetWin.SetSingleRecord();
            vetWin.BuildAndShowDialog(veteranNumber);

            parentWin.DataContext = null;
            parentWin.MainWindowContent = vetWin;
            parentWin.DataContext = parentWin;
        }

        private void LoadMultiSearchResult(string searchTerm, int numberOfRecords)
        {
            List<int> Ids = new List<int>(numberOfRecords);
            List<SearchResults> results = new List<SearchResults>(numberOfRecords);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT ID,LName,FName,MName,Suffix " +
                            "FROM Veterans WHERE LName=@searchTerm ORDER BY FName";
                        command.Parameters.Add("@searchTerm", MySqlDbType.VarChar).Value = searchTerm;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string name = "";
                                int idNum;

                                idNum = reader.GetInt32(0);
                                Ids.Add(idNum);

                                if (!reader.IsDBNull(1))
                                {
                                    name += reader.GetString(1);
                                }

                                if (!reader.IsDBNull(2))
                                {
                                    name += ", ";
                                    name += reader.GetString(2);
                                }

                                if (!reader.IsDBNull(3))
                                {
                                    name += " ";
                                    name += reader.GetString(3);
                                }

                                if (!reader.IsDBNull(4))
                                {
                                    name += ", ";
                                    name += reader.GetString(4);
                                }

                                SearchResults record = new SearchResults()
                                {
                                    Name = name,
                                    IdNum = idNum
                                };

                                results.Add(record);
                            }
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(Tools.DBErrorMessage, Tools.DBErrorTitle);
            }
            catch (MySqlException e)
            {
                Tools.HandleSQLExceptions(e);
            }

            ListBox_SearchResults.DataContext = results;

            searchResultIds = Ids;
        }

        private void Btn_ViewResults_Click(object sender, RoutedEventArgs e)
        {
            int selectedId;

            selectedId = Convert.ToInt32(ListBox_SearchResults.SelectedValue);

            vetWin = new VeteranRecord(parentWin);
            vetWin.SetMultiRecord(searchResultIds);
            vetWin.BuildAndShowDialog(selectedId);

            parentWin.DataContext = null;
            parentWin.MainWindowContent = vetWin;
            parentWin.DataContext = parentWin;
        }

        private void Btn_Add_Click(object sender, RoutedEventArgs e)
        {
            vetWin = new VeteranRecord(parentWin);
            vetWin.SetSingleRecord();
            vetWin.BuildAndShowDialog();

            parentWin.DataContext = null;
            parentWin.MainWindowContent = vetWin;
            parentWin.DataContext = parentWin;
        }

        private void Btn_QuickAdd_Click(object sender, RoutedEventArgs e)
        {
            // Add selecting cemetery then put that in the argument list
            VetQuickAdd vetWin = new VetQuickAdd(CemeteryDetails);

            parentWin.DataContext = null;
            parentWin.MainWindowContent = vetWin;
            parentWin.DataContext = parentWin;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable button if actual cemetery selected
            // Disable on blank cemetery or "()" which is also a blank cemetery
            Btn_QuickAdd.IsEnabled = !(string.IsNullOrEmpty(CemeteryDetails) || CemeteryDetails == "()");
        }
    }

    public class SearchResults
    {
        public int IdNum { get; set; }
        public string Name { get; set; }
    }
}

