using demo2.Helpers;
using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace demo2.Forms
{
    public partial class FormList : Form
    {
        private string connectionString = "Data Source=ReminderApp.db;";

        public FormList()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadTasks();
        }

        private void LoadTasks()
        {
            string query = "SELECT Id, TaskName, Description, Reminder, Passed FROM Task WHERE UserId = @UserId";
            DataTable taskTable = new DataTable();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    int userId = SessionManager.Instance.LoggedInUserId;
                    command.Parameters.AddWithValue("@UserId", userId);
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                    dataAdapter.Fill(taskTable);
                    dataGridViewTasks.DataSource = null;
                    dataGridViewTasks.Columns.Clear();
                    dataGridViewTasks.DataSource = taskTable;
                    dataGridViewTasks.Columns["TaskName"].HeaderText = "Task";
                    dataGridViewTasks.Columns["Description"].HeaderText = "Description";
                    dataGridViewTasks.Columns["Reminder"].HeaderText = "Reminder";
                    dataGridViewTasks.Columns["Passed"].HeaderText = "Passed";
                    dataGridViewTasks.Columns["Id"].Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewTasks.SelectedRows.Count > 0)
            {
                int selectedRowIndex = dataGridViewTasks.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridViewTasks.Rows[selectedRowIndex];
                int id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                string taskName = selectedRow.Cells["TaskName"].Value.ToString();
                string taskDescription = selectedRow.Cells["Description"].Value.ToString();
                DateTime reminder = Convert.ToDateTime(selectedRow.Cells["Reminder"].Value);
                FormAdd formAdd = new FormAdd(id, taskName, taskDescription, reminder);
                formAdd.ShowDialog();
                LoadTasks();
            }
            else
            {
                MessageBox.Show("Please select a task to edit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewTasks.SelectedRows.Count > 0)
            {
                int selectedRowIndex = dataGridViewTasks.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridViewTasks.Rows[selectedRowIndex];
                int taskID = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                DeleteTask(taskID);
            }
            else
            {
                MessageBox.Show("Please select a task to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteTask(int taskID)
        {
            string query = "DELETE FROM Task WHERE Id = @TaskID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@TaskID", taskID);
                    command.ExecuteNonQuery();
                    LoadTasks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { }

        private void dataGridViewTasks_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (dataGridViewTasks.SelectedRows.Count > 0)
            {
                int selectedRowIndex = dataGridViewTasks.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridViewTasks.Rows[selectedRowIndex];
                int id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                string taskName = selectedRow.Cells["TaskName"].Value.ToString();
                AddTaskToDone(id, taskName);
                LoadTasks();
                DeleteTask(id);
            }
            else
            {
                MessageBox.Show("Please select a task to mark as done.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddTaskToDone(int taskId, string taskName)
        {
            int userId = SessionManager.Instance.LoggedInUserId;

            if (userId == 0)
            {
                MessageBox.Show("User not logged in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "INSERT INTO Done (TaskId, TaskName, UserId) VALUES (@TaskId, @TaskName, @UserId)";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@TaskId", taskId);
                    command.Parameters.AddWithValue("@TaskName", taskName);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = @"
                     CREATE TABLE IF NOT EXISTS Done (
                         Id INTEGER PRIMARY KEY AUTOINCREMENT,
                         TaskId INTEGER,
                         TaskName TEXT,
                         UserId INTEGER,
                         FOREIGN KEY(TaskId) REFERENCES Task(Id) ON DELETE CASCADE,
                         FOREIGN KEY (UserId) REFERENCES User(Id)
                     );";

                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
