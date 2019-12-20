using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;
using System.IO;

namespace Museum_Admin
{
    public class BranchDBInfo
    {
        // All database interaction in this program:
        // Source: https://docs.microsoft.com/en-us/azure/mysql/connect-csharp
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient?view=netframework-4.8
        // And all Microsoft .Net references under the SQLClient classes

        // This class contains a branch record
        // It handles database loading and writing

        private bool hasDataChanged;
        private bool isNewRecord;
        private string branch;
        private string oldBranch;
        private string branchPicLoc;
        private string oldPicLoc;
        private bool hasPicChanged = false;

        public string Branch
        {
            get
            {
                return branch;
            }
            set
            {
                branch = value;
                // Local to decide whether or not to write this data
                hasDataChanged = true;
                // Global to prevent leaving without saving
                Tools.hasDataChanged = true;
            }
        }

        public string BranchPicLoc
        {
            get
            {
                return branchPicLoc;
            }
            set
            {
                // Save the old pic location to possibly delete
                oldPicLoc = branchPicLoc;
                // Set the new path (should be whole path).
                branchPicLoc = value;
                // Flag to handle deleting and copying image files
                hasPicChanged = true;
                // Local to decide whether or not to write this data
                hasDataChanged = true;
                // Global to prevent leaving without saving
                Tools.hasDataChanged = true;
            }
        }

        // Constructor - New Record
        public BranchDBInfo()
        {
            hasDataChanged = false;
            isNewRecord = true;
        }

        // Constructor - Loads record from database
        public BranchDBInfo(string branchName)
        {
            oldBranch = branchName;
            branch = branchName;
            branchPicLoc = "";

            LoadDataFromDatabase();

            hasDataChanged = false;
            isNewRecord = false;
        }

        // Copy Constructor
        public BranchDBInfo(BranchDBInfo other)
        {
            branch = other.branch;
            oldBranch = other.oldBranch;
            branchPicLoc = other.branchPicLoc;

            hasDataChanged = other.hasDataChanged;
            isNewRecord = other.isNewRecord;
        }

        // Writes the contents of the member variables to the database
        public void WriteDataToDatabase()
        {
            if (hasPicChanged)
            {
                UpdateImage();
            }

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

        // Copies image to the correct folder.
        private void UpdateImage()
        {
            string branchFolder;
            string oldPicPath;
            string newPicFilename;

            branchFolder = ConfigurationManager.AppSettings["InstallDirectory"];
            branchFolder += ConfigurationManager.AppSettings["BranchPicDirectory"];

            // Delete the old logo from the directory
            if (!string.IsNullOrEmpty(oldPicLoc))
            {
                oldPicPath = branchFolder;
                oldPicPath += oldPicLoc;

                try
                {
                    File.Delete(oldPicPath);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show(Tools.fileMissingMessage, Tools.fileMissingTitle);
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show(Tools.directoryMissingMessage, Tools.directoryMissingTitle);
                }
            }

            // Copy the new logo into the correct directory

            // Get the new logo's file name
            newPicFilename = branchPicLoc.Substring(branchPicLoc.LastIndexOf(@"\") + 1);

            // If the new logo is not the empty string (the delete logo case) copy it to the logo folder
            if (!string.IsNullOrEmpty(branchPicLoc))
            {
                try
                {
                    File.Copy(branchPicLoc, branchFolder + newPicFilename);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show(Tools.fileMissingMessage, Tools.fileMissingTitle);
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show(Tools.directoryMissingMessage, Tools.directoryMissingTitle);
                }

                BranchPicLoc = newPicFilename;
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
                        command.CommandText = @"UPDATE BranchesList SET BranchName = @BName, BranchPicLoc = @picLoc WHERE BranchName = @oldBranch;";
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = branch;
                        command.Parameters.Add("@oldBranch", MySqlDbType.VarChar).Value = oldBranch;

                        if (!string.IsNullOrEmpty(branchPicLoc))
                        {
                            command.Parameters.Add("@picLoc", MySqlDbType.VarChar).Value = branchPicLoc;
                        }
                        else
                        {
                            command.Parameters.Add("@picLoc", MySqlDbType.VarChar).Value = DBNull.Value;
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

                        command.CommandText = @"INSERT INTO BranchesList ( ";

                        command.CommandText += "BranchName, ";
                        values += "@BName,";
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = branch;

                        if (!string.IsNullOrEmpty(branchPicLoc))
                        {
                            command.CommandText += "BranchPicLoc, ";
                            values += "@picLoc,";
                            command.Parameters.Add("@picLoc", MySqlDbType.VarChar).Value = branchPicLoc;
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

        // Loads member variables from the database for the set branch
        private void LoadDataFromDatabase()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT BranchPicLoc FROM BranchesList WHERE BranchName = @BName;";
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = branch;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    branchPicLoc = reader.GetString(0);
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
                        command.CommandText = @"DELETE FROM BranchesList WHERE BranchName=@BName;";
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
                if (e.Number == 1451)
                {
                    MessageBox.Show("Ranks and / or awards exist for this branch. All ranks and awards must be deleted prior to deleting this branch.",
                        "Branch Delete Error");

                    return;
                }

                Tools.HandleSQLExceptions(e);
            }
        }

        // Loads the branches into a list of strings
        public static List<string> LoadStringList()
        {
            List<string> records = new List<string>();
            records.Add(""); // Add a default blank branch

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT BranchName FROM BranchesList;";

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

        // Loads the branches into a list of objects
        public static List<BranchDBInfo> LoadObjectList()
        {
            List<BranchDBInfo> records = new List<BranchDBInfo>();
            BranchDBInfo current;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT BranchName FROM BranchesList;";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    current = new BranchDBInfo(reader.GetString(0));
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
