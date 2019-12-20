using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;

namespace Museum_Admin
{
    public class VetServiceDBInfo
    {
        // All database interaction in this program:
        // Source: https://docs.microsoft.com/en-us/azure/mysql/connect-csharp
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient?view=netframework-4.8
        // And all Microsoft .Net references under the SQLClient classes

        // This class contains a service (branch, rank, unit) record of a given veteran
        // It handles database loading and writing

        private bool hasDataChanged;

        private int id;
        public int sNum { get; set; }
        private string branch;
        private string rank;
        private string unitShip;

        public string Details
        {
            get
            {
                return branch + " - " + rank + " - " + unitShip;
            }
        }

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

        public string UnitShip
        {
            get
            {
                return unitShip;
            }
            set
            {
                unitShip = value;
                // Local to decide whether or not to write this data
                hasDataChanged = true;
                // Global to prevent leaving without saving
                Tools.hasDataChanged = true;
            }
        }

        // Constructor - New Record
        public VetServiceDBInfo()
        {
            id = 0;
            sNum = 0;
            hasDataChanged = false;
        }

        // Constructor - Loads record from database
        public VetServiceDBInfo(int sId)
        {
            sNum = sId;
            LoadDataFromDatabase();
            hasDataChanged = false;
        }

        // Copy Constructor
        public VetServiceDBInfo(VetServiceDBInfo other)
        {
            hasDataChanged = other.hasDataChanged;
            id = other.id;
            sNum = other.sNum;
            branch = other.branch;
            rank = other.rank;
            unitShip = other.unitShip;
        }

        // Sets the id number for the associated veteran record
        public void SetVetId(int idNum)
        {
            id = idNum;
        }

        // Writes the contents of the member variables to the database for the set ID number
        // If no set ID number, retrieves the auto generated one from the database and stores it
        public void WriteDataToDatabase()
        {
            // New record - Insert into database
            if (sNum == 0)
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
                        command.CommandText = @"UPDATE Services SET Branch = @branch, SRank = @rank, UnitShip = @unitShip WHERE SNum = @sNum;";

                        if (!string.IsNullOrEmpty(branch))
                        {
                            command.Parameters.Add("@branch", MySqlDbType.VarChar).Value = branch;
                        }
                        else
                        {
                            command.Parameters.Add("@branch", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(rank))
                        {
                            command.Parameters.Add("@rank", MySqlDbType.VarChar).Value = rank;
                        }
                        else
                        {
                            command.Parameters.Add("@rank", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(unitShip))
                        {
                            command.Parameters.Add("@unitShip", MySqlDbType.VarChar).Value = unitShip;
                        }
                        else
                        {
                            command.Parameters.Add("@unitShip", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        command.Parameters.Add("@sNum", MySqlDbType.Int32).Value = sNum;

                        int rowCount = command.ExecuteNonQuery();

                        // We should always only edit one record
                        if (rowCount != 1)
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

        // Inserts a new record into the database, sets the ID number to the auto generated ID
        private void InsertDataIntoDatabase()
        {
            // Don't insert records without corresponding IDs
            if (id == 0)
            {
                string message = "Error Vet ID not set.";
                string title = "ID Number Error";
                MessageBox.Show(message, title);

                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    // Insert a new record into the database
                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        string values = ") VALUES ( ";

                        command.CommandText = @"INSERT INTO Services ( ";

                        command.CommandText += "id, ";
                        values += "@id,";
                        command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

                        if (!string.IsNullOrEmpty(branch))
                        {
                            command.CommandText += "Branch, ";
                            values += "@branch,";
                            command.Parameters.Add("@branch", MySqlDbType.VarChar).Value = branch;
                        }

                        if (!string.IsNullOrEmpty(rank))
                        {
                            command.CommandText += "SRank, ";
                            values += "@rank,";
                            command.Parameters.Add("@rank", MySqlDbType.VarChar).Value = rank;
                        }

                        if (!string.IsNullOrEmpty(unitShip))
                        {
                            command.CommandText += "UnitShip, ";
                            values += "@unitShip,";
                            command.Parameters.Add("@unitShip", MySqlDbType.VarChar).Value = unitShip;
                        }

                        // Remove the last set of ", " from the command string
                        command.CommandText = command.CommandText.Substring(0, command.CommandText.Length - 2);
                        // Remove the last comma from the values string
                        values = values.Substring(0, values.Length - 1);
                        // Add the two halves together
                        command.CommandText += values;
                        command.CommandText += @");";

                        int rowCount = command.ExecuteNonQuery();

                        // We should always only create one record
                        if (rowCount != 1)
                        {
                            string message = "Error. Multiple records created. " + rowCount +
                                " records created.";
                            string title = "Mutiple Creation Error.";
                            MessageBox.Show(message, title);
                        }
                    }

                    // Retrieve the last id number created this session
                    using (MySqlCommand idCommand = conn.CreateCommand())
                    {
                        idCommand.CommandText = @"SELECT LAST_INSERT_ID();";

                        using (MySqlDataReader reader = idCommand.ExecuteReader())
                        {
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                sNum = reader.GetInt32(0);
                            }
                        }
                    }

                    // If we didn't successfully capture the ID number, have the user reopen the record
                    if (sNum == 0)
                    {
                        string message = "Error Retrieving sNum. Close this record and reopen it.";
                        string title = "Error Retrieving ID Number";
                        MessageBox.Show(message, title);
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

        // Loads member variables from the database for the set ID number
        private void LoadDataFromDatabase()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT id,Branch,SRank,UnitShip FROM Services WHERE SNum=@sId;";
                        command.Parameters.Add("@sId", MySqlDbType.Int32).Value = sNum;

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
                                    branch = reader.GetString(1);
                                }

                                if (!reader.IsDBNull(2))
                                {
                                    rank = reader.GetString(2);
                                }

                                if (!reader.IsDBNull(3))
                                {
                                    unitShip = reader.GetString(3);
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
                        command.CommandText = @"DELETE FROM Services WHERE SNum=@sId;";
                        command.Parameters.Add("@sId", MySqlDbType.Int32).Value = sNum;

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
