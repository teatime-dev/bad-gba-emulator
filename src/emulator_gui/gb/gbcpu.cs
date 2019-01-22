using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class gbCPU {
    static int[] prefixBytes = new int[4] { 0xCB, 0xDD, 0xED, 0xFD };
    private byte[] romData;
    private byte A, B, C, D, E, F, H, L, SP, PC;
    private int currentPos = 0;
    public gbCPU(byte[] romData) {
        this.romData = romData;
        A = B = C = D = E = F = H = L = SP = PC = 0b0000_0000;
    }

    public bool run() {
        bool finished = false;
        while (!finished) {
            runOpcode(romData[currentPos], currentPos);
        }
        return true;
    }

    private void runOpcode(byte byteToRun, int memPosition) {
        byte prefixByte, prefixByte2, opcode, displacementByte, data1, data2;
        bool hasPrefix = false;
        bool isSecondary = false;
        bool unknownOP = true;
        if (prefixBytes.Contains(byteToRun)) {
            hasPrefix = true;
            prefixByte = byteToRun;
            //Secondary byte pattern
            if (prefixByte == 0xDD || prefixByte == 0xFD) {
                if (romData[memPosition + 1] == 0xCB) {
                    prefixByte2 = romData[memPosition + 1];
                    displacementByte = romData[memPosition + 2];
                    isSecondary = true;
                }
            }
        }
        //primary byte pattern
        opcode = romData[memPosition];
        if (hasPrefix) {
            opcode = romData[memPosition + 1];
            if (isSecondary) {
                opcode = romData[memPosition + 3];
            }
        }
        //Opcode is now established
        int x = (opcode & 0b1100_0000) >> 6;
        int y = (opcode & 0b0011_1000) >> 3;
        int z = (opcode & 0b0000_0111);
        int p = (opcode & 0b0011_0000) >> 4;
        int q = (opcode & 0b0000_1000) >> 3;

        //Handle operation


        if(unknownOP) {
            throw new Exception("Was not able to handle operation:" + opcode);
        }
    }
}
