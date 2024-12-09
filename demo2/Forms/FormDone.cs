using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace demo2.Forms
{
    public partial class FormDone : Form
    {
        private string connectionString = "Data Source=ReminderApp.db;";

        public FormDone()
        {
            InitializeComponent();
            LoadDone();

        }

        private void LoadDone()
        {
               // SQL query to select all tasks from the Task table
            string query = "SELECT Id, TaskName FROM Done";

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
                    dataGridView1.DataSource = null;
                    dataGridView1.Columns.Clear();

                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = taskTable;

                    // Set headers for the columns
                    dataGridView1.Columns["TaskName"].HeaderText = "Task";
                    

                    dataGridView1.Columns["Id"].Visible = false;
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the database operation
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // Event handler for deleting a task
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];

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
            string query = "DELETE FROM Done WHERE Id = @TaskID";

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
                    LoadDone();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }




        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {

        }
    }
}
