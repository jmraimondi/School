using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;
using System.IO;

namespace Museum_Admin
{
    public class VetCommentRecordDBInfo
    {
        // All database interaction in this program:
        // Source: https://docs.microsoft.com/en-us/azure/mysql/connect-csharp
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient?view=netframework-4.8
        // And all Microsoft .Net references under the SQLClient classes

        // This class contains the record of a comment about a given veteran
        // It handles database loading and writing

        private int id;
        private int cNum;
        private string comment;

        public int ID
        {
            get
            {
                return id;
            }
        }

        public string Comment
        {
            get
            {
                return comment;
            }
        }

        // Constructor - Loads record from database
        public VetCommentRecordDBInfo(int idNum)
        {
            cNum = idNum;
            LoadDataFromDatabase();
        }

        // Load member variables from the database for the set comment
        private void LoadDataFromDatabase()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT ID, UserComment FROM UserComments WHERE CNum = @cnum;";
                        command.Parameters.Add("@cnum", MySqlDbType.Int32).Value = cNum;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    id = reader.GetInt32(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    comment = reader.GetString(1);
                                }
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
        }

        // Remove this record from the database
        public void DeleteFromDatabase()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"DELETE FROM UserComments WHERE CNum = @cnum;";
                        command.Parameters.Add("@cnum", MySqlDbType.Int32).Value = cNum;

                        int rowCount = command.ExecuteNonQuery();

                        // We should always only delete one record
                        if (rowCount != 1)
                        {
                            string message = "Error. Multiple records deleted. " + rowCount +
                                " records deleted.";
                            string title = "Mutiple Deletion Error.";
                            MessageBox.Show(message, title);
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
        }
    }
}
