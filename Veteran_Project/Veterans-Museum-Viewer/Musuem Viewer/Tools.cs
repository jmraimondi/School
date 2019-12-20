using System;
using System.Text;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Musuem_Viewer
{
    class Tools
    {
        // This class contains random items for use throughout the program

        // All database interaction in this program:
        // Source: https://docs.microsoft.com/en-us/azure/mysql/connect-csharp
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient?view=netframework-4.8
        // And all Microsoft .Net references under the SQLClient classes

        public const string DBErrorMessage = "MySQL server not found. Restart MariaDB.";
        public const string DBErrorTitle = "SQL Error";

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
    }
}
