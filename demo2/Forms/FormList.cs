using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Bunifu.UI.WinForms;

namespace demo2.Forms
{
    public partial class FormList : Form
    {
        public FormList()
        {
            InitializeComponent();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewTasks.SelectedRows.Count > 0)
            {
                 }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewTasks.SelectedRows.Count > 0)
            {
                var confirmResult = MessageBox.Show("Are you sure to delete this task?",
                                                     "Confirm Delete",
                                                     MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    // Delete logic here
                    dataGridViewTasks.Rows.RemoveAt(dataGridViewTasks.SelectedRows[0].Index);
                }
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            // Implement filtering logic here
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridViewTasks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
