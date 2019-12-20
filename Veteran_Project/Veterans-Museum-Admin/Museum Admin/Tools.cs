using System;
using System.Text;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Museum_Admin
{
    public class Tools
    {
        // This class contains random items for use throughout the program

        // All database interaction in this program:
        // Source: https://docs.microsoft.com/en-us/azure/mysql/connect-csharp
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient?view=netframework-4.8
        // And all Microsoft .Net references under the SQLClient classes

        public const string DBErrorMessage = "MySQL server not found. Restart MariaDB.";
        public const string DBErrorTitle = "SQL Error";

        public const string unsavedMessage = "Unsaved changes. Are you sure you want to leave the page?";
        public const string unsavedClosingMessage = "Unsaved changes. Are you sure you want to close the program?";
        public const string unsavedTitle = "Unsaved Changes Detected";

        public const string deleteMessage = "Are you sure you want to delete this record?";
        public const string deleteTitle = "Delete Confirmation";

        public const string deleteCommentMessage = "Are you sure you want to delete this comment?";

        public const string fileMissingMessage = "File not found. Please try loading file again.";
        public const string fileMissingTitle = "File Not Found";

        public const string directoryMissingMessage = "Directory not found. Please try loading file again.";
        public const string directoryMissingTitle = "Directory Not Found";

        public const string RecordSelectMessage = "Please select a record to view.";
        public const string RecordDeleteMessage = "Please select a record to delete.";
        public const string RecordSelectTitle = "Record Selection Error";

        public static bool hasDataChanged { get; set; }

        // Handle SQL Exceptions
        public static void HandleSQLExceptions(MySqlException e)
        {
            StringBuilder errorBuilder = new StringBuilder();
            errorBuilder.Append("Message: " + e.Message + "\n" +
                                "Error Number: " + e.Number + "\n" +
                                "Source: " + e.Source + "\n");

            string errorMessage;
            errorMessage = errorBuilder.ToString();

            string title = "SQL Error";

            MessageBox.Show(errorMessage, title);
        }

        // Builds a bitmap image and returns it without keeping the file open
        public static BitmapImage LoadBitmap(string path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.EndInit();

            return bitmap;
        }

        // Separate CemName from CemCity in "CemName (CemCity)"
        public static void CemDetailsParser(string details, ref string name, ref string city)
        {
            int openParend = details.IndexOf("(");
            city = details.Substring(openParend + 1);
            city = city.Remove(city.Length - 1);
            name = details.Remove(openParend - 1);
        }
    }
}
