using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;

namespace Museum_Admin
{
    public class RankDBInfo
    {
        // All database interaction in this program:
        // Source: https://docs.microsoft.com/en-us/azure/mysql/connect-csharp
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient?view=netframework-4.8
        // And all Microsoft .Net references under the SQLClient classes

        // This class contains a rank record
        // It handles database loading and writing

        private bool hasDataChanged;
        private bool isNewRecord;
        private string rank;
        private string oldRank;
        private string branch;
        private string rankAbrev;

        public string Rank
        {
            get
            {
                return rank;
            }
            set
            {
                rank = value;
                // Local to decide whether or not to write this data
                hasDataChanged = true;
                // Global to prevent leaving without saving
                Tools.hasDataChanged = true;
            }
        }

        public string RankAbrev
        {
            get
            {
                return rankAbrev;
            }
            set
            {
                rankAbrev = value;
                // Local to decide whether or not to write this data
                hasDataChanged = true;
                // Global to prevent leaving without saving
                Tools.hasDataChanged = true;
            }
        }

        // Constructor - New Record
        public RankDBInfo(string serviceBranch)
        {
            branch = serviceBranch;

            hasDataChanged = false;
            isNewRecord = true;
        }

        // Constructor - Loads record from database
        public RankDBInfo(string serviceBranch, string serviceRank)
        {
            oldRank = serviceRank;
            branch = serviceBranch;
            rank = serviceRank;

            LoadDataFromDatabase();

            hasDataChanged = false;
            isNewRecord = false;
        }

        // Copy Constructor
        public RankDBInfo(RankDBInfo other)
        {
            branch = other.branch;
            rank = other.rank;
            oldRank = other.oldRank;
            rankAbrev = other.rankAbrev;

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
                        command.CommandText = @"UPDATE RanksList SET RankName = @RName, RankAbrev = @rankA" +
                            "WHERE RankName = @oldRank AND BranchName = @BName;";
                        command.Parameters.Add("@RName", MySqlDbType.VarChar).Value = rank;
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = branch;
                        command.Parameters.Add("@oldRank", MySqlDbType.VarChar).Value = oldRank;

                        if (!string.IsNullOrEmpty(rankAbrev))
                        {
                            command.Parameters.Add("@rankA", MySqlDbType.VarChar).Value = rankAbrev;
                        }
                        else
                        {
                            command.Parameters.Add("@rankA", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

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
                        string values = ") VALUES ( ";

                        command.CommandText = @"INSERT INTO RanksList ( ";

                        command.CommandText += "RankName, BranchName, ";
                        values += "@RName, @BName,";
                        command.Parameters.Add("@RName", MySqlDbType.VarChar).Value = rank;
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = branch;

                        if (!string.IsNullOrEmpty(rankAbrev))
                        {
                            command.CommandText += "RankAbrev, ";
                            values += "@rankA,";
                            command.Parameters.Add("@rankA", MySqlDbType.VarChar).Value = rankAbrev;
                        }

                        // Remove the last set of ", " from the command string
                        command.CommandText = command.CommandText.Substring(0, command.CommandText.Length - 2);
                        // Remove the last comma from the values string
                        values = values.Substring(0, values.Length - 1);
                        // Add the two halves together
                        command.CommandText += values;
                        command.CommandText += @");";

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

        // Loads member variables from the database for the set rank and branch
        private void LoadDataFromDatabase()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT RankAbrev FROM RanksList WHERE RankName = @RName AND BranchName = @BName;";
                        command.Parameters.Add("@RName", MySqlDbType.VarChar).Value = rank;
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = branch;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    rankAbrev = reader.GetString(0);
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
                        command.CommandText = @"DELETE FROM RanksList WHERE RankName = @RName AND BranchName = @BName;";
                        command.Parameters.Add("@RName", MySqlDbType.VarChar).Value = rank;
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

        // Loads the ranks for a given branch into a list of strings
        public static List<string> LoadStringList(string Branch)
        {
            List<string> records = new List<string>();
            records.Add(""); // Add a default blank rank

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT RankName FROM RanksList WHERE BranchName=@BName;";
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

        // Loads the ranks for a given branch into a list of objects
        public static List<RankDBInfo> LoadObjectList(string Branch)
        {
            List<RankDBInfo> records = new List<RankDBInfo>();
            RankDBInfo current;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT RankName FROM RanksList WHERE BranchName = @BName;";
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = Branch;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    current = new RankDBInfo(Branch, reader.GetString(0));

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
