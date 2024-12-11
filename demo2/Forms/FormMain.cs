using demo2.Forms;
using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace demo2
{
    public partial class FormMain : Form
    {
        private string connectionString = "Data Source=ReminderApp.db;";
        private IconButton currentBtn;
        private Panel leftBorderBtn;
        private Form currentChildForm;
        private System.Windows.Forms.Timer reminderTimer;
        private Timer dateTimeTimer;
        private Timer reminderStopTimer;
        private System.Media.SoundPlayer soundPlayer;

        public FormMain()
        {
            InitializeComponent();
            InitializeDatabase();
            InitializeDateTimeUpdater();
            dateTimeTimer.Start();
            InitializeReminderChecker();
            this.StartPosition = FormStartPosition.CenterScreen;
            leftBorderBtn = new Panel();
            leftBorderBtn.Size = new Size(7, 60);
            panel2.Controls.Add(leftBorderBtn);
            this.Text = string.Empty;
            this.DoubleBuffered = true;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            reminderTimer = new System.Windows.Forms.Timer();
            reminderTimer.Interval = 1000;
            reminderStopTimer = new Timer { Interval = 60000 };
            reminderStopTimer.Tick += ReminderStopTimer_Tick;
        }

        private void ReminderStopTimer_Tick(object sender, EventArgs e)
        {
            reminderTimer.Stop();
            reminderStopTimer.Stop();
        }

        private struct RGBColors
        {
            public static Color color1 = ColorTranslator.FromHtml("#fadbd8");
            public static Color color2 = ColorTranslator.FromHtml("#f8edc8");
        }

        private void ActivateButton(object senderBtn, Color color)
        {
            if (senderBtn != null)
            {
                DisableButton();
                currentBtn = (IconButton)senderBtn;
                currentBtn.BackColor = ColorTranslator.FromHtml("#69A095");
                currentBtn.ForeColor = color;
                currentBtn.IconColor = color;
                currentBtn.Padding = new Padding(20, 0, 0, 0);
                leftBorderBtn.BackColor = color;
                leftBorderBtn.Location = new Point(0, currentBtn.Location.Y);
                leftBorderBtn.Visible = true;
                leftBorderBtn.BringToFront();
                iconButton6.IconChar = currentBtn.IconChar;
                iconButton6.IconColor = color;
                titleChildForm.Text = currentBtn.Text;
            }
        }

        private void DisableButton()
        {
            if (currentBtn != null)
            {
                currentBtn.BackColor = ColorTranslator.FromHtml("#A5C4BD");
                currentBtn.ForeColor = ColorTranslator.FromHtml("#f6f4f0");
                currentBtn.IconColor = ColorTranslator.FromHtml("#f6f4f0");
                currentBtn.Padding = new Padding(10, 0, 10, 0);
            }
        }

        private void openChildForm(Form childForm)
        {
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }
            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            childForm.Size = panelDesktop.Size;
            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color1);
            currentChildForm.Close();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color2);
            openChildForm(new FormAdd());
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color1);
            openChildForm(new FormList());
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color2);
            openChildForm(new FormDone());
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color2);
           
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color1);
            openChildForm(new FormLogin());
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            currentChildForm.Close();
            Reset();
        }

        private void Reset()
        {
            DisableButton();
            leftBorderBtn.Visible = false;
            iconButton6.IconChar = IconChar.ClockFour;
            iconButton6.IconColor = ColorTranslator.FromHtml("#f6f4f0");
            titleChildForm.Text = "TaskMinder";
        }

        private void InitializeDateTimeUpdater()
        {
            dateTimeTimer = new Timer { Interval = 1000 };
            dateTimeTimer.Tick += DateTimeTimer_Tick;
        }

        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            maskedTextBox1.Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
        }

        private void InitializeReminderChecker()
        {
            reminderTimer = new Timer { Interval = 1000 };
            reminderTimer.Tick += ReminderTimer_Tick;
            reminderTimer.Start();
        }

        private void ReminderTimer_Tick(object sender, EventArgs e)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, TaskName, Reminder, Passed FROM Task WHERE Reminder <= @CurrentTime AND Passed = 'No'";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CurrentTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    using (var reader = command.ExecuteReader())
                    {
                        var taskIdsToUpdate = new List<int>();

                        while (reader.Read())
                        {
                            int taskId = Convert.ToInt32(reader["Id"]);
                            string taskName = reader["TaskName"].ToString();
                            DateTime reminderTime = DateTime.Parse(reader["Reminder"].ToString());
                            DateTime currentTime = DateTime.Now;

                            if (currentTime >= reminderTime && currentTime < reminderTime.AddSeconds(1))
                            {
                                try
                                {
                                    if (soundPlayer == null)
                                    {
                                        soundPlayer = new System.Media.SoundPlayer
                                        {
                                            SoundLocation = @"C:\Users\Sanag\Downloads\12345.wav"
                                        };
                                    }
                                    soundPlayer.PlayLooping();
                                    MessageBox.Show($"Reminder: {taskName} is due at {reminderTime}.", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    soundPlayer.Stop();
                                    taskIdsToUpdate.Add(taskId);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"An error occurred while showing the reminder: {ex.Message}");
                                }
                            }
                        }

                        if (taskIdsToUpdate.Count > 0)
                        {
                            UpdatePassedColumn(connection, taskIdsToUpdate);
                        }
                    }
                }
            }
        }

        private void UpdatePassedColumn(SQLiteConnection connection, List<int> taskIds)
        {
            string updateQuery = "UPDATE Task SET Passed = 'Yes' WHERE Id = @Id";
            using (var command = new SQLiteCommand(updateQuery, connection))
            {
                foreach (var taskId in taskIds)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Id", taskId);
                    command.ExecuteNonQuery();
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            dateTimeTimer.Stop();
            base.OnFormClosing(e);
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

        private void FormMain_Load(object sender, EventArgs e) { }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e) { }
        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e) { }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
