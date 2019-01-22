using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace emulator_gui {
    public partial class GBAForm : Form {
        public GBAForm() {
            InitializeComponent();
        }

        private void openROMToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show("Hi");
        }

        private void panel1_Paint(object sender, PaintEventArgs e) {
            e.Graphics.DrawLine(new Pen(Color.Blue, 1), new Point(1, 2), new Point(2, 3));
        }

        private void goToGBToolStripMenuItem_Click(object sender, EventArgs e) {
            GBForm gBForm = new GBForm();
            this.Hide();
            gBForm.Show();
        }
    }
}
