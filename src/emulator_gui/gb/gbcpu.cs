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

    /// <summary>
    /// Returns true if the bytes added is a half carry (i.e. bit 3 goes to bit 4)
    /// Does not add the bits or change flags
    /// </summary>
    /// <param name="a">First byte</param>
    /// <param name="b">Second byte</param>
    /// <returns></returns>
    public static bool IsHalfCarry(byte a, byte b) {
        return ((((a & 0xf) + (b & 0xf)) & 0x10) == 0x10);
    }
    /// <summary>
    /// Adds b to a, changes flag_h if a half carry
    /// </summary>
    /// <param name="a">Byte to add to</param>
    /// <param name="b">Byte to add</param>
    /// <returns></returns>
    private bool AddandCarry(ref byte a, byte b) {
        if ((((a & 0xf) + (b & 0xf)) & 0x10) == 0x10) {
            flag_h = true;
        } else {
            flag_h = false;
        }
        a = (byte)(a + b);
        return flag_h;
    }
    public static bool GetBit(byte b, int bitNumber) {
        if (bitNumber < 0 || bitNumber > 7) {
            throw new Exception("bitNumber is higher than a byte can handle :" + bitNumber);
        }
        return (b & (1 << bitNumber)) != 0;
    }
    public static void SetBit(ref byte b, int bitNumber) {
        b = (byte)(b | (byte)Math.Pow(2, bitNumber));
    }
    public static void ClearBit(ref byte b, int bitNumber) {
        b = (byte)(b & ~(byte)Math.Pow(2, bitNumber));
    }
    static int[] prefixBytes = new int[4] { 0xCB, 0xDD, 0xED, 0xFD };
    //private byte[] memory = new byte[0xFFFF];
    private gbMemory memory = new gbMemory();
    private byte A, B, C, D, E, F, H, L;
    private ushort HL {
        get {
            return (ushort)(L + (H * Math.Pow(2, 8)));
        }
        set {
            byte[] bytes = BitConverter.GetBytes(value);
            H = bytes[1];
            L = bytes[0];
        }
    }
    private ushort BC {
        get {
            return (ushort)(C + (B * Math.Pow(2, 8)));
        }
        set {
            byte[] bytes = BitConverter.GetBytes(value);
            B = bytes[1];
            C = bytes[0];
        }
    }
    private ushort DE {
        get {
            return (ushort)(E + (D * Math.Pow(2, 8)));
        }
        set {
            byte[] bytes = BitConverter.GetBytes(value);
            D = bytes[1];
            E = bytes[0];
        }
    }
    private ushort AF {
        get {
            return (ushort)(F + (A * Math.Pow(2, 8)));
        }
        set {
            byte[] bytes = BitConverter.GetBytes(value);
            A = bytes[1];
            F = bytes[0];
        }
    }
    private bool flag_z {
        get {
            return GetBit(F, 7);
        }
        set {
            if (value) {
                SetBit(ref F, 7);
            } else {
                ClearBit(ref F, 7);
            }
        }
    }
    private bool flag_n {
        get {
            return GetBit(F, 6);
        }
        set {
            if (value) {
                SetBit(ref F, 6);
            } else {
                ClearBit(ref F, 6);
            }
        }
    }
    private bool flag_h {
        get {
            return GetBit(F, 5);
        }
        set {
            if (value) {
                SetBit(ref F, 5);
            } else {
                ClearBit(ref F, 5);
            }
        }
    }
    private bool flag_c {
        get {
            return GetBit(F, 4);
        }
        set {
            if (value) {
                SetBit(ref F, 4);
            } else {
                ClearBit(ref F, 4);
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
    private string[] table_rp_names = new string[4] { "BC", "DE", "HL", "SP" };
    private ushort get_table_rp(int index) {
        switch (index) {
            case 0:
                return BC;
            case 1:
                return DE;
            case 2:
                return HL;
            case 3:
                return SP;
            default:
                throw new IndexOutOfRangeException("rp index should only be 0-3");
        }
    }
    private void set_table_rp(int index, ushort value) {
        switch (index) {
            case 0:
                BC = value;
                break;
            case 1:
                DE = value;
                break;
            case 2:
                HL = value;
                break;
            case 3:
                SP = value;
                break;
        }
    }
    private ushort get_table_rp2(int index) {
        switch (index) {
            case 0:
                return BC;
            case 1:
                return DE;
            case 2:
                return HL;
            case 3:
                return AF;
            default:
                throw new IndexOutOfRangeException("rp2 index should only be 0-3");
        }
    }
    private void set_table_rp2(int index, ushort value) {
        switch (index) {
            case 0:
                BC = value;
                break;
            case 1:
                DE = value;
                break;
            case 2:
                HL = value;
                break;
            case 3:
                AF = value;
                break;
        }
    }
    private void set_table_rp(int index, byte[] value) {
        //return (ushort)(H + (L * Math.Pow(2, 8)));
        ushort valueToSet = (ushort)(value[0] + (value[1] * Math.Pow(2, 8)));
        set_table_rp(index, valueToSet);
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
    private string[] table_rp2_names = new string[4] { "BC", "DE", "HL", "AF" };
    private string[] table_cc = new string[4] { "NZ", "Z", "NC", "C" };
    private string[] table_alu = new string[8] { "ADD A", "ADC A", "SUB", "SBC A", "AND", "XOR", "OR", "CP" };
    private string[] table_rot = new string[8] { "RLC", "RRC", "RL", "RR", "SLA", "SRA", "SWAP", "SRL" };
    private string operation = "";
    private byte prefixByte, prefixByte2, opcode;
    private byte[] data;
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
        //Array.Copy(romData, 0, memory.memoryRaw, 0, 0x100);
        memory.loadCatridge(romData);
        A = B = C = D = E = F = H = L = 0b0000_0000;
        SP = 0;
        PC = 0;
        data = new byte[2];
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
        switch (y) {
            case 0:
                //ADD A,
                if (A + r_value > 0xff) {
                    flag_c = true;
                }
                AddandCarry(ref A, r_value);
                flag_n = false;
                flag_z = A == 0;
                return;
            case 1:
                //ADC A,
                if (A + r_value > 0xff) {
                    flag_c = true;
                }
                AddandCarry(ref A, (byte)(r_value + (flag_c ? 1 : 0)));
                flag_n = false;
                flag_z = A == 0;
                return;
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
                flag_z = (A == 0);
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
            if (prefixByte == 0xCB) {
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
                            switch (y) {
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
                                    operation = "JR " + table_cc[y - 4] + "," + displacement;
                                    opLength = 2;
                                    unknownOP = false;
                                    bool condition;
                                    switch (y - 4) {
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
                                    } else {
                                        // DELETE THIS
                                        Console.WriteLine("DIDNT JUMP");
                                    }
                                    break;
                            }
                            break;
                        // Z is 1
                        case 1:
                            switch (q) {
                                case 0:
                                    //x0z1q0
                                    operation = "LD " + table_rp_names[p] + "," + (memory[memPosition + 1] + (memory[memPosition + 2] << 8)).ToString("X");
                                    //op_LD(table_rp[p], romData[memPosition + 1] + romData[memPosition + 2]);
                                    opLength = 3;
                                    data[0] = memory[memPosition + 1];
                                    data[1] = memory[memPosition + 2];
                                    // TODO: FIX THIS - switch statement which could be a table_rp function thing.
                                    // Cant return ref to HL  because it doesn't like that - it's fine with SP because it doesnt have custom get/set.
                                    // Maybe return an anonymous function which has get and set for all of them?
                                    set_table_rp(p, data);
                                    //SP = Convert.ToUInt16(data1 + data2 << 8);
                                    //TEMPORARY CODE
                                    unknownOP = false;
                                    break;
                                case 1:

                                    break;
                            }
                            break;
                        //Z is 2
                        case 2:
                            opLength = 1;
                            unknownOP = false;
                            switch (q) {
                                //Q is 0
                                case 0:
                                    switch (p) {
                                        case 0:
                                            operation = "LD (BC),A";
                                            memory[BC] = A;
                                            break;
                                        case 1:
                                            operation = "LD (DE), A";
                                            memory[DE] = A;
                                            break;
                                        case 2:
                                            operation = "LD [HL+], A";
                                            memory[HL] = A;
                                            HL -= 1;
                                            break;
                                        case 3:
                                            operation = "LD [HL-], A";
                                            memory[HL] = A;
                                            HL -= 1;

                                            //DELETE BELOW

                                            //DELETE ABOVE
                                            break;
                                    }
                                    break;
                                case 1:
                                    switch (p) {
                                        case 0:
                                            operation = "LD A,(BC)";
                                            A = memory[BC];
                                            break;
                                        case 1:
                                            operation = "LD A,(DE)";
                                            A = memory[DE];
                                            break;
                                        case 2:
                                            operation = "LD A,(HL+)";
                                            A = memory[HL];
                                            HL -= 1;
                                            break;
                                        case 3:
                                            operation = "LD A,(HL-)";
                                            A = memory[HL];
                                            HL -= 1;
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case 3:

                            break;
                        case 4:
                            // Z is 4
                            operation = "INC " + table_r[y];
                            opLength = 1;
                            flag_n = false;
                            flag_z = (A == 0);
                            byte added = get_table_r(y);
                            AddandCarry(ref added, 0b1);
                            set_table_r(y, added);
                            unknownOP = false;
                            break;
                        case 5:

                            break;
                        //Z is 6
                        case 6:
                            //System.Exception: 'Was not able to handle operation:38 if applicable, operation is known as:  | x0 | y4 | z6 | p2 | q0'
                            operation = "LD " + table_r[y] + "," + memory[memPosition + 1];
                            opLength = 2;
                            unknownOP = false;
                            data[0] = memory[memPosition + 1];
                            set_table_r(y, data[0]);
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
                        if (y == 6) {
                            throw new NotImplementedException("exception to replace LD(HL),(HL)");
                        }
                    }
                    byte right = get_table_r(z);
                    set_table_r(y, right);

                    break;
                // X is 2
                case 2:
                    operation = table_alu[y] + " " + table_r[z];
                    do_alu(y, z);
                    unknownOP = false;
                    opLength = 1;
                    break;
                case 3:
                    // X is 3
                    switch (z) {
                        case 0:
                            // Z is 0
                            switch (y) {
                                case 0:
                                case 1:
                                case 2:
                                case 3:

                                    break;
                                case 4:
                                    data[0] = memory[memPosition + 1];
                                    //data[1] = memory[memPosition + 1];
                                    operation = "LD(0xFF00 + " + data[0] + "),A";
                                    opLength = 2;
                                    memory[0xFF00 + data[0]] = A;
                                    unknownOP = false;
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
                            // Z is 1
                            switch (q) {
                                case 0:
                                    operation = "POP " + table_rp2_names[p];
                                    opLength = 1;
                                    unknownOP = false;
                                    set_table_rp2(p, (ushort)(memory[SP] + (memory[SP + 1] * Math.Pow(2, 8))));
                                    SP += 2;
                                    break;
                                case 1:

                                    break;
                            }
                            break;
                        case 2:
                            // Z is 2
                            switch (y) {
                                case 0:
                                case 1:
                                case 2:
                                case 3:

                                    break;
                                case 4:
                                    operation = "LD(0xFF00+C), A";
                                    opLength = 1;
                                    memory[0xFF00 + C] = A;
                                    unknownOP = false;
                                    break;
                                case 5:

                                    break;
                                case 6:

                                    break;
                                case 7:

                                    break;
                            }
                            break;
                        case 3:

                            break;
                        case 4:

                            break;
                        case 5:
                            switch (q) {
                                case 0:

                                    break;
                                case 1:
                                    if (p != 0) {
                                        throw new Exception("operation is removed (x3z5q1p1-3)");
                                    }
                                    data[0] = memory[memPosition + 1];
                                    data[1] = memory[memPosition + 2];
                                    ushort callValue = (ushort)(data[0] + (data[1] * Math.Pow(2, 8)));
                                    operation = "CALL " + callValue;
                                    byte[] nextAddress = BitConverter.GetBytes(PC + 3);
                                    opLength = 0;
                                    memory[SP - 1] = nextAddress[1];
                                    memory[SP - 2] = nextAddress[0];
                                    SP -= 2;
                                    memPosition = callValue;
                                    unknownOP = false;
                                    break;
                            }
                            break;
                        case 6:

                            break;
                        case 7:

                            break;
                    }
                    break;
            }
        }
        // 16 bit STORED AS LEAST SIGNIFICANT BIT FIRST
        if (unknownOP) {
            throw new Exception("Was not able to handle operation:" + opcode + " if applicable, operation is known as: " + operation + " | x" + x + " | y" + y + " | z" + z + " | p" + p + " | q" + q);
        }
        PC += opLength;
        Console.WriteLine(operation + " PC = " + memPosition);
    }
}
