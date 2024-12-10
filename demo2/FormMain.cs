using demo2.Forms;
using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Data.Entity.Infrastructure.Design.Executor;



namespace demo2
{

    public partial class FormMain : Form
    {
        //fields
        private string connectionString = "Data Source=ReminderApp.db;";
        private readonly object dbLock = new object();


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
          //  InitializeDatabase();
            InitializeDateTimeUpdater();
            dateTimeTimer.Start(); // Start the datetime updater
            InitializeReminderChecker();
            this.StartPosition = FormStartPosition.CenterScreen;
            
            leftBorderBtn = new Panel();
            leftBorderBtn.Size = new Size(7, 60);
            panel2.Controls.Add(leftBorderBtn);
            
            //form
            this.Text = string.Empty;
            //this.ControlBox = false;
            this.DoubleBuffered = true;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            reminderTimer = new System.Windows.Forms.Timer();
            reminderTimer.Interval = 1000; // 1 second interval

            reminderStopTimer = new Timer { Interval = 60000 }; // 1 minute interval
            reminderStopTimer.Tick += ReminderStopTimer_Tick;

        }


        private void ReminderStopTimer_Tick(object sender, EventArgs e)
        {
            // Stop the reminder timer
            reminderTimer.Stop();

            // Optionally, you could disable the stop timer if you no longer need it
            reminderStopTimer.Stop();
        }

        private struct RGBColors
        {
            public static Color color1 = ColorTranslator.FromHtml("#fadbd8"); 
            public static Color color2 = ColorTranslator.FromHtml("#f8edc8");
           // public static Color color3 = ColorTranslator.FromHtml("#f2f3f4");
           // public static Color color4 = ColorTranslator.FromHtml("#fef5e7");
           // public static Color color5 = ColorTranslator.FromHtml("#eafaf1");
           // public static Color color6 = ColorTranslator.FromHtml("#f4ecf7");
        }

        private void ActivateButton(object senderBtn, Color color)
        {
            if (senderBtn != null)
            {
                DisableButton();
                //Button
                currentBtn = (IconButton)senderBtn;
                currentBtn.BackColor = ColorTranslator.FromHtml("#69A095");
                currentBtn.ForeColor = color;
                currentBtn.IconColor = color;
                

                currentBtn.Padding = new Padding(20, 0, 0, 0);
                
                leftBorderBtn.BackColor = color;
                leftBorderBtn.Location = new Point(0, currentBtn.Location.Y);
                leftBorderBtn.Visible = true;
                leftBorderBtn.BringToFront();

                //Icon current child form
                iconButton6.IconChar = currentBtn.IconChar;
                iconButton6.IconColor = color;
                titleChildForm.Text = currentBtn.Text;
                //iconButton6.Text = currentBtn.Text;



            }
        }

        private void DisableButton()
        {
            if (currentBtn != null)
            {
                //Button
                currentBtn.BackColor = ColorTranslator.FromHtml("#A5C4BD");
                currentBtn.ForeColor = ColorTranslator.FromHtml("#f6f4f0");
               // currentBtn.TextAlign = ContentAlignment.MiddleLeft;
                currentBtn.IconColor = ColorTranslator.FromHtml("#f6f4f0");
                
                currentBtn.Padding = new Padding(10, 0, 10, 0);

            }
        }


        private void openChildForm(Form childForm)
        {
            // Close any currently opened child form
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }


            // Set up the new child form
            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            childForm.Size =panelDesktop.Size;
           // panelDesktop.Controls.Clear(); 
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
            currentChildForm.Close();
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
        // Timer Initialization
        private void InitializeDateTimeUpdater()
        {
            dateTimeTimer = new Timer { Interval = 1000 };
            dateTimeTimer.Tick += DateTimeTimer_Tick;
        }

        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            maskedTextBox1.Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            DateTime currentTime = DateTime.Now;
            
        }

       // Reminder Timer
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
                        while (reader.Read())
                        {
                            string taskName = reader["TaskName"].ToString();
                            DateTime reminderTime = DateTime.Parse(reader["Reminder"].ToString());
                            DateTime currentTime = DateTime.Now;

                            // Allow a small range for matching reminders
                            if (currentTime >= reminderTime && currentTime < reminderTime.AddSeconds(1))
                            {
                                try
                                {
                                    MessageBox.Show($"Reminder: {taskName} is due at {reminderTime}.", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    if (soundPlayer == null)
                                    {
                                        soundPlayer = new System.Media.SoundPlayer();
                                        soundPlayer.SoundLocation = @"C:\Users\Sanag\Downloads\12345.wav"; // Update to relative if needed
                                    }
                                    soundPlayer.PlayLooping();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"An error occurred while showing the reminder: {ex.Message}");
                                }
                            }
                        }
                    }
                }
            }
        }



        // Form Load and Closing Handlers
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            dateTimeTimer.Stop();
            base.OnFormClosing(e);
        }





        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {

        }
    }
}
