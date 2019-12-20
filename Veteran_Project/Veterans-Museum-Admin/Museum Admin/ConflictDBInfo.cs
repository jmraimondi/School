using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;

namespace Museum_Admin
{
    public class ConflictDBInfo
    {
        // All database interaction in this program:
        // Source: https://docs.microsoft.com/en-us/azure/mysql/connect-csharp
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient?view=netframework-4.8
        // And all Microsoft .Net references under the SQLClient classes

        // This class contains a conflict (war) record
        // It handles database loading and writing

        private bool hasDataChanged;
        private bool isNewRecord;
        private string conflict;
        private string oldName;

        public string Conflict
        {
            get
            {
                return conflict;
            }
            set
            {
                conflict = value;
                // Local to decide whether or not to write this data
                hasDataChanged = true;
                // Global to prevent leaving without saving
                Tools.hasDataChanged = true;
            }
        }

        // Constructor - New Record
        public ConflictDBInfo()
        {
            hasDataChanged = false;
            isNewRecord = true;
        }

        // Constructor - Loads record from database
        public ConflictDBInfo(string conflictName)
        {
            conflict = conflictName;
            oldName = conflictName;

            // LoadDataFromDatabase not needed since only value in relation is CLName

            hasDataChanged = false;
            isNewRecord = false;
        }

        // Copy Constructor
        public ConflictDBInfo(ConflictDBInfo other)
        {
            hasDataChanged = other.hasDataChanged;
            isNewRecord = other.isNewRecord;
            conflict = other.conflict;
            oldName = other.oldName;
        }

        // Writes the contents of the member variables to the database
        public void WriteDataToDatabase()
        {
            // New record - Insert into database
            if (isNewRecord && hasDataChanged)
            {
                InsertDataIntoDatabase();
            }
            // Existing record - Update database
            else if (hasDataChanged)
            {
                UpdateDatabase();
            }
        }

        // Updates this record in the database with the contents of member variables
        private void UpdateDatabase()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    // Insert a new record into the database
                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"UPDATE ConflictList SET CLName = @conflict WHERE CLName = @oldName;";
                        command.Parameters.Add("@conflict", MySqlDbType.VarChar).Value = conflict;
                        command.Parameters.Add("@oldName", MySqlDbType.VarChar).Value = oldName;

                        int rowCount = command.ExecuteNonQuery();

                        if (rowCount == 1)
                        {
                            // User has now written the data to the database
                            Tools.hasDataChanged = false;
                        }
                        // We should always only edit one record
                        else
                        {
                            string message = "Error. Multiple records edited. " + rowCount +
                                " records edited.";
                            string title = "Mutiple Edit Error.";
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

        // Inserts a new record into the database
        private void InsertDataIntoDatabase()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    // Insert a new record into the database
                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"INSERT INTO ConflictList (CLName) VALUES (@conflict);";
                        command.Parameters.Add("@conflict", MySqlDbType.VarChar).Value = conflict;

                        int rowCount = command.ExecuteNonQuery();

                        if (rowCount == 1)
                        {
                            // User has now written the data to the database
                            Tools.hasDataChanged = false;
                        }
                        // We should always only edit one record
                        else
                        {
                            string message = "Error. Multiple records created. " + rowCount +
                                " records created.";
                            string title = "Mutiple Creation Error.";
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
                        command.CommandText = @"DELETE FROM ConflictList WHERE CLName=@conflict;";
                        command.Parameters.Add("@conflict", MySqlDbType.VarChar).Value = conflict;

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

        // Loads the conflicts into a list of strings
        public static List<string> LoadStringList()
        {
            List<string> records = new List<string>();
            records.Add(""); // Add a default blank conflict

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT CLName FROM ConflictList;";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    records.Add(reader.GetString(0));
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

            return records;
        }

        // Loads the conflicts into a list of objects
        public static List<ConflictDBInfo> LoadObjectList()
        {
            List<ConflictDBInfo> records = new List<ConflictDBInfo>();
            ConflictDBInfo current;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT CLName FROM ConflictList;";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    current = new ConflictDBInfo(reader.GetString(0));
                                    records.Add(current);
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

            return records;
        }
    }
}