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
    /// Interaction logic for Comments.xaml
    /// </summary>
    public partial class Comments : UserControl
    {
        private CommentRecord vetCommentWin;
        private List<int> searchResultIds;
        List<SearchResults> results;
        private MainWindow parentWin;

        public Comments(MainWindow parent)
        {
            InitializeComponent();

            parentWin = parent;

            DataContext = this;

            LoadUserComments();
        }

        private void LoadUserComments()
        {
            List<int> Ids = new List<int>();
            results = new List<SearchResults>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT CNum,LName,FName,MName,Suffix " +
                            "FROM UserComments NATURAL JOIN Veterans ORDER BY LName, FName";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string name = "";
                                int cNum;

                                cNum = reader.GetInt32(0);
                                Ids.Add(cNum);

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
                                    IdNum = cNum
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

            ListBox_CommentDetails.DataContext = results;

            searchResultIds = Ids;
        }

        private void Btn_ViewComment_Click(object sender, RoutedEventArgs e)
        {
            int selectedId;

            selectedId = Convert.ToInt32(ListBox_CommentDetails.SelectedValue);

            // Verify the user selected a record to view
            if (selectedId > 0)
            {
                if (searchResultIds.Count > 1)
                {
                    vetCommentWin = new CommentRecord(parentWin);
                    vetCommentWin.SetMultiRecord(searchResultIds);
                    vetCommentWin.BuildAndShowDialog(selectedId);

                }
                else
                {
                    vetCommentWin = new CommentRecord(parentWin);
                    vetCommentWin.SetSingleRecord();
                    vetCommentWin.BuildAndShowDialog(selectedId);
                }

                parentWin.DataContext = null;
                parentWin.MainWindowContent = vetCommentWin;
                parentWin.DataContext = parentWin;
            }
            else
            {
                MessageBox.Show(Tools.RecordSelectMessage, Tools.RecordSelectTitle);
            }
        }
    }
}
