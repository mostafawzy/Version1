using demo2.Forms;
using demo2.Helpers;
using FontAwesome.Sharp;
using System;
using NAudio.Wave;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace demo2
{
    public partial class FormMain : Form
    {
        private string connectionString = "Data Source=ReminderApp.db;";
        private IconButton currentBtn;
        private Panel leftBorderBtn;
        private Form currentChildForm;
        private System.Windows.Forms.Timer reminderTimer;
        private new System.Windows.Forms.Timer dateTimeTimer;
        private new System.Windows.Forms.Timer reminderStopTimer;
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
            reminderStopTimer =  new System.Windows.Forms.Timer { Interval = 60000 };
            reminderStopTimer.Tick += ReminderStopTimer_Tick;
            this.MaximumSize = new Size(1100, 975);
        }
        private SemaphoreSlim threadLimiter = new SemaphoreSlim(5); 


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

        private void iconButton6_Click(object sender, EventArgs e){}

        private void iconButton5_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color1);

            if (SessionManager.Instance.LoggedInUserId == 0) 
            {
                var loginForm = new FormLogin();
                loginForm.OnLoginSuccessCallback = UpdateButtonText; 
                openChildForm(loginForm);
            }
            else 
            {
                SessionManager.Instance.ClearSession();
                MessageBox.Show("See you soon. Your tasks are waiting!");
                UpdateButtonText();
            }
        }

        private void UpdateButtonText()
        {
            if (SessionManager.Instance.LoggedInUserId == 0)
            {
                this.iconButton5.Text = "Login";
                this.iconButton5.IconChar = FontAwesome.Sharp.IconChar.FaceGrin;
            }
            else
            {
                this.iconButton5.Text = "Logout";
                this.iconButton5.IconChar = IconChar.RightFromBracket;
            }
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
            dateTimeTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            dateTimeTimer.Tick += DateTimeTimer_Tick;
        }

        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            maskedTextBox1.Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
        }

        private void InitializeReminderChecker()
        {
            reminderTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            reminderTimer.Tick += ReminderTimer_Tick;
            reminderTimer.Start();
        }


        //...................... WithOut_Threads................................

        //private void ReminderTimer_Tick(object sender, EventArgs e)
        //{
        //    DateTime currentTime = DateTime.Now;
        //    using (var connection = new SQLiteConnection(connectionString))
        //    {
        //        connection.Open();
        //        string query = "SELECT Id, TaskName, Reminder, Passed ,Sound FROM Task WHERE Reminder <= @CurrentTime AND Passed = 'No'";
        //        using (var command = new SQLiteCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@CurrentTime", currentTime);
        //            using (var reader = command.ExecuteReader())
        //            {
        //                var taskIdsToUpdate = new List<int>();

        //                while (reader.Read())
        //                {
        //                    int taskId = Convert.ToInt32(reader["Id"]);
        //                    string taskName = reader["TaskName"].ToString();
        //                    DateTime reminderTime = DateTime.Parse(reader["Reminder"].ToString());
        //                    string soundFileName = reader["Sound"].ToString();
        //                    // DateTime currentTime = DateTime.Now;

        //                    if (currentTime >= reminderTime && currentTime < reminderTime.AddSeconds(1))
        //                    {
        //                        try
        //                        {
        //                            // Create a new SoundPlayer instance for each alarm
        //                            using (var player = new System.Media.SoundPlayer
        //                            {
        //                                SoundLocation = $@"D:\SaSa\icons\alarmTones\{soundFileName}.wav"
        //                                //SoundLocation = $@"D:\SaSa\icons\alarmTones\Tone1.wav"
        //                            })
        //                            {
        //                                player.PlayLooping();
        //                                MessageBox.Show(
        //                                    $"Reminder: {taskName} is due at {reminderTime}.\n{GetThreadInfo(Thread.CurrentThread)}",
        //                                    "Reminder",
        //                                    MessageBoxButtons.OK,
        //                                    MessageBoxIcon.Information);
        //                                player.Stop();
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            MessageBox.Show($"An error occurred while showing the reminder: {ex.Message}");
        //                        }
        //                        taskIdsToUpdate.Add(taskId);
        //                    }
        //                }

        //                if (taskIdsToUpdate.Count > 0)
        //                {
        //                    UpdatePassedColumn(connection, taskIdsToUpdate);
        //                }
        //            }
        //        }
        //    }
        //}
        //private void UpdatePassedColumn(SQLiteConnection connection, List<int> taskIds)
        //{
        //    string updateQuery = "UPDATE Task SET Passed ='Yes' WHERE Id = @Id";
        //    using (var command = new SQLiteCommand(updateQuery, connection))
        //    {
        //        foreach (var taskId in taskIds)
        //        {
        //            command.Parameters.Clear();
        //            command.Parameters.AddWithValue("@Id", taskId);
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}


        //.......................Using Task...............

        //private async void ReminderTimer_Tick(object sender, EventArgs e)
        //{
        //    using (var connection = new SQLiteConnection(connectionString))
        //    {
        //        await connection.OpenAsync();
        //        string query = "SELECT Id, TaskName, Reminder, Passed, Sound FROM Task WHERE Reminder <= @CurrentTime AND Passed = 'No'";
        //        using (var command = new SQLiteCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@CurrentTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                var taskIdsToUpdate = new List<int>();
        //                var tasks = new List<Task>();

        //                while (await reader.ReadAsync())
        //                {
        //                    int taskId = Convert.ToInt32(reader["Id"]);
        //                    string taskName = reader["TaskName"].ToString();
        //                    DateTime reminderTime = DateTime.Parse(reader["Reminder"].ToString());
        //                    string soundFileName = reader["Sound"].ToString();
        //                    DateTime currentTime = DateTime.Now;

        //                    if (currentTime >= reminderTime && currentTime < reminderTime.AddSeconds(1))
        //                    {
        //                        tasks.Add(Task.Run(() =>
        //                        {
        //                            try
        //                            {
        //                                var stopFlag = new ManualResetEvent(false);

        //                                // Run PlaySound in a separate thread to avoid blocking
        //                                Task.Run(() => PlaySound($@"D:\SaSa\icons\alarmTones\{soundFileName}.wav", stopFlag));

        //                                // Show MessageBox (runs on this thread)
        //                                MessageBox.Show(
        //                                    $"Reminder: {taskName} is due at {reminderTime}.\n{GetThreadInfo(Thread.CurrentThread)}",
        //                                    "Reminder",
        //                                    MessageBoxButtons.OK,
        //                                    MessageBoxIcon.Information);

        //                                stopFlag.Set();
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                MessageBox.Show($"An error occurred while showing the reminder: {ex.Message}");
        //                            }
        //                        }));

        //                        taskIdsToUpdate.Add(taskId);
        //                    }
        //                }

        //                await Task.WhenAll(tasks); // Wait for all tasks to finish

        //                if (taskIdsToUpdate.Count > 0)
        //                {
        //                    await UpdatePassedColumnAsync(connection, taskIdsToUpdate);
        //                }
        //            }
        //        }
        //    }
        //}




      //  inputThreads
        private async void ReminderTimer_Tick(object sender, EventArgs e)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Id, TaskName, Reminder, Passed, Sound FROM Task WHERE Reminder <= @CurrentTime AND Passed = 'No'";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CurrentTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var taskIdsToUpdate = new List<int>();
                        var tasks = new List<Task>();

                        while (await reader.ReadAsync())
                        {
                            int taskId = Convert.ToInt32(reader["Id"]);
                            string taskName = reader["TaskName"].ToString();
                            DateTime reminderTime = DateTime.Parse(reader["Reminder"].ToString());
                            string soundFileName = reader["Sound"].ToString();
                            DateTime currentTime = DateTime.Now;

                            if (currentTime >= reminderTime && currentTime < reminderTime.AddSeconds(1))
                            {
                                tasks.Add(Task.Run(async () =>
                                {
                                    await threadLimiter.WaitAsync();

                                    try
                                    {
                                        var stopFlag = new ManualResetEvent(false);

                                        Task.Run(() => PlaySound($@"D:\SaSa\icons\alarmTones\{soundFileName}.wav", stopFlag));

                                        MessageBox.Show(
                                            $"Reminder: {taskName} is due at {reminderTime}.\n{GetThreadInfo(Thread.CurrentThread)}",
                                            "Reminder",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);

                                        stopFlag.Set();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    finally
                                    {
                                        threadLimiter.Release();
                                    }
                                }));

                                taskIdsToUpdate.Add(taskId);
                            }
                        }

                        await Task.WhenAll(tasks);

                        if (taskIdsToUpdate.Count > 0)
                        {
                            await UpdatePassedColumnAsync(connection, taskIdsToUpdate);
                        }
                    }
                }
            }
        }



        private async Task UpdatePassedColumnAsync(SQLiteConnection connection, List<int> taskIds)
        {
            string updateQuery = "UPDATE Task SET Passed = 'Yes' WHERE Id = @Id";
            using (var command = new SQLiteCommand(updateQuery, connection))
            {
                foreach (var taskId in taskIds)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Id", taskId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private void PlaySound(string filePath, ManualResetEvent stopFlag)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    using (var audioFile = new AudioFileReader(filePath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();

                        while (!stopFlag.WaitOne(100)) 
                        {
                            if (outputDevice.PlaybackState == PlaybackState.Stopped)
                            {
                                audioFile.Position = 0; 
                                outputDevice.Play();    
                                
                            }
                        }
                        outputDevice.Stop();    
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error playing sound: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show($"Sound file not found: {filePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private static string GetThreadInfo(Thread th)
        {
            return $"Thread Info:\n" +
                   $"Managed Thread ID: {th.ManagedThreadId}\n" +
                   $"Is Thread Pool Thread: {th.IsThreadPoolThread}\n" +
                   $"Is Background Thread: {th.IsBackground}";

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
        


        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e) { }
        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e){}

        private void dateTimePicker1_ValueChanged_2(object sender, EventArgs e)
        {
            
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int threadCount) && threadCount > 0)
            {
                threadLimiter = new SemaphoreSlim(threadCount); 
            }
            else
            {
                MessageBox.Show("Please enter a positive number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
