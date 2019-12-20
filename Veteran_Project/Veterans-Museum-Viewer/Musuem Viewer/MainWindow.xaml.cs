using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Musuem_Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VeteranInfo vetWin;
        private List<int> searchResultIds;

        public MainWindow()
        {
            InitializeComponent();

            WindowState = WindowState.Maximized;
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

            vetWin = new VeteranInfo();
            vetWin.SetSingleRecord();
            vetWin.BuildAndShowDialog(veteranNumber);
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

            // Setup veteraninfo window and show dialog on click for Btn_ViewResults
        }

        private void Btn_ViewResults_Click(object sender, RoutedEventArgs e)
        {
            int selectedId;

            selectedId = Convert.ToInt32(ListBox_SearchResults.SelectedValue);

            vetWin = new VeteranInfo();
            vetWin.SetMultiRecord(searchResultIds);
            vetWin.BuildAndShowDialog(selectedId);
        }
    }

    public class SearchResults
    {
        public int IdNum { get; set; }
        public string Name { get; set; }
    }
}
