using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class gb {
        byte[] romData;
        private gbCPU CPU;
        public gb(string ROM) {
            using (BinaryReader reader = new BinaryReader(File.Open(ROM, FileMode.Open))) {
                romData = reader.ReadBytes(int.MaxValue);
            }
            CPU = new gbCPU(romData);
        }
    public bool start() {
        bool state;
        state = CPU.run();
        return state;
    }
    }
