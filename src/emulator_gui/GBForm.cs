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
    public partial class GBForm : Form {
        private gb gameboy;
        public GBForm() {
            InitializeComponent();
        }

        private void goToGBToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void openROMToolStripMenuItem_Click(object sender, EventArgs e) {
            if (openGBRomDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                gameboy = new gb(openGBRomDialog.FileName);
                //System.IO.StreamReader sr = new
                //   System.IO.StreamReader(openGBRomDialog.FileName);
                MessageBox.Show(openGBRomDialog.FileName);
                
            }
        }

        private void gbPanel_Paint(object sender, PaintEventArgs e) {
            e.Graphics.DrawLine(new Pen(Color.Blue, 1), new Point(1, 2), new Point(2, 3));
        }
    }
}
