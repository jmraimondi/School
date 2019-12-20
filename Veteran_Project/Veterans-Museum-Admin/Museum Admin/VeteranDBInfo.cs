using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;
using System.IO;

namespace Museum_Admin
{
    public class VeteranDBInfo
    {
        // All database interaction in this program:
        // Source: https://docs.microsoft.com/en-us/azure/mysql/connect-csharp
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient?view=netframework-4.8
        // And all Microsoft .Net references under the SQLClient classes

        // This class contains the overall record of a given veteran
        // It handles database loading and writing

        private int id;
        private string firstName;
        private string middleName;
        private string lastName;
        private string suffix;
        private DateTime dob;
        private DateTime dod;
        private string cemName;
        private string cemCity;
        private string cemSection;
        private string cemRow;
        private string markerLocation;
        private string markerPicLoc;
        private string milPicLoc;
        private string casualPicLoc;
        private string miscPicLoc;
        private string vetComments;
        private bool hasMarkerPicChanged = false;
        private string oldMarkerPic;
        private bool hasMilPicChanged = false;
        private string oldMilPic;
        private bool hasCasualPicChanged = false;
        private string oldCasualPic;
        private bool hasMiscPicChanged = false;
        private string oldMiscPic;

        public List<VetServiceDBInfo> ServiceDetails;
        public List<VetAwardDBInfo> AwardDetails;
        public List<VetConflictDBInfo> ConflictDetails;

        private bool hasDobChanged;
        private bool hasDodChanged;

        public string Name
        {
            get
            {
                string name = "";
                name += lastName;

                // Assuming if no first name, there is no middle name or suffix
                if (!string.IsNullOrEmpty(firstName))
                {
                    name += ", " + firstName;

                    if (!string.IsNullOrEmpty(middleName))
                    {
                        name += " " + middleName;
                    }

                    if (!string.IsNullOrEmpty(suffix))
                    {
                        name += ", " + suffix;
                    }
                }

                return name;
            }
        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                firstName = value;
                Tools.hasDataChanged = true;
            }
        }

