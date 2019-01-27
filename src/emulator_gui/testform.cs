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
    public partial class testform : Form {
        public testform() {
            InitializeComponent();
        }
        private byte A, B, C, D, E, F, H, L;
        private ushort HL {
            get {
                return (ushort)(H + (L * Math.Pow(2, 8)));
            }
            set {
                byte[] bytes = BitConverter.GetBytes(value);
                H = bytes[0];
                L = bytes[1];
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            /*
            byte opcode;
            opcode = 0b11010111;
            int x = (opcode & 0b1100_0000) >> 6;
            int y = (opcode & 0b0011_1000) >> 3;
            int z = (opcode & 0b0000_0111);
            int p = (opcode & 0b0011_0000) >> 4;
            int q = (opcode & 0b0000_1000) >> 3;
            Console.WriteLine(opcode);
            Console.WriteLine(Convert.ToByte(x));
            Console.WriteLine(Convert.ToByte(y));*/
            H = 0b0001_1110;
            L = 0b1011_0010;
            Console.WriteLine(HL);
            HL = 0b1111_0101_1001_0000;
            Console.WriteLine(HL);
            Console.WriteLine(H);
            Console.WriteLine(L);




        }
    }
}