using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;

namespace Museum_Admin
{
    public class AwardDBInfo
    {
        // All database interaction in this program:
        // Source: https://docs.microsoft.com/en-us/azure/mysql/connect-csharp
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient?view=netframework-4.8
        // And all Microsoft .Net references under the SQLClient classes

        // This class contains an award record
        // It handles database loading and writing

        private bool hasDataChanged;
        private bool isNewRecord;
        private string award;
        private string oldaward;
        private string branch;

        public string Award
        {
            get
            {
                return award;
            }
            set
            {
                award = value;
                // Local to decide whether or not to write this data
                hasDataChanged = true;
                // Global to prevent leaving without saving
                Tools.hasDataChanged = true;
            }
        }

        // Constructor - New Record
        public AwardDBInfo(string serviceBranch)
        {
            branch = serviceBranch;

            hasDataChanged = false;
            isNewRecord = true;
        }

        // Constructor - Existing Record
        public AwardDBInfo(string serviceBranch, string selectedAward)
        {
            oldaward = selectedAward;
            branch = serviceBranch;
            award = selectedAward;

            hasDataChanged = false;
            isNewRecord = false;
        }

        // Copy Constructor
        public AwardDBInfo(AwardDBInfo other)
        {
            branch = other.branch;
            award = other.award;
            oldaward = other.oldaward;

            hasDataChanged = other.hasDataChanged;
            isNewRecord = other.isNewRecord;
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

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"UPDATE AwardsList SET AwardName = @AName WHERE AwardName = @oldAward AND BranchName = @BName;";
                        command.Parameters.Add("@AName", MySqlDbType.VarChar).Value = award;
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = branch;
                        command.Parameters.Add("@oldAward", MySqlDbType.VarChar).Value = oldaward;

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
                        command.CommandText = @"INSERT INTO AwardsList (BranchName, AwardName) VALUES (@BName, @AName);";
                        command.Parameters.Add("@AName", MySqlDbType.VarChar).Value = award;
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = branch;

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
                        command.CommandText = @"DELETE FROM AwardsList WHERE AwardName = @AName AND BranchName = @BName;";
                        command.Parameters.Add("@AName", MySqlDbType.VarChar).Value = award;
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = branch;

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

        // Loads the awards for a given branch into a list of strings
        public static List<string> LoadStringList(string Branch)
        {
            List<string> records = new List<string>();
            records.Add(""); // Add a default blank award

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT AwardName FROM AwardsList WHERE BranchName=@BName;";
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = Branch;

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

        // Loads the awards for a given branch into a list of objects
        public static List<AwardDBInfo> LoadObjectList(string Branch)
        {
            List<AwardDBInfo> records = new List<AwardDBInfo>();
            AwardDBInfo current;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT AwardName FROM AwardsList WHERE BranchName=@BName;";
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = Branch;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    current = new AwardDBInfo(Branch, reader.GetString(0));

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
