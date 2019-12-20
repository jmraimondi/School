using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Musuem_Viewer
{
    /// <summary>
    /// Interaction logic for UserComments.xaml
    /// </summary>
    public partial class UserComments : Window
    {
        private int veteranId;
        private string comment;

        public UserComments(int idNum)
        {
            InitializeComponent();

            WindowState = WindowState.Maximized;

            veteranId = idNum;
            comment = "";
        }

        // Checks for 3000 character limit
        private bool IsCommentLengthOk()
        {
            bool isValidLength = true;
            comment = TxtBox_Comment.Text;
            int commentLen = comment.Length;

            if (commentLen > 3000)
            {
                string message = "Maximum comment length is 3000 characters. This comment is " + commentLen + " characters.";
                string title = "Comment Length Exceeded";

                MessageBox.Show(message, title);

                isValidLength = false;
            }

            return isValidLength;
        }

        private void Btn_SaveClose_Click(object sender, RoutedEventArgs e)
        {
            // Check for 3000 character limit
            if (IsCommentLengthOk())
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString))
                    {
                        conn.OpenAsync();

                        using (MySqlCommand command = conn.CreateCommand())
                        {
                            command.CommandText = "INSERT INTO UserComments (ID, UserComment)" +
                                " VALUES (@idNum, @comment);";
                            command.Parameters.Add("@idNum", MySqlDbType.Int32).Value = veteranId;
                            command.Parameters.Add("@comment", MySqlDbType.VarChar).Value = comment;

                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show(Tools.DBErrorMessage, Tools.DBErrorTitle);
                }
                catch (MySqlException ex)
                {
                    Tools.HandleSQLExceptions(ex);
                }

                string message = "Thank you for your submission. It will be reviewed.";
                string title = "Submission Received";

                MessageBox.Show(message, title);

                Close();
            }
        }

        private void Btn_NoSaveClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
