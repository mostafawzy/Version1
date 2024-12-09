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
            // SQL query to select all tasks from the Task table
            string query = "SELECT Id, TaskName, Description, Reminder FROM Task";

            // Create a new DataTable to hold the data
            DataTable taskTable = new DataTable();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Create a SQLiteDataAdapter to execute the query and fill the DataTable
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, connection);

                    // Fill the DataTable with the results of the query
                    dataAdapter.Fill(taskTable);

                    // Clear the previous data and columns from the DataGridView
                    dataGridViewTasks.DataSource = null;
                    dataGridViewTasks.Columns.Clear();

                    // Bind the DataTable to the DataGridView
                    dataGridViewTasks.DataSource = taskTable;

                    // Set headers for the columns
                    dataGridViewTasks.Columns["TaskName"].HeaderText = "Task";
                    dataGridViewTasks.Columns["Description"].HeaderText = "Description";
                    dataGridViewTasks.Columns["Reminder"].HeaderText = "Reminder";

                    dataGridViewTasks.Columns["Id"].Visible = false;
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the database operation
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // Event handler for editing a task
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewTasks.SelectedRows.Count > 0)
            {
                // Get the selected row
                int selectedRowIndex = dataGridViewTasks.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridViewTasks.Rows[selectedRowIndex];

                // Retrieve the task ID and other details from the selected row
                int id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                string taskName = selectedRow.Cells["TaskName"].Value.ToString();
                string taskDescription = selectedRow.Cells["Description"].Value.ToString();
                //int reminder = Convert.ToInt32(selectedRow.Cells["Reminder"].Value);
                DateTime reminder = Convert.ToDateTime(selectedRow.Cells["Reminder"].Value);

                // Open the edit form and pass the task details for editing
                FormAdd formAdd = new FormAdd(id, taskName, taskDescription, reminder);
                formAdd.ShowDialog();

                // Refresh the DataGridView after editing
                LoadTasks();
            }
            else
            {
                MessageBox.Show("Please select a task to edit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event handler for deleting a task
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewTasks.SelectedRows.Count > 0)
            {
                // Get the selected row
                int selectedRowIndex = dataGridViewTasks.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridViewTasks.Rows[selectedRowIndex];

                // Retrieve the TaskID
                int taskID = Convert.ToInt32(selectedRow.Cells["Id"].Value);

                // Delete the task from the database
                DeleteTask(taskID);
            }
            else
            {
                MessageBox.Show("Please select a task to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to delete a task from the database
        private void DeleteTask(int taskID)
        {
            string query = "DELETE FROM Task WHERE Id = @TaskID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Create a SQLiteCommand to execute the delete query
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@TaskID", taskID);

                    // Execute the command
                    command.ExecuteNonQuery();

                    // Refresh the DataGridView after deletion
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
                // Get the selected row
                int selectedRowIndex = dataGridViewTasks.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridViewTasks.Rows[selectedRowIndex];

                // Retrieve the task details from the selected row
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
            string query = "INSERT INTO Done (TaskId, TaskName) VALUES (@TaskId, @TaskName)";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@TaskId", taskId);
                    command.Parameters.AddWithValue("@TaskName", taskName);

                    
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
                TaskId INTEGER ,
                TaskName TEXT ,
                
                FOREIGN KEY(TaskId) REFERENCES Task(Id) ON DELETE CASCADE
            );";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }



    }
}
