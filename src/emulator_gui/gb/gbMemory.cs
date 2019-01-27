using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class gbMemory {
    public byte[] memoryRaw;
    public byte this[int key] {
        get {
            return memoryRaw[key];
        }
        set {
            memoryRaw[key] = value;

        }
    }
    public gbMemory() {
        memoryRaw = new byte[0xFFFF];
    }

}
