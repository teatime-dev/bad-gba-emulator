using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class gbCPU {
    
    private bool GetBit(byte b, int bitNumber) {
        return (b & (1 << bitNumber - 1)) != 0;
    }
    static int[] prefixBytes = new int[4] { 0xCB, 0xDD, 0xED, 0xFD };

    //private byte[] memory = new byte[0xFFFF];
    private gbMemory memory = new gbMemory();
    private byte A, B, C, D, E, F, H, L;
    private ushort HL {
        get {
            return (ushort)(H + (L * Math.Pow(2,8)));
        }
        set {
            byte[] bytes = BitConverter.GetBytes(value);
            H = bytes[0];
            L = bytes[1];
        }
    }
    private bool flag_z {
        get {           
            return GetBit(F,7);
        }
        set {
            if(value) {
                F = (byte)(F | 0b1000_0000);
            } else {
                F = (byte)(F & 0b0111_1111);
            }
        }
    }
    private bool flag_n {
        get {
            return GetBit(F, 6);
        }
        set {
            if (value) {
                F = (byte)(F | 0b1000_0000);
            } else {
                F = (byte)(F & 0b0111_1111);
            }
        }
    }
    private bool flag_h {
        get {
            return GetBit(F, 5);
        }
        set {
            if (value) {
                F = (byte)(F | 0b1000_0000);
            } else {
                F = (byte)(F & 0b0111_1111);
            }
        }
    }
    private bool flag_c {
        get {
            return GetBit(F, 4);
        }
        set {
            if (value) {
                F = (byte)(F | 0b1000_0000);
            } else {
                F = (byte)(F & 0b0111_1111);
            }
        }
    }

    private ushort SP, PC;
    private int currentPos = 0;
    private string[] table_r = new string[8] { "B", "C", "D", "E", "H", "L", "(HL)", "A" };
    private ref byte get_table_r(int index) {
        switch (index) {
            case 0:
                return ref B;
            case 1:
                return ref C;
            case 2:
                return ref D;
            case 3:
                return ref E;
            case 4:
                return ref H;
            case 5:
                return ref L;
            case 6:
                return ref memory.memoryRaw[H + (L << 8)];
            case 7:
                return ref A;
            default:
                throw new Exception("Index was out of range:" + index);
        }
    }
    private string[] table_rp = new string[4] { "BC", "DE", "HL", "SP" };
    private string[] table_rp2 = new string[4] { "BC", "DE", "HL", "AF" };
    private string[] table_cc = new string[4] { "NZ", "Z", "NC", "C" };
    private string[] table_alu = new string[8] { "ADD A", "ADC A", "SUB", "SBC A", "AND", "XOR", "OR", "CP" };
    private string[] table_rot = new string[8] { "RLC", "RRC", "RL", "RR", "SLA", "SRA", "SWAP", "SRL" };
    private string operation = "";
    private byte prefixByte, prefixByte2, opcode, displacementByte, data1, data2;
    private bool hasPrefix = false;
    private bool isSecondary = false;
    private bool unknownOP = true;
    private int x = 0;
    private int y = 0;
    private int z = 0;
    private int p = 0;
    private int q = 0;
    private ushort opLength = 1;
    public gbCPU(byte[] romData) {
        //this.memory = romData;
        Array.Copy(romData, 0, memory.memoryRaw, 0x0100, 0x014F - 0x0100);
        A = B = C = D = E = F = H = L = 0b0000_0000;
        SP = 0;
        PC = 0x0100;
    }

    public bool run() {
        bool finished = false;
        while (!finished) {
            runOpcode(memory[PC], PC);
        }
        return true;
    }

    private void do_alu(int y, int z) {
        ref byte r_value = ref get_table_r(z);
        //	alu[y] r[z]
        switch(y) {
            case 0:
                //ADD A,
                throw new NotImplementedException();
            case 1:
                //ADC A,
                throw new NotImplementedException();
            case 2:
                //SUB
                throw new NotImplementedException();
            case 3:
                //SBC A,
                throw new NotImplementedException();
            case 4:
                //AND
                throw new NotImplementedException();
            case 5:
                //XOR
                A = (byte)(r_value ^ A);
                flag_z = A == 0;
                flag_n = false;
                flag_h = false;
                flag_c = false;
                return;
            case 6:
                //OR
                throw new NotImplementedException();
            case 7:
                //CP
                throw new NotImplementedException();
        }
    }

    //private ref byte
    private void runOpcode(byte byteToRun, int memPosition) {
        hasPrefix = false;
        isSecondary = false;
        unknownOP = true;
        opLength = 0;
        if (prefixBytes.Contains(byteToRun)) {
            hasPrefix = true;
            prefixByte = byteToRun;
            //Secondary byte pattern
            if (prefixByte == 0xDD || prefixByte == 0xFD) {
                if (memory[memPosition + 1] == 0xCB) {
                    prefixByte2 = memory[memPosition + 1];
                    displacementByte = memory[memPosition + 2];
                    isSecondary = true;
                }
            }
        }
        //primary byte pattern
        opcode = memory[memPosition];
        if (hasPrefix) {
            opcode = memory[memPosition + 1];
            if (isSecondary) {
                opcode = memory[memPosition + 3];
                opLength = 4;
            }
        }
        //Opcode is now established

        x = (opcode & 0b1100_0000) >> 6;
        y = (opcode & 0b0011_1000) >> 3;
        z = (opcode & 0b0000_0111);
        p = (opcode & 0b0011_0000) >> 4;
        q = (opcode & 0b0000_1000) >> 3;
        operation = "";
        //Handle operation
        if (isSecondary) {

        } else {
            switch (x) {
                case 0:
                    // X is 0
                    switch (z) {
                        // Z is 0
                        case 0:

                            break;
                        // Z is 1
                        case 1:
                            switch(q) {
                                case 0:
                                    //x0z1q0
                                    operation = "LD " + table_rp[p] + "," + (memory[memPosition + 1] + (memory[memPosition + 2] << 8)).ToString("X");
                                    //op_LD(table_rp[p], romData[memPosition + 1] + romData[memPosition + 2]);
                                    opLength = 3;
                                    SP = Convert.ToUInt16(memory[memPosition + 1] + (memory[memPosition + 2] << 8));
                                    //TEMPORARY CODE
                                    unknownOP = false;
                                    break;
                                case 1:

                                    break;
                            }
                            break;
                        //Z is 2
                        case 2:
                            switch(q) {
                                //Q is 0
                                case 0:
                                    switch(p) {
                                        case 0:
                                            operation = "LD (BC),A";

                                            break;
                                        case 1:
                                            operation = "LD (DE), A";

                                            break;
                                        case 2:
                                            operation = "LD [HL+], A";
                                            opLength = 1;
                                            memory[HL] = A;
                                            HL -= 1;
                                            unknownOP = false;
                                            break;
                                        case 3:
                                            operation = "LD [HL-], A";
                                            opLength = 1;
                                            memory[HL] = A;
                                            HL -= 1;
                                            unknownOP = false;
                                            break;
                                    }
                                    break;
                                case 1:
                                    switch (p) {
                                        case 0:
                                            operation = "LD A,(BC)";

                                            break;
                                        case 1:
                                            operation = "LD A,(DE)";

                                            break;
                                        case 2:
                                            operation = "LD A,(HL+)";
                                            opLength = 1;
                                            A = memory[HL];
                                            HL -= 1;
                                            unknownOP = false;
                                            break;
                                        case 3:
                                            operation = "LD A,(HL-)";
                                            opLength = 1;
                                            A = memory[HL];
                                            HL -= 1;
                                            unknownOP = false;
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case 3:

                            break;
                        case 4:

                            break;
                        case 5:

                            break;
                        case 6:

                            break;
                        case 7:

                            break;
                    }
                    break;
                case 1:
                    // X is 1
                    operation = "LD " + table_r[y] + "," + table_r[z];
                    //:  | x1 | y7 | z4 | p3 | q1'


                    if (z == 6) {
                        if(y == 6) {
                            throw new NotImplementedException("exception to replace LD(HL),(HL)");
                        }
                    }
                    break;
                // X is 2
                case 2:
                    operation = table_alu[y] + " " + table_r[z];
                    do_alu(y, z);
                    unknownOP = false;
                    opLength = 1;
                    break;
                case 3:

                    break;
            }
        }
        // 16 bit STORED AS LEAST SIGNIFICANT BIT FIRST
        if (unknownOP) {
            throw new Exception("Was not able to handle operation:" + opcode + " if applicable, operation is known as: " + operation + " | x" + x + " | y" + y + " | z" + z + " | p" + p + " | q" + q);
        }
        PC += opLength;
    }
}
