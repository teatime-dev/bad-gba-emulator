using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class gbMemory {
    public byte[] memoryRaw;
    private bool biosEnabled = true;
    public byte this[int key] {
        get {
            return memoryRaw[key];
        }
        set {
            if(biosEnabled && key < 255) {

            } else {
                memoryRaw[key] = value;
                if(biosEnabled) {
                    if(memoryRaw[0xFF50] > 0) {
                        biosEnabled = false;
                        //Array.Clear
                    }
                }
            }

        }
    }
    public gbMemory() {
        memoryRaw = new byte[0x10000];
    }

}
