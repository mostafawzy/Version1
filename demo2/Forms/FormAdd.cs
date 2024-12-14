using demo2.Helpers;
using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace demo2.Forms
{
    public partial class FormAdd : Form
    {
        private string connectionString = "Data Source=ReminderApp.db;";
        private int taskId;

        public FormAdd()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        public FormAdd(int taskId, string taskName, string taskDescription, DateTime reminder, string alarmSound)
        {
            InitializeComponent();
            this.taskId = taskId;
            richTextBox1.Text = taskName;
            richTextBox2.Text = taskDescription;
            dateTimePicker1.Value = reminder;
            comboBox1.SelectedItem = alarmSound ?? "Default";
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Task (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TaskName TEXT NOT NULL,
                Description TEXT NOT NULL,
                Reminder DATETIME,
                Passed TEXT DEFAULT 'No',
                UserId INTEGER,
                Sound TEXT,
                FOREIGN KEY (UserId) REFERENCES User(Id)
            );";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creating table: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            string taskName = richTextBox1.Text;
            string description = richTextBox2.Text;
            string selectedSound = comboBox1.SelectedItem?.ToString();
            DateTime reminder = dateTimePicker1.Value;
            int userId = SessionManager.Instance.LoggedInUserId;

            if (userId <= 0)
            {
                MessageBox.Show("Please log in first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(taskName) )
            {
                MessageBox.Show("Task name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

           
            if (reminder < DateTime.Now)
            {
                MessageBox.Show("You entered Past Time!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


         
            string reminderValue = reminder.ToString("yyyy-MM-dd HH:mm:ss");
            string passedValue = reminder < DateTime.Now ? "Yes" : "No";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query;
                SQLiteCommand command;

                if (taskId == 0)
                {
                    query = "INSERT INTO Task (TaskName, Description, Reminder, Passed, UserId, Sound) VALUES (@TaskName, @Description, @Reminder, @Passed, @UserId, @Sound)";
                    command = new SQLiteCommand(query, connection);
                }
                else
                {
                    query = "UPDATE Task SET TaskName = @TaskName, Description = @Description, Reminder = @Reminder, Passed = @Passed, UserId = @UserId, Sound = @Sound WHERE Id = @TaskId";
                    command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@TaskId", taskId);
                }

                command.Parameters.AddWithValue("@TaskName", taskName);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@Reminder", reminderValue);
                command.Parameters.AddWithValue("@Passed", passedValue);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Sound", selectedSound);


                command.ExecuteNonQuery();
            }

            MessageBox.Show("Task saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            richTextBox1.Clear();
            richTextBox2.Clear();
            dateTimePicker1.Value = DateTime.Now;
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
           
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }
       

    }
}
