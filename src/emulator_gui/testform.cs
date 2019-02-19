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
        //private byte A, B, C, D, E, F, H, L;
        //private ushort HL {
        //    get {
        //        return (ushort)(H + (L * Math.Pow(2, 8)));
        //    }
        //    set {
        //        byte[] bytes = BitConverter.GetBytes(value);
        //        H = bytes[0];
        //        L = bytes[1];
        //    }
        //}
        //byte byte1 = 0b11110010;
        //byte byte2 = 0b10101010;
        private void button1_Click(object sender, EventArgs e) {
            gbCPU a = new gbMemory();
            a.memory[10] = 0b11110000;
            gbLCD b = new gbLCD(a);
            Console.WriteLine(b.returnMem(10));
            Console.WriteLine(b.returnMem(9));
            a[10] = 0b10101010;
            Console.WriteLine(b.returnMem(10));
            Console.WriteLine(b.returnMem(9));
            //Console.WriteLine(Convert.ToString((byte)(a << 1), 2));
            //Console.WriteLine(Convert.ToString((byte)(b << 1), 2));
            /*
            byte a = 0b11110000;
            Console.WriteLine(a);
            gbCPU.SetBit(ref a, 7);
            Console.WriteLine(a);
            gbCPU.SetBit(ref a, 0);
            Console.WriteLine(a);
            gbCPU.ClearBit(ref a, 7);
            gbCPU.ClearBit(ref a, 0);
            gbCPU.ClearBit(ref a, 3);
            Console.WriteLine(a);
            gbCPU.ClearBit(ref a, 3);
            gbCPU.SetBit(ref a, 2);
            Console.WriteLine(a);
            Console.WriteLine(gbCPU.GetBit(a, 7));
            Console.WriteLine(gbCPU.GetBit(a, 6));
            Console.WriteLine(gbCPU.GetBit(a, 5));
            Console.WriteLine(gbCPU.GetBit(a, 4));
            Console.WriteLine(gbCPU.GetBit(a, 3));
            Console.WriteLine(gbCPU.GetBit(a, 2));
            Console.WriteLine(gbCPU.GetBit(a, 1));
            Console.WriteLine(gbCPU.GetBit(a, 0));*/
            /*gb_combinationWord gb = new gb_combinationWord(ref byte1,ref byte2);
            Console.WriteLine("byte1:" + byte1 + " byte2:" + byte2 + " gbyte1:" + gb.a + " gbyte2:" + gb.b);
            byte1 = 0b00000001;
            Console.WriteLine("byte1:" + byte1 + " byte2:" + byte2 + " gbyte1:" + gb.a + " gbyte2:" + gb.b);
            gb.a = 0b11011011;
            Console.WriteLine("byte1:" + byte1 + " byte2:" + byte2 + " gbyte1:" + gb.a + " gbyte2:" + gb.b);
            gb.b = 0b11001111;
            Console.WriteLine("byte1:" + byte1 + " byte2:" + byte2 + " gbyte1:" + gb.a + " gbyte2:" + gb.b);
            byte2 = 0b11111111;
            byte1 = 0b00000000;
            Console.WriteLine("byte1:" + byte1 + " byte2:" + byte2 + " gbyte1:" + gb.a + " gbyte2:" + gb.b);
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
            Console.WriteLine(Convert.ToByte(y));
            H = 0b0001_1110;
            L = 0b1011_0010;
            Console.WriteLine(HL);
            HL = 0b1111_0101_1001_0000;
            Console.WriteLine(HL);
            Console.WriteLine(H);
            Console.WriteLine(L);*/




        }
    }
}