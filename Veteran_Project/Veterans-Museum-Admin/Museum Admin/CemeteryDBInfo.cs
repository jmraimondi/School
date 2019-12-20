using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;
using System.IO;

namespace Museum_Admin
{
    public class CemeteryDBInfo
    {
        // All database interaction in this program:
        // Source: https://docs.microsoft.com/en-us/azure/mysql/connect-csharp
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient?view=netframework-4.8
        // And all Microsoft .Net references under the SQLClient classes

        // This class contains a cemetery record
        // It handles database loading and writing

        private bool hasDataChanged;
        private bool isNewRecord;
        private string name;
        private string oldName;
        private string city;
        private string oldCity;
        private string address;
        private string gps;
        private string airPicLoc;
        private string dirPicLoc;
        private bool hasAirPicChanged = false;
        private string oldAirPic;
        private bool hasDirPicChanged = false;
        private string oldDirPic;

        public string Details
        {
            get
            {
                return name + " (" + city + ")";
            }
            set
            {
                Tools.CemDetailsParser(value, ref name, ref city);
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                // Local to decide whether or not to write this data
                hasDataChanged = true;
                // Global to prevent leaving without saving
                Tools.hasDataChanged = true;
            }
        }

        public string City
        {
            get
            {
                return city;
            }
            set
            {
                city = value;
                // Local to decide whether or not to write this data
                hasDataChanged = true;
                // Global to prevent leaving without saving
                Tools.hasDataChanged = true;
            }
        }

        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
                // Local to decide whether or not to write this data
                hasDataChanged = true;
                // Global to prevent leaving without saving
                Tools.hasDataChanged = true;
            }
        }

        public string GPS
        {
            get
            {
                return gps;
            }
            set
            {
                gps = value;
                // Local to decide whether or not to write this data
                hasDataChanged = true;
                // Global to prevent leaving without saving
                Tools.hasDataChanged = true;
            }
        }

        public string AirPicLoc
        {
            get
            {
                return airPicLoc;
            }
            set
            {
                // Save old data and set changed flag
                oldAirPic = AirPicLoc;
                hasAirPicChanged = true;

                airPicLoc = value;
                // Local to decide whether or not to write this data
                hasDataChanged = true;
                // Global to prevent leaving without saving
                Tools.hasDataChanged = true;
            }
        }

        public string DirPicLoc
        {
            get
            {
                return dirPicLoc;
            }
            set
            {
                // Save old data and set changed flag
                oldDirPic = dirPicLoc;
                hasDirPicChanged = true;

                dirPicLoc = value;
                // Local to decide whether or not to write this data
                hasDataChanged = true;
                // Global to prevent leaving without saving
                Tools.hasDataChanged = true;
            }
        }

        // Constructor - New Record
        public CemeteryDBInfo()
        {
            hasDataChanged = false;
            isNewRecord = true;
        }

        // Constructor - Loads record from database
        public CemeteryDBInfo(string CName, string CCity)
        {
            oldName = CName;
            oldCity = CCity;
            name = CName;
            city = CCity;

            LoadDataFromDatabase();

            hasDataChanged = false;
            isNewRecord = false;
        }

        // Constructor - Loads record from database
        public CemeteryDBInfo(string selectedDetails)
        {
            Details = selectedDetails;
            oldName = name;
            oldCity = city;

            LoadDataFromDatabase();

            hasDataChanged = false;
            isNewRecord = false;
        }

        // Copy Constructor
        public CemeteryDBInfo(CemeteryDBInfo other)
        {
            name = other.name;
            oldName = other.oldName;
            city = other.city;
            oldCity = other.oldCity;
            address = other.address;
            gps = other.gps;
            airPicLoc = other.airPicLoc;
            dirPicLoc = other.dirPicLoc;

            hasDataChanged = other.hasDataChanged;
            isNewRecord = other.isNewRecord;
        }

        // Writes the contents of the member variables to the database
        public void WriteDataToDatabase()
        {
            // Handle picture copying and getting file names prior to writing to database.
            if (hasAirPicChanged || hasDirPicChanged)
            {
                UpdateAllImages();

                hasAirPicChanged = false;
                hasDirPicChanged = false;
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

        // Calls UpdateImage for each of the images, if they have changed.
        private void UpdateAllImages()
        {
            if (hasAirPicChanged)
            {
                string folder;
                folder = ConfigurationManager.AppSettings["InstallDirectory"];
                folder += ConfigurationManager.AppSettings["CemAirPicDirectory"];

                airPicLoc = UpdateImage(folder, oldAirPic, airPicLoc);
            }

            if (hasDirPicChanged)
            {
                string folder;
                folder = ConfigurationManager.AppSettings["InstallDirectory"];
                folder += ConfigurationManager.AppSettings["CemDirectionsDirectory"];

                dirPicLoc = UpdateImage(folder, oldDirPic, dirPicLoc);
            }
        }

        // Copies image to the correct folder.
        private string UpdateImage(string folder, string oldPic, string newPicPath)
        {
            string oldPicPath;
            string newPicFilename;

            // Delete the old image from the directory
            if (!string.IsNullOrEmpty(oldPic))
            {
                oldPicPath = folder;
                oldPicPath += oldPic;

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

            // Copy the new pic into the correct directory
            // If the path exists (blank is the delete image case)
            if (!string.IsNullOrEmpty(newPicPath))
            {
                // Get the new image's file name
                newPicFilename = newPicPath.Substring(newPicPath.LastIndexOf(@"\") + 1);

                // Append cemetery name and city to front of filename to make sorting easier and prevent duplicate file names
                newPicFilename = Name + " " + City + " " + newPicFilename;

                try
                {
                    File.Copy(newPicPath, folder + newPicFilename);
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
            else
            {
                newPicFilename = "";
            }

            return newPicFilename;
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
                        command.CommandText = @"UPDATE Cemeteries SET CName = @Name, CCity = @City, CAddress = @address, GPS = @cgps, " +
                        "CemAirPicLoc = @airPicLoc, DirectionsPicLoc = @dirPicLoc WHERE CName = @oldName AND CCity = @oldCity;";
                        command.Parameters.Add("@Name", MySqlDbType.VarChar).Value = name;
                        command.Parameters.Add("@City", MySqlDbType.VarChar).Value = city;
                        command.Parameters.Add("@oldName", MySqlDbType.VarChar).Value = oldName;
                        command.Parameters.Add("@oldCity", MySqlDbType.VarChar).Value = oldCity;

                        if (!string.IsNullOrEmpty(address))
                        {
                            command.Parameters.Add("@address", MySqlDbType.VarChar).Value = address;
                        }
                        else
                        {
                            command.Parameters.Add("@address", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(gps))
                        {
                            command.Parameters.Add("@cgps", MySqlDbType.VarChar).Value = gps;
                        }
                        else
                        {
                            command.Parameters.Add("@cgps", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(airPicLoc))
                        {
                            command.Parameters.Add("@airPicLoc", MySqlDbType.VarChar).Value = airPicLoc;
                        }
                        else
                        {
                            command.Parameters.Add("@airPicLoc", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(dirPicLoc))
                        {
                            command.Parameters.Add("@dirPicLoc", MySqlDbType.VarChar).Value = dirPicLoc;
                        }
                        else
                        {
                            command.Parameters.Add("@dirPicLoc", MySqlDbType.VarChar).Value = DBNull.Value;
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

                        command.CommandText = @"INSERT INTO Cemeteries ( ";

                        command.CommandText += "CName, CCity, ";
                        values += "@Name, @City,";
                        command.Parameters.Add("@Name", MySqlDbType.VarChar).Value = name;
                        command.Parameters.Add("@City", MySqlDbType.VarChar).Value = city;

                        if (!string.IsNullOrEmpty(address))
                        {
                            command.CommandText += "CAddress, ";
                            values += "@address,";
                            command.Parameters.Add("@address", MySqlDbType.VarChar).Value = address;
                        }

                        if (!string.IsNullOrEmpty(gps))
                        {
                            command.CommandText += "GPS, ";
                            values += "@cgps,";
                            command.Parameters.Add("@cgps", MySqlDbType.VarChar).Value = gps;
                        }

                        if (!string.IsNullOrEmpty(airPicLoc))
                        {
                            command.CommandText += "CemAirPicLoc, ";
                            values += "@airPicLoc,";
                            command.Parameters.Add("@airPicLoc", MySqlDbType.VarChar).Value = airPicLoc;
                        }

                        if (!string.IsNullOrEmpty(dirPicLoc))
                        {
                            command.CommandText += "DirectionsPicLoc, ";
                            values += "@dirPicLoc,";
                            command.Parameters.Add("@dirPicLoc", MySqlDbType.VarChar).Value = dirPicLoc;
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

        // Loads member variables from the database for the set cemetery name and city
        private void LoadDataFromDatabase()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT CAddress, GPS, CemAirPicLoc, DirectionsPicLoc" +
                            " FROM Cemeteries WHERE CName = @Name AND CCity = @City;";
                        command.Parameters.Add("@Name", MySqlDbType.VarChar).Value = name;
                        command.Parameters.Add("@City", MySqlDbType.VarChar).Value = city;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    address = reader.GetString(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    gps = reader.GetString(1);
                                }

                                if (!reader.IsDBNull(2))
                                {
                                    airPicLoc = reader.GetString(2);
                                }

                                if (!reader.IsDBNull(3))
                                {
                                    dirPicLoc = reader.GetString(3);
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
                        command.CommandText = @"DELETE FROM Cemeteries WHERE CName = @Name AND CCity = @City;";
                        command.Parameters.Add("@Name", MySqlDbType.VarChar).Value = name;
                        command.Parameters.Add("@City", MySqlDbType.VarChar).Value = city;

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


        // Loads the cemeteries into a list of strings
        public static List<string> LoadStringList()
        {
            List<string> records = new List<string>();
            records.Add("()"); // Add a default blank cemetery

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT CName,CCity FROM Cemeteries;";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string cemDetails = "";

                                if (!reader.IsDBNull(0))
                                {
                                    cemDetails = reader.GetString(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    cemDetails += " (";
                                    cemDetails += reader.GetString(1);
                                    cemDetails += ")";
                                }

                                records.Add(cemDetails);
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

        // Loads the cemeteries into a list of objects
        public static List<CemeteryDBInfo> LoadObjectList()
        {
            List<CemeteryDBInfo> records = new List<CemeteryDBInfo>();
            CemeteryDBInfo current;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT CName,CCity FROM Cemeteries;";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string name = "";
                                string city = "";

                                if (!reader.IsDBNull(0))
                                {
                                    name = reader.GetString(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    city = reader.GetString(1);
                                }

                                current = new CemeteryDBInfo(name, city);
                                records.Add(current);
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
