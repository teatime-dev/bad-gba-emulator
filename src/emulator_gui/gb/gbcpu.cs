using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class gbCPU {

    // from https://stackoverflow.com/questions/48567214/how-to-convert-a-byte-in-twos-complement-form-to-its-integer-value-c-sharp
    public static int ConvertTwosComplementByteToInteger(byte rawValue) {
        // If a positive value, return it
        if ((rawValue & 0x80) == 0) {
            return rawValue;
        }

        // Otherwise perform the 2's complement math on the value
        return (byte)(~(rawValue - 0x01)) * -1;
    }


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
    private byte get_table_r(int index) {
        switch (index) {
            case 0:
                return B;
            case 1:
                return C;
            case 2:
                return D;
            case 3:
                return E;
            case 4:
                return H;
            case 5:
                return L;
            case 6:
                return memory.memoryRaw[H + (L << 8)];
            case 7:
                return A;
            default:
                throw new Exception("Index was out of range:" + index);
        }
    }
    private void set_table_r(int index, byte value) {
        switch (index) {
            case 0:
                B = value;
                break;
            case 1:
                C = value;
                break;
            case 2:
                D = value;
                break;
            case 3:
                E = value;
                break;
            case 4:
                H = value;
                break;
            case 5:
                L = value;
                break;
            case 6:
                memory.memoryRaw[H + (L << 8)] = value;
                break;
            case 7:
                A = value;
                break;
            default:
                throw new Exception("Index was out of range:" + index);
        }
    }
    /*private out ushort get_table_rp(int index) {
        switch (index) {
            case 0:
                return ref BC;
            case 1:
                return ref DE;
            case 2:
                ref ushort a = ref HL;
                return ref HL;
            case 3:
                return SP;
            default:
                throw new Exception("Index was out of range:" + index);
        }
    }*/
    private string[] table_rp = new string[4] { "BC", "DE", "HL", "SP" };
    private string[] table_rp2 = new string[4] { "BC", "DE", "HL", "AF" };
    private string[] table_cc = new string[4] { "NZ", "Z", "NC", "C" };
    private string[] table_alu = new string[8] { "ADD A", "ADC A", "SUB", "SBC A", "AND", "XOR", "OR", "CP" };
    private string[] table_rot = new string[8] { "RLC", "RRC", "RL", "RR", "SLA", "SRA", "SWAP", "SRL" };
    private string operation = "";
    private byte prefixByte, prefixByte2, opcode, data1, data2;
    private sbyte displacementByte;
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
        Array.Copy(romData, 0, memory.memoryRaw, 0, 0x100);
        A = B = C = D = E = F = H = L = 0b0000_0000;
        SP = 0;
        PC = 0;
    }

    public bool run() {
        bool finished = false;
        while (!finished) {
            runOpcode(memory[PC], PC);
        }
        return true;
    }

    private void do_alu(int y, int z) {
        byte r_value = get_table_r(z);
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
                    displacementByte = (sbyte)memory[memPosition + 2];
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
        if (hasPrefix) {
            if(prefixByte == 0xCB) {
                switch (x) {
                    case 0:
                        operation = table_rot[y] + " " + table_r[z];
                        break;
                    case 1:
                        operation = "BIT " + y + ", " + table_r[z];
                        flag_z = !GetBit(get_table_r(z), y);
                        flag_n = false;
                        flag_h = true;
                        unknownOP = false;
                        break;
                    case 2:
                        operation = "RES " + y + ", " + table_r[z];
                        break;
                    case 3:
                        operation = "SET " + y + ", " + table_r[z];
                        break;
                }
                opLength = 2;
            } else {
                throw new Exception("Prefix byte detected that isn't beginning with CB");
            }
        } else {
            switch (x) {
                case 0:
                    // X is 0
                    switch (z) {
                        
                        case 0:
                            // Z is 0
                            switch(y) {
                                case 0:

                                    break;
                                case 1:

                                    break;
                                case 2:

                                    break;
                                case 3:

                                    break;
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                    displacementByte = (sbyte)memory[memPosition + 1];
                                    int displacement = displacementByte;
                                    operation = "JR " + table_cc[y-4] + "," + displacement;
                                    opLength = 2;
                                    unknownOP = false;
                                    bool condition;
                                    switch(y-4) {
                                        case 0:
                                            condition = !flag_z;
                                            break;
                                        case 1:
                                            condition = flag_z;
                                            break;
                                        case 2:
                                            condition = !flag_c;
                                            break;
                                        case 3:
                                            condition = flag_c;
                                            break;
                                        default:
                                            condition = false;
                                            throw new Exception("JR condition isn't correct:" + (y - 4));
                                    }
                                    if (condition) {
                                        PC = (ushort)(PC + (displacement + 2));
                                        opLength = 0;
                                    }
                                    break;
                            }
                            break;
                        // Z is 1
                        case 1:
                            switch(q) {
                                case 0:
                                    //x0z1q0
                                    operation = "LD " + table_rp[p] + "," + (memory[memPosition + 1] + (memory[memPosition + 2] << 8)).ToString("X");
                                    //op_LD(table_rp[p], romData[memPosition + 1] + romData[memPosition + 2]);
                                    opLength = 3;
                                    data1 = memory[memPosition + 1];
                                    data2 = memory[memPosition + 2];
                                    // TODO: FIX THIS - switch statement which could be a table_rp function thing.
                                    // Cant return ref to HL  because it doesn't like that - it's fine with SP because it doesnt have custom get/set.
                                    // Maybe return an anonymous function which has get and set for all of them?
                                    switch(p) {

                                    }
                                    SP = Convert.ToUInt16(data1 + data2 << 8);
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
                        //Z is 6
                        case 6:
                            //System.Exception: 'Was not able to handle operation:38 if applicable, operation is known as:  | x0 | y4 | z6 | p2 | q0'
                            operation = "LD " + table_r[y] + "," + memory[memPosition + 1];
                            opLength = 2;
                            unknownOP = false;
                            data1 = memory[memPosition + 1];
                            ref byte loadTarget = ref get_table_r(y);
                            loadTarget = data1;
                            break;
                        case 7:

                            break;
                    }
                    break;
                case 1:
                    // X is 1
                    operation = "LD " + table_r[y] + "," + table_r[z];
                    unknownOP = false;
                    opLength = 1;
                    //:  | x1 | y7 | z4 | p3 | q1'
                    if (z == 6) {
                        if(y == 6) {
                            throw new NotImplementedException("exception to replace LD(HL),(HL)");
                        }
                    }
                    ref byte left = ref get_table_r(y);
                    ref byte right = ref get_table_r(z);
                    left = right;
                    
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
        } else {
            Console.WriteLine(operation + " PC = " + memPosition);
        } 
        PC += opLength;
    }
}
