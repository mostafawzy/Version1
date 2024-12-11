using demo2.Helpers;
using System;
using System.Data;
using System.Data.SQLite;
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
            string query = "SELECT Id, TaskName ,UserId FROM Done  WHERE UserId = @UserId";
            DataTable taskTable = new DataTable();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, connection);
                    int userId = SessionManager.Instance.LoggedInUserId;
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@UserId", userId);
                    dataAdapter.Fill(taskTable);

                    dataGridView1.DataSource = null;
                    dataGridView1.Columns.Clear();
                    dataGridView1.DataSource = taskTable;

                    dataGridView1.Columns["TaskName"].HeaderText = "Task";
                    dataGridView1.Columns["Id"].Visible = false;
                    dataGridView1.Columns["UserId"].Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];
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
            string query = "DELETE FROM Done WHERE Id = @TaskID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@TaskID", taskID);
                    command.ExecuteNonQuery();
                    LoadDone();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {}
    }
}
