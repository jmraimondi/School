using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;
using System.Text;

namespace Musuem_Viewer
{
    public class VeteranDBInfo
    {
        // All database interaction in this program:
        // Source: https://docs.microsoft.com/en-us/azure/mysql/connect-csharp
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient?view=netframework-4.8
        // And all Microsoft .Net references under the SQLClient classes

        public int Id { get; set; }
        private string firstName;
        private string middleName;
        private string lastName;
        private string suffix;
        private DateTime dob;
        private DateTime dod;
        public string CemName { get; set; }
        public string CemAddress { get; set; }
        public string CemGPS { get; set; }
        public string CemCity { get; set; }
        public string CemSection { get; set; }
        public string CemRow { get; set; }
        public string MarkerLocation { get; set; }
        public string MarkerPicLoc { get; set; }
        public string MilPicLoc { get; set; }
        public string CasualPicLoc { get; set; }
        public string MiscPicLoc { get; set; }
        public string CemDirectionsPicLoc { get; set; }
        public string CemAirPicLoc { get; set; }
        public string VetComments { get; set; }

        public string CemDetails
        {
            get
            {
                return CemName + " - " + CemCity;
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
        }

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

        // Constructor - Loads record from database
        public VeteranDBInfo(int idNum)
        {
            Id = idNum;
            LoadDataFromDatabase();
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
                        command.CommandText = "SELECT FName,MName,LName,Suffix,DOB,DOD,Veterans.CName,Veterans.CCity,CSection,CRow," +
                            "MarkerLocation,MarkerPicLoc,MilPicLoc,CasualPicLoc,MiscPicLoc,Comments,CAddress,GPS,DirectionsPicLoc,CemAirPicLoc " +
                            "FROM Veterans LEFT JOIN Cemeteries ON Veterans.CName = Cemeteries.CName AND Veterans.CCity = Cemeteries.CCity " +
                            "WHERE ID=@idNum;";
                        command.Parameters.Add("@idNum", MySqlDbType.Int32).Value = Id;

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
                                    CemName = reader.GetString(6);
                                }

                                if (!reader.IsDBNull(7))
                                {
                                    CemCity = reader.GetString(7);
                                }

                                if (!reader.IsDBNull(8))
                                {
                                    CemSection = reader.GetString(8);
                                }

                                if (!reader.IsDBNull(9))
                                {
                                    CemRow = reader.GetString(9);
                                }

                                if (!reader.IsDBNull(10))
                                {
                                    MarkerLocation = reader.GetString(10);
                                }

                                if (!reader.IsDBNull(11))
                                {
                                    MarkerPicLoc = reader.GetString(11);
                                }

                                if (!reader.IsDBNull(12))
                                {
                                    MilPicLoc = reader.GetString(12);
                                }

                                if (!reader.IsDBNull(13))
                                {
                                    CasualPicLoc = reader.GetString(13);
                                }

                                if (!reader.IsDBNull(14))
                                {
                                    MiscPicLoc = reader.GetString(14);
                                }

                                if (!reader.IsDBNull(15))
                                {
                                    VetComments = reader.GetString(15);
                                }

                                if (!reader.IsDBNull(16))
                                {
                                    CemAddress = reader.GetString(16);
                                }

                                if (!reader.IsDBNull(17))
                                {
                                    CemGPS = reader.GetString(17);
                                }

                                if (!reader.IsDBNull(18))
                                {
                                    CemDirectionsPicLoc = reader.GetString(18);
                                }

                                if (!reader.IsDBNull(19))
                                {
                                    CemAirPicLoc = reader.GetString(19);
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
    }
}
