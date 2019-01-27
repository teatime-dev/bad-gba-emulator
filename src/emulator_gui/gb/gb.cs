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
        using (MemoryStream memoryStream = new MemoryStream()) {
            File.Open(ROM, FileMode.Open).CopyTo(memoryStream);
            romData = memoryStream.ToArray();
        }
        CPU = new gbCPU(romData);
    }
    public bool start() {
        bool state;
        state = CPU.run();
        return state;
    }
}
