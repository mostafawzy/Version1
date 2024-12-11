using demo2.Helpers;
using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace demo2.Forms
{
    public partial class FormLogin : Form
    {
        
        private string connectionString = "Data Source=ReminderApp.db;";

        public FormLogin()
        {
            InitializeComponent();
            InitializeDatabase();
            this.Text = string.Empty;
            this.ControlBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string username = textBox1.Text;
            string password = textBox2.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill out both fields.");
                return;
            }

            if (ValidateLogin(username, password))
            {
                MessageBox.Show("Login successful!", "Success");
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Error");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Register Button Action
            string username = textBox1.Text;
            string password = textBox2.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill out both fields.");
                return;
            }

            if (registerUser(username, password))
            {
                MessageBox.Show("Registration successful!");
            }
            else
            {
                MessageBox.Show("Username already exists.");
            }
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE NOT NULL,
                        Password TEXT NOT NULL
                    );";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private bool registerUser(string username, string password)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (var checkCmd = new SQLiteCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@Username", username);
                    long count = (long)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        return false;
                    }
                }

                string insertQuery = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
                using (var cmd = new SQLiteCommand(insertQuery, connection))
                {
                    string hashedPassword = HashPassword(password);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        // private bool ValidateLogin(string username, string password)
        // {
        //     using (var connection = new SQLiteConnection(connectionString))
        //     {
        //         connection.Open();
        //
        //         string query = "SELECT Password FROM Users WHERE Username = @Username";
        //         using (var command = new SQLiteCommand(query, connection))
        //         {
        //             command.Parameters.AddWithValue("@Username", username);
        //             var result = command.ExecuteScalar();
        //
        //             if (result != null)
        //             {
        //                 string storedHashedPassword = result.ToString();
        //                 return VerifyPassword(password, storedHashedPassword);
        //             }
        //         }
        //     }
        //     return false;
        // }
        private int loggedInUserId;

        private bool ValidateLogin(string username, string password)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Password FROM Users WHERE Username = @Username";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string storedHashedPassword = reader["Password"].ToString();
                        if (VerifyPassword(password, storedHashedPassword))
                        {
                            int userId = Convert.ToInt32(reader["Id"]);
                            SessionManager.Instance.SetUser(userId, username); // Set session data
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            string hashedEnteredPassword = HashPassword(enteredPassword);
            return hashedEnteredPassword == storedHashedPassword;

        }


private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