        public string MiddleName
        {
            get
            {
                return middleName;
            }
            set
            {
                middleName = value;
                Tools.hasDataChanged = true;
            }
        }

        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                lastName = value;
                Tools.hasDataChanged = true;
            }
        }

        public string Suffix
        {
            get
            {
                return suffix;
            }
            set
            {
                suffix = value;
                Tools.hasDataChanged = true;
            }
        }

        public string Dob
        {
            get
            {
                string returnString;
                returnString = dob.ToShortDateString();

                // This is checking for a null value
                if (returnString == "1/1/0001")
                {
                    returnString = "";
                }

                return returnString;
            }
            set
            {
                if (value != "")
                {
                    hasDobChanged = DateTime.TryParse(value, out dob);
                    Tools.hasDataChanged = true;
                }
            }
        }

        public string Dod
        {
            get
            {
                string returnString;
                returnString = dod.ToShortDateString();

                // This is checking for a null value
                if (returnString == "1/1/0001")
                {
                    returnString = "";
                }

                return returnString;
            }
            set
            {
                if (value != "")
                {
                    hasDodChanged = DateTime.TryParse(value, out dod);
                    Tools.hasDataChanged = true;
                }
            }
        }

        public string CemDetails
        {
            get
            {
                return cemName + " (" + cemCity + ")";
            }
            set
            {
                Tools.hasDataChanged = true;

                Tools.CemDetailsParser(value, ref cemName, ref cemCity);
            }
        }

        public string CemSection
        {
            get
            {
                return cemSection;
            }
            set
            {
                cemSection = value;
                Tools.hasDataChanged = true;
            }
        }

        public string CemRow
        {
            get
            {
                return cemRow;
            }
            set
            {
                cemRow = value;
                Tools.hasDataChanged = true;
            }
        }

        public string MarkerLocation
        {
            get
            {
                return markerLocation;
            }
            set
            {
                markerLocation = value;
                Tools.hasDataChanged = true;
            }
        }

        public string MarkerPicLoc
        {
            get
            {
                return markerPicLoc;
            }
            set
            {
                oldMarkerPic = markerPicLoc;
                markerPicLoc = value;
                hasMarkerPicChanged = true;
                Tools.hasDataChanged = true;
            }
        }

        public string MilPicLoc
        {
            get
            {
                return milPicLoc;
            }
            set
            {
                oldMilPic = milPicLoc;
                milPicLoc = value;
                hasMilPicChanged = true;
                Tools.hasDataChanged = true;
            }
        }

        public string CasualPicLoc
        {
            get
            {
                return casualPicLoc;
            }
            set
            {
                oldCasualPic = casualPicLoc;
                casualPicLoc = value;
                hasCasualPicChanged = true;
                Tools.hasDataChanged = true;
            }
        }

        public string MiscPicLoc
        {
            get
            {
                return miscPicLoc;
            }
            set
            {
                oldMiscPic = miscPicLoc;
                miscPicLoc = value;
                hasMiscPicChanged = true;
                Tools.hasDataChanged = true;
            }
        }

        public string VetComments
        {
            get
            {
                return vetComments;
            }
            set
            {
                vetComments = value;
                Tools.hasDataChanged = true;
            }
        }

        // Constructor - New Record
        public VeteranDBInfo()
        {
            id = 0;
            hasDobChanged = false;
            hasDodChanged = false;

            ServiceDetails = new List<VetServiceDBInfo>();
            AwardDetails = new List<VetAwardDBInfo>();
            ConflictDetails = new List<VetConflictDBInfo>();
        }

        // Constructor - Loads record from database
        public VeteranDBInfo(int idNum)
        {
            id = idNum;
            hasDobChanged = false;
            hasDodChanged = false;
            LoadDataFromDatabase();
        }

        // Writes the contents of the member variables to the database for the set ID number
        // If no set ID number, retrieves the auto generated one from the database and stores it
        public void WriteDataToDatabase()
        {
            bool isNewId = false;

            // New record - Insert into database
            if (id == 0)
            {
                isNewId = true;
                InsertDataIntoDatabase();
            }

            bool hasAnyPicChanged = hasMarkerPicChanged || hasMilPicChanged || hasCasualPicChanged || hasMiscPicChanged;

            if (hasAnyPicChanged)
            {
                UpdateAllImages();

                hasMarkerPicChanged = false;
                hasMilPicChanged = false;
                hasCasualPicChanged = false;
                hasMiscPicChanged = false;
            }

            // Existing record or new record with pictures that needed ID to process then update - Update database
            if (!isNewId || hasAnyPicChanged)
            {
                UpdateDatabase();
            }

            // By this point, the ID is known from the database
            // Set vetIds on the lists
            if (isNewId)
            {
                foreach (VetServiceDBInfo record in ServiceDetails)
                {
                    record.SetVetId(id);
                }

                foreach (VetAwardDBInfo record in AwardDetails)
                {
                    record.SetVetId(id);
                }

                foreach (VetConflictDBInfo record in ConflictDetails)
                {
                    record.SetVetId(id);
                }
            }

            // Write the list data to database
            foreach (VetServiceDBInfo record in ServiceDetails)
            {
                record.WriteDataToDatabase();
            }

            foreach (VetAwardDBInfo record in AwardDetails)
            {
                record.WriteDataToDatabase();
            }

            foreach (VetConflictDBInfo record in ConflictDetails)
            {
                record.WriteDataToDatabase();
            }
        }

        // Calls UpdateImage for each of the images, if they have changed.
        private void UpdateAllImages()
        {
            if (hasMarkerPicChanged)
            {
                string folder;
                folder = ConfigurationManager.AppSettings["InstallDirectory"];
                folder += ConfigurationManager.AppSettings["MarkerPicDirectory"];

                markerPicLoc = UpdateImage(folder, oldMarkerPic, markerPicLoc);
            }

            if (hasMilPicChanged)
            {
                string folder;
                folder = ConfigurationManager.AppSettings["InstallDirectory"];
                folder += ConfigurationManager.AppSettings["MilPicDirectory"];

                milPicLoc = UpdateImage(folder, oldMilPic, milPicLoc);
            }

            if (hasCasualPicChanged)
            {
                string folder;
                folder = ConfigurationManager.AppSettings["InstallDirectory"];
                folder += ConfigurationManager.AppSettings["CasualPicDirectory"];

                casualPicLoc = UpdateImage(folder, oldCasualPic, casualPicLoc);
            }

            if (hasMiscPicChanged)
            {
                string folder;
                folder = ConfigurationManager.AppSettings["InstallDirectory"];
                folder += ConfigurationManager.AppSettings["MiscPicDirectory"];

                miscPicLoc = UpdateImage(folder, oldMiscPic, miscPicLoc);
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

                // Append ID number to front of filename to make sorting easier and prevent duplicate file names
                newPicFilename = id + " " + newPicFilename;

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

        // Loads the conflict details into a list
        private List<VetConflictDBInfo> LoadConflictDetails()
        {
            List<VetConflictDBInfo> records = new List<VetConflictDBInfo>();
            int conflictIdNum;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT CNum FROM Conflicts WHERE ID=@idNum;";
                        command.Parameters.Add("@idNum", MySqlDbType.Int32).Value = id;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    conflictIdNum = reader.GetInt32(0);
                                    VetConflictDBInfo record = new VetConflictDBInfo(conflictIdNum);
                                    records.Add(record);
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

        // Loads the award details into a list
        private List<VetAwardDBInfo> LoadAwardDetails()
        {
            List<VetAwardDBInfo> records = new List<VetAwardDBInfo>();
            int awardIdNum;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT ANum FROM Awards WHERE ID=@idNum;";
                        command.Parameters.Add("@idNum", MySqlDbType.Int32).Value = id;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    awardIdNum = reader.GetInt32(0);
                                    VetAwardDBInfo record = new VetAwardDBInfo(awardIdNum);
                                    records.Add(record);
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

        // Loads the service details into a list
        private List<VetServiceDBInfo> LoadServiceDetails()
        {
            List<VetServiceDBInfo> records = new List<VetServiceDBInfo>();
            int serviceIdNum;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT SNum FROM Services WHERE ID=@idNum;";
                        command.Parameters.Add("@idNum", MySqlDbType.Int32).Value = id;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    serviceIdNum = reader.GetInt32(0);
                                    VetServiceDBInfo record = new VetServiceDBInfo(serviceIdNum);
                                    records.Add(record);
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
                        // Write entire update statement, add parameters with data or null
                        command.CommandText = @"UPDATE Veterans SET FName = @firstName, MName = @middleName, LName = @lastName, " +
                        "Suffix = @suffix, DOB = @dob, DOD = @dod, CName = @cemName, CCity = @cemCity, CSection = @cemSection, " +
                        "CRow = @cemRow, MarkerLocation = @markerLoc, MarkerPicLoc = @markerPicLoc, MilPicLoc = @milPicLoc, " +
                        "CasualPicLoc = @casualPicLoc, MiscPicLoc = @miscPicLoc, Comments = @comments WHERE id = @idNum;";

                        if (!string.IsNullOrEmpty(firstName))
                        {
                            command.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = firstName;
                        }
                        else
                        {
                            command.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(middleName))
                        {
                            command.Parameters.Add("@middleName", MySqlDbType.VarChar).Value = middleName;
                        }
                        else
                        {
                            command.Parameters.Add("@middleName", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(lastName))
                        {
                            command.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = lastName;
                        }
                        else
                        {
                            command.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(suffix))
                        {
                            command.Parameters.Add("@suffix", MySqlDbType.VarChar).Value = suffix;
                        }
                        else
                        {
                            command.Parameters.Add("@suffix", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (hasDobChanged)
                        {
                            command.Parameters.Add("@dob", MySqlDbType.Date).Value = dob.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            command.Parameters.Add("@dob", MySqlDbType.Date).Value = DBNull.Value;
                        }

                        if (hasDodChanged)
                        {
                            command.Parameters.Add("@dod", MySqlDbType.Date).Value = dod.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            command.Parameters.Add("@dod", MySqlDbType.Date).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(cemName))
                        {
                            command.Parameters.Add("@cemName", MySqlDbType.VarChar).Value = cemName;
                        }
                        else
                        {
                            command.Parameters.Add("@cemName", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(cemCity))
                        {
                            command.Parameters.Add("@cemCity", MySqlDbType.VarChar).Value = cemCity;
                        }
                        else
                        {
                            command.Parameters.Add("@cemCity", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(cemSection))
                        {
                            command.Parameters.Add("@cemSection", MySqlDbType.VarChar).Value = cemSection;
                        }
                        else
                        {
                            command.Parameters.Add("@cemSection", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(cemRow))
                        {
                            command.Parameters.Add("@cemRow", MySqlDbType.VarChar).Value = cemRow;
                        }
                        else
                        {
                            command.Parameters.Add("@cemRow", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(markerLocation))
                        {
                            command.Parameters.Add("@markerLoc", MySqlDbType.VarChar).Value = markerLocation;
                        }
                        else
                        {
                            command.Parameters.Add("@markerLoc", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(markerPicLoc))
                        {
                            command.Parameters.Add("@markerPicLoc", MySqlDbType.VarChar).Value = markerPicLoc;
                        }
                        else
                        {
                            command.Parameters.Add("@markerPicLoc", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(milPicLoc))
                        {
                            command.Parameters.Add("@milPicLoc", MySqlDbType.VarChar).Value = milPicLoc;
                        }
                        else
                        {
                            command.Parameters.Add("@milPicLoc", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(casualPicLoc))
                        {
                            command.Parameters.Add("@casualPicLoc", MySqlDbType.VarChar).Value = casualPicLoc;
                        }
                        else
                        {
                            command.Parameters.Add("@casualPicLoc", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(miscPicLoc))
                        {
                            command.Parameters.Add("@miscPicLoc", MySqlDbType.VarChar).Value = miscPicLoc;
                        }
                        else
                        {
                            command.Parameters.Add("@miscPicLoc", MySqlDbType.VarChar).Value = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(vetComments))
                        {
                            command.Parameters.Add("@comments", MySqlDbType.VarChar, 750).Value = vetComments;
                        }
                        else
                        {
                            command.Parameters.Add("@comments", MySqlDbType.VarChar, 750).Value = DBNull.Value;
                        }

                        command.Parameters.Add("@idNum", MySqlDbType.Int32).Value = id;

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

        // Inserts a new record into the database, sets the ID number to the auto generated ID
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
                        string values = ") VALUES (";

                        command.CommandText = @"INSERT INTO Veterans (";

                        if (!string.IsNullOrEmpty(firstName))
                        {
                            command.CommandText += "FName, ";
                            values += "@firstName,";
                            command.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = firstName;
                        }

                        if (!string.IsNullOrEmpty(middleName))
                        {
                            command.CommandText += "MName, ";
                            values += "@middleName,";
                            command.Parameters.Add("@middleName", MySqlDbType.VarChar).Value = middleName;
                        }

                        if (!string.IsNullOrEmpty(lastName))
                        {
                            command.CommandText += "LName, ";
                            values += "@lastName,";
                            command.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = lastName;
                        }

                        if (!string.IsNullOrEmpty(suffix))
                        {
                            command.CommandText += "Suffix, ";
                            values += "@suffix,";
                            command.Parameters.Add("@suffix", MySqlDbType.VarChar).Value = suffix;
                        }

                        if (hasDobChanged)
                        {
                            command.CommandText += "DOB, ";
                            values += "@dob,";
                            command.Parameters.Add("@dob", MySqlDbType.Date).Value = dob.ToString("yyyy-MM-dd");
                        }

                        if (hasDodChanged)
                        {
                            command.CommandText += "DOD, ";
                            values += "@dod,";
                            command.Parameters.Add("@dod", MySqlDbType.Date).Value = dod.ToString("yyyy-MM-dd");
                        }

                        if (!string.IsNullOrEmpty(cemName))
                        {
                            command.CommandText += "CName, ";
                            values += "@cemName,";
                            command.Parameters.Add("@cemName", MySqlDbType.VarChar).Value = cemName;
                        }

                        if (!string.IsNullOrEmpty(cemCity))
                        {
                            command.CommandText += "CCity, ";
                            values += "@cemCity,";
                            command.Parameters.Add("@cemCity", MySqlDbType.VarChar).Value = cemCity;
                        }

                        if (!string.IsNullOrEmpty(cemSection))
                        {
                            command.CommandText += "CSection, ";
                            values += "@cemSection,";
                            command.Parameters.Add("@cemSection", MySqlDbType.VarChar).Value = cemSection;
                        }

                        if (!string.IsNullOrEmpty(cemRow))
                        {
                            command.CommandText += "CRow, ";
                            values += "@cemRow,";
                            command.Parameters.Add("@cemRow", MySqlDbType.VarChar).Value = cemRow;
                        }

                        if (!string.IsNullOrEmpty(markerLocation))
                        {
                            command.CommandText += "MarkerLocation, ";
                            values += "@markerLoc,";
                            command.Parameters.Add("@markerLoc", MySqlDbType.VarChar).Value = markerLocation;
                        }

                        // Do not do pictures on insert. Only on update to prepend with ID number.

                        if (!string.IsNullOrEmpty(vetComments))
                        {
                            command.CommandText += "Comments, ";
                            values += "@comments,";
                            command.Parameters.Add("@comments", MySqlDbType.VarChar, 750).Value = vetComments;
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
                        // We should always only create one record
                        else
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
                                id = reader.GetInt32(0);
                            }
                        }
                    }

                    // If we didn't successfully capture the ID number, have the user reopen the record
                    if (id == 0)
                    {
                        string message = "Error Retrieving ID Number. Close this record and reopen it.";
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
                        command.CommandText = "SELECT FName,MName,LName,Suffix,DOB,DOD,CName,CCity,CSection,CRow,MarkerLocation," +
                            "MarkerPicLoc,MilPicLoc,CasualPicLoc,MiscPicLoc,Comments FROM Veterans WHERE ID=@idNum;";
                        command.Parameters.Add("@idNum", MySqlDbType.Int32).Value = id;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    firstName = reader.GetString(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    middleName = reader.GetString(1);
                                }

                                if (!reader.IsDBNull(2))
                                {
                                    lastName = reader.GetString(2);
                                }

                                if (!reader.IsDBNull(3))
                                {
                                    suffix = reader.GetString(3);
                                }

                                if (!reader.IsDBNull(4))
                                {
                                    dob = reader.GetDateTime(4);
                                }

                                if (!reader.IsDBNull(5))
                                {
                                    dod = reader.GetDateTime(5);
                                }

                                if (!reader.IsDBNull(6))
                                {
                                    cemName = reader.GetString(6);
                                }

                                if (!reader.IsDBNull(7))
                                {
                                    cemCity = reader.GetString(7);
                                }

                                if (!reader.IsDBNull(8))
                                {
                                    cemSection = reader.GetString(8);
                                }

                                if (!reader.IsDBNull(9))
                                {
                                    cemRow = reader.GetString(9);
                                }

                                if (!reader.IsDBNull(10))
                                {
                                    markerLocation = reader.GetString(10);
                                }

                                if (!reader.IsDBNull(11))
                                {
                                    markerPicLoc = reader.GetString(11);
                                }

                                if (!reader.IsDBNull(12))
                                {
                                    milPicLoc = reader.GetString(12);
                                }

                                if (!reader.IsDBNull(13))
                                {
                                    casualPicLoc = reader.GetString(13);
                                }

                                if (!reader.IsDBNull(14))
                                {
                                    miscPicLoc = reader.GetString(14);
                                }

                                if (!reader.IsDBNull(15))
                                {
                                    vetComments = reader.GetString(15);
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

            // Load the list data from the database
            ServiceDetails = LoadServiceDetails();
            AwardDetails = LoadAwardDetails();
            ConflictDetails = LoadConflictDetails();
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
                        command.CommandText = @"DELETE FROM Veterans WHERE ID=@idNum;";
                        command.Parameters.Add("@idNum", MySqlDbType.Int32).Value = id;

                        int rowCount = command.ExecuteNonQuery();

                        if (rowCount == 1)
                        {
                            // User has now written the data to the database
                            Tools.hasDataChanged = false;
                        }
                        // We should always only delete one record
                        else
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
