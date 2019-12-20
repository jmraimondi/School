using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;
using System.IO;

namespace Museum_Admin
{
    public class QueryDBInfo
    {
        // All database interaction in this program:
        // Source: https://docs.microsoft.com/en-us/azure/mysql/connect-csharp
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient?view=netframework-4.8
        // And all Microsoft .Net references under the SQLClient classes

        // This class contains functions to run queries
        // It handles database loading - no writing

        public static int CountOfDatabase()
        {
            int results = 0;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT COUNT(*) FROM Veterans;";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    results = reader.GetInt32(0);
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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        public static int CountByCemetery(string selectedCemetery)
        {
            int results = 0;
            string city = "";
            string name = "";

            Tools.CemDetailsParser(selectedCemetery, ref name, ref city);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT COUNT(*) FROM Veterans WHERE CName=@name AND CCity=@city;";
                        command.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                        command.Parameters.Add("@city", MySqlDbType.VarChar).Value = city;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    results = reader.GetInt32(0);
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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        public static List<VeteranDBInfo> ListByCemetery(string selectedCemetery)
        {
            List<VeteranDBInfo> results = new List<VeteranDBInfo>();
            string city = "";
            string name = "";

            Tools.CemDetailsParser(selectedCemetery, ref name, ref city);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT FName,MName,LName,Suffix FROM Veterans WHERE CName=@name AND CCity=@city ORDER BY LName, FName;";
                        command.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                        command.Parameters.Add("@city", MySqlDbType.VarChar).Value = city;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VeteranDBInfo record = new VeteranDBInfo();

                                if (!reader.IsDBNull(0))
                                {
                                    record.FirstName = reader.GetString(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    record.MiddleName = reader.GetString(1);
                                }

                                if (!reader.IsDBNull(2))
                                {
                                    record.LastName = reader.GetString(2);
                                }

                                if (!reader.IsDBNull(3))
                                {
                                    record.Suffix = reader.GetString(3);
                                }

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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        // Returns a list of all cemeteries with a count of veterans in each one
        public static List<string> CountedCemeteryList()
        {
            List<string> results = new List<string>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT cemeteries.CName, cemeteries.CCity, COUNT(veterans.CName) FROM veterans RIGHT JOIN cemeteries " +
                            "ON veterans.CName=cemeteries.CName AND veterans.CCity=cemeteries.CCity GROUP BY cemeteries.CName, cemeteries.CCity";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string record = "";

                                if (!reader.IsDBNull(0))
                                {
                                    record += reader.GetString(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    record += " (";
                                    record += reader.GetString(1);
                                    record += ")";
                                }

                                if (!reader.IsDBNull(2))
                                {
                                    record += " Veteran Count: ";
                                    record += reader.GetInt32(2);
                                }

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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        public static int CountByBranch(string selectedBranch)
        {
            int results = 0;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT COUNT(*) FROM Services WHERE Branch=@BName;";
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = selectedBranch;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    results = reader.GetInt32(0);
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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        public static List<VeteranDBInfo> ListByBranch(string selectedBranch)
        {
            List<VeteranDBInfo> results = new List<VeteranDBInfo>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT FName,MName,LName,Suffix FROM Veterans NATURAL JOIN Services WHERE Branch=@BName ORDER BY LName, FName;";
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = selectedBranch;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VeteranDBInfo record = new VeteranDBInfo();

                                if (!reader.IsDBNull(0))
                                {
                                    record.FirstName = reader.GetString(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    record.MiddleName = reader.GetString(1);
                                }

                                if (!reader.IsDBNull(2))
                                {
                                    record.LastName = reader.GetString(2);
                                }

                                if (!reader.IsDBNull(3))
                                {
                                    record.Suffix = reader.GetString(3);
                                }

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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        // Returns a list of all branches with a count of veterans in each one
        public static List<string> CountedBranchList()
        {
            List<string> results = new List<string>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT BranchName, COUNT(Branch) FROM services RIGHT JOIN brancheslist " +
                            "ON branch=branchName GROUP BY branchName;";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string record = "";

                                if (!reader.IsDBNull(0))
                                {
                                    record += reader.GetString(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    record += " Veteran Count: ";
                                    record += reader.GetInt32(1);
                                }

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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        public static int CountByConflict(string selectedConflict)
        {
            int results = 0;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT COUNT(*) FROM Conflicts WHERE ConflictName=@CName;";
                        command.Parameters.Add("@CName", MySqlDbType.VarChar).Value = selectedConflict;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    results = reader.GetInt32(0);
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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        public static List<VeteranDBInfo> ListByConflict(string selectedConflict)
        {
            List<VeteranDBInfo> results = new List<VeteranDBInfo>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT FName,MName,LName,Suffix FROM Veterans NATURAL JOIN Conflicts WHERE ConflictName=@CName ORDER BY LName, FName;";
                        command.Parameters.Add("@CName", MySqlDbType.VarChar).Value = selectedConflict;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VeteranDBInfo record = new VeteranDBInfo();

                                if (!reader.IsDBNull(0))
                                {
                                    record.FirstName = reader.GetString(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    record.MiddleName = reader.GetString(1);
                                }

                                if (!reader.IsDBNull(2))
                                {
                                    record.LastName = reader.GetString(2);
                                }

                                if (!reader.IsDBNull(3))
                                {
                                    record.Suffix = reader.GetString(3);
                                }

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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        // Returns a list of all conflicts with a count of veterans in each one
        public static List<string> CountedConflictList()
        {
            List<string> results = new List<string>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT CLName, COUNT(ConflictName) FROM conflicts RIGHT JOIN conflictlist " +
                            "ON ConflictName=CLName GROUP BY CLName;";

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string record = "";

                                if (!reader.IsDBNull(0))
                                {
                                    record += reader.GetString(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    record += " Veteran Count: ";
                                    record += reader.GetInt32(1);
                                }

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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        public static int CountByBranchCem(string selectedBranch, string selectedCemetery)
        {
            int results = 0;
            string city = "";
            string name = "";

            Tools.CemDetailsParser(selectedCemetery, ref name, ref city);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT COUNT(*) FROM Veterans NATURAL JOIN services " + 
                            "WHERE CName = @name AND CCity = @city AND Branch = @BName;";
                        command.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                        command.Parameters.Add("@city", MySqlDbType.VarChar).Value = city;
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = selectedBranch;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    results = reader.GetInt32(0);
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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        public static List<VeteranDBInfo> ListByBranchCem(string selectedBranch, string selectedCemetery)
        {
            List<VeteranDBInfo> results = new List<VeteranDBInfo>();
            string city = "";
            string name = "";

            Tools.CemDetailsParser(selectedCemetery, ref name, ref city);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT FName,MName,LName,Suffix FROM Veterans NATURAL JOIN services " +
                            "WHERE CName = @name AND CCity = @city AND Branch = @BName ORDER BY LName, FName;";
                        command.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                        command.Parameters.Add("@city", MySqlDbType.VarChar).Value = city;
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = selectedBranch;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VeteranDBInfo record = new VeteranDBInfo();

                                if (!reader.IsDBNull(0))
                                {
                                    record.FirstName = reader.GetString(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    record.MiddleName = reader.GetString(1);
                                }

                                if (!reader.IsDBNull(2))
                                {
                                    record.LastName = reader.GetString(2);
                                }

                                if (!reader.IsDBNull(3))
                                {
                                    record.Suffix = reader.GetString(3);
                                }

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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        public static int CountByBranchConflict(string selectedBranch, string selectedConflict)
        {
            int results = 0;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT COUNT(*) FROM Conflicts NATURAL JOIN Veterans NATURAL JOIN Services " + 
                            "WHERE ConflictName=@CName AND Branch = @BName;";
                        command.Parameters.Add("@CName", MySqlDbType.VarChar).Value = selectedConflict;
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = selectedBranch;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    results = reader.GetInt32(0);
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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        public static List<VeteranDBInfo> ListByBranchConflict(string selectedBranch, string selectedConflict)
        {
            List<VeteranDBInfo> results = new List<VeteranDBInfo>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT FName,MName,LName,Suffix FROM Conflicts NATURAL JOIN Veterans NATURAL JOIN Services " + "" +
                            "WHERE ConflictName=@CName AND Branch = @BName ORDER BY LName, FName;";
                        command.Parameters.Add("@CName", MySqlDbType.VarChar).Value = selectedConflict;
                        command.Parameters.Add("@BName", MySqlDbType.VarChar).Value = selectedBranch;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VeteranDBInfo record = new VeteranDBInfo();

                                if (!reader.IsDBNull(0))
                                {
                                    record.FirstName = reader.GetString(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    record.MiddleName = reader.GetString(1);
                                }

                                if (!reader.IsDBNull(2))
                                {
                                    record.LastName = reader.GetString(2);
                                }

                                if (!reader.IsDBNull(3))
                                {
                                    record.Suffix = reader.GetString(3);
                                }

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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        public static int CountByConflictCem(string selectedConflict, string selectedCemetery)
        {
            int results = 0;
            string city = "";
            string name = "";

            Tools.CemDetailsParser(selectedCemetery, ref name, ref city);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT COUNT(*) FROM Veterans NATURAL JOIN Conflicts " +
                            "WHERE CName = @name AND CCity = @city AND ConflictName=@CName;";
                        command.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                        command.Parameters.Add("@city", MySqlDbType.VarChar).Value = city;
                        command.Parameters.Add("@CName", MySqlDbType.VarChar).Value = selectedConflict;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    results = reader.GetInt32(0);
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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }

        public static List<VeteranDBInfo> ListByConflictCem(string selectedConflict, string selectedCemetery)
        {
            List<VeteranDBInfo> results = new List<VeteranDBInfo>();
            string city = "";
            string name = "";

            Tools.CemDetailsParser(selectedCemetery, ref name, ref city);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                {
                    conn.OpenAsync();

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT FName,MName,LName,Suffix FROM Veterans NATURAL JOIN Conflicts " +
                            "WHERE CName = @name AND CCity = @city AND ConflictName=@CName ORDER BY LName, FName;";
                        command.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                        command.Parameters.Add("@city", MySqlDbType.VarChar).Value = city;
                        command.Parameters.Add("@CName", MySqlDbType.VarChar).Value = selectedConflict;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VeteranDBInfo record = new VeteranDBInfo();

                                if (!reader.IsDBNull(0))
                                {
                                    record.FirstName = reader.GetString(0);
                                }

                                if (!reader.IsDBNull(1))
                                {
                                    record.MiddleName = reader.GetString(1);
                                }

                                if (!reader.IsDBNull(2))
                                {
                                    record.LastName = reader.GetString(2);
                                }

                                if (!reader.IsDBNull(3))
                                {
                                    record.Suffix = reader.GetString(3);
                                }

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

            // Queries never change data in the database
            Tools.hasDataChanged = false;

            return results;
        }
    }
}
