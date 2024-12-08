using demo2.Forms;
using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;


namespace demo2
{
    public partial class FormMain : Form
    {
        //fields
        private IconButton currentBtn;
        private Panel leftBorderBtn;
        private Form currentChildForm;
        private System.Windows.Forms.Timer reminderTimer;


        public FormMain()
        {
            InitializeComponent();
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
           
        }

        private struct RGBColors
        {
            public static Color color1 = ColorTranslator.FromHtml("#fadbd8");
            public static Color color2 = ColorTranslator.FromHtml("#d4e6f1");
            public static Color color3 = ColorTranslator.FromHtml("#f2f3f4");
            public static Color color4 = ColorTranslator.FromHtml("#fef5e7");
            public static Color color5 = ColorTranslator.FromHtml("#eafaf1");
            public static Color color6 = ColorTranslator.FromHtml("#f4ecf7");
        }

        private void ActivateButton(object senderBtn, Color color)
        {
            if (senderBtn != null)
            {
                DisableButton();
                //Button
                currentBtn = (IconButton)senderBtn;
                currentBtn.BackColor = ColorTranslator.FromHtml("#4D766E");
                currentBtn.ForeColor = color;
                //currentBtn.TextAlign = ContentAlignment.MiddleCenter;
                currentBtn.IconColor = color;
                // currentBtn.TextImageRelation = TextImageRelation.TextBeforeImage;
                //currentBtn.ImageAlign = ContentAlignment.MiddleCenter;

                currentBtn.Padding = new Padding(17, 0, 20, 0);
                //LeftBordeBtn
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
                //currentBtn.TextImageRelation = TextImageRelation.ImageBeforeText;
                //currentBtn.ImageAlign = ContentAlignment.MiddleLeft;
                currentBtn.Padding = new Padding(10, 0, 20, 0);
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
            childForm.Dock = DockStyle.Fill; // Ensure the child form fills the panel
            panelDesktop.Controls.Clear(); // Clear any existing controls in the panel
            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;
            childForm.BringToFront(); // Bring the child form to the front
            childForm.Show();
        }


        private void iconButton1_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color1);
            openChildForm(new FormDashboard());
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color2);
            openChildForm(new FormAdd());
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color3);
            openChildForm(new FormList());
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color4);
            openChildForm(new FormDone());
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color5);
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
        private void iconButton6_Click(object sender, EventArgs e)
        {

        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
         private extern static void ReleaseCapture();
       
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int IParam);
        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
           ReleaseCapture();
           SendMessage(this.Handle, 0x112, 0xf012, 0);

        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
