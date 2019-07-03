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
    public enum INTERRUPTS {
        VBLANK,
        LCDC,
        SERIAL,
        TIMER,
        HiToLo,
    }
    public void interrupt(INTERRUPTS interrupt) {
        throw new NotImplementedException();
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
    /// <summary>
    /// Subtracts b from a, changes flag_h if a half carry
    /// </summary>
    /// <param name="a">Byte to remove from</param>
    /// <param name="b">Byte to remove</param>
    /// <returns></returns>
    private bool SubtractAndCarry(ref byte a, byte b) {
        if (((a & 0xF) - (b & 0xF)) < 0) {
            flag_h = true;
        } else {
            flag_h = false;
        }
        a = (byte)(a - b);
        return flag_h;
    }
    /// <summary>
    /// Gets the nth bit, 0-7
    /// </summary>
    /// <param name="b">bit to get</param>
    /// <param name="bitNumber">nth bit, 0-7</param>
    /// <returns></returns>
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
    public gbMemory memory;
    public gbLCD lcd;
   
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
    /// <summary>
    /// register table, in order: B,C,D,E,H,L,(HL),A
    /// </summary>
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
                return memory.memoryRaw[HL];
            case 7:
                return A;
            default:
                throw new Exception("Index was out of range:" + index);
        }
    }
    /// <summary>
    /// sets the value at the register table
    /// in order of index 
    /// B,C,D,E,H,L,(HL),A
    /// </summary>
    /// <param name="index">Value of index</param>
    /// <param name="value">Value to set it to</param>
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
    /// <summary>
    /// 16 bit register values, in order:
    /// BC,DE,HL,SP
    /// </summary>
    private string[] table_rp_names = new string[4] { "BC", "DE", "HL", "SP" };
    /// <summary>
    /// Gets the value of the 16 bit register
    /// In order (0-3):
    /// BC,DE,HL,SP
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Sets the value located at the point in the 16 bit register
    /// BC,DE,HL,SP
    /// </summary>
    /// <param name="index">The index of the 16 bit register, refer to previous</param>
    /// <param name="value">The value to set it at</param>
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
    /// <summary>
    /// Gets the value at table rp2
    /// In order: BC,DE,HL,AF
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Sets the value at table rp2
    /// In order: BC,DE,HL,AF
    /// </summary>
    /// <param name="index">Index of the value</param>
    /// <param name="value">Value to set it to</param>
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
    /// <summary>
    /// Overload for normal method.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
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
    private byte n {
        get {
            return memory[PC + 1];
        }
    }
    private ushort nn {
        get {
            return (ushort)(data[0] + (data[1] * Math.Pow(2, 8)));
        }
    }
    /// <summary>
    /// BC,DE,HL,AF
    /// </summary>
    private string[] table_rp2_names = new string[4] { "BC", "DE", "HL", "AF" };
    /// <summary>
    /// NZ,Z,NC,C
    /// </summary>
    private string[] table_cc = new string[4] { "NZ", "Z", "NC", "C" };
    /// <summary>
    /// ADD A,ADC A,SUB,SBC A,AND,XOR,OR,CP
    /// </summary>
    private string[] table_alu = new string[8] { "ADD A", "ADC A", "SUB", "SBC A", "AND", "XOR", "OR", "CP" };
    /// <summary>
    /// RLC,RRC,RL,RR,SLA,SRA,SWAP,SRL
    /// </summary>
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
    private int cycleLength = 0;
    public gbCPU(byte[] romData) {
        //this.memory = romData;
        //Array.Copy(romData, 0, memory.memoryRaw, 0, 0x100);
        memory = new gbMemory();
        memory.loadCatridge(romData);
        lcd = new gbLCD(this);
        A = B = C = D = E = F = H = L = 0b0000_0000;
        SP = 0;
        PC = 0;
        data = new byte[2];
    }

    public bool run() {
        bool finished = false;
        while (!finished) {
            runOpcode(memory[PC]);
        }
        return true;
    }
    private void do_rot(int y, int z) {
        byte r_value = get_table_r(z);
        //	rot[y] r[z]
        switch (y) {
            case 0:
                //RLC 
                throw new NotImplementedException();
            case 1:
                //RRC
                throw new NotImplementedException();
            case 2:
                //RL
                // Moves bit 7 to carry, and carry to bit 0
                bool tempBit = flag_c;
                flag_c = GetBit(r_value, 7);
                r_value = (byte)(r_value << 1);
                ClearBit(ref r_value, 0);
                if(tempBit) {
                    SetBit(ref r_value, 0);
                }
                flag_h = false;
                flag_n = false;
                flag_z = false;
                if(r_value == 0) {
                    flag_z = true;
                }
                set_table_r(z, r_value);
                break;
            case 3:
                //RR
                throw new NotImplementedException();
            case 4:
                //SLA
                throw new NotImplementedException();
            case 5:
                //SRA
                throw new NotImplementedException();
            case 6:
                //SWAP
                throw new NotImplementedException();
            case 7:
                //SRL
                throw new NotImplementedException();
        }
    }
    private int do_alu(int y, int z, bool isHLorN) {
        byte r_value = (byte)z;
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
                break;
            case 1:
                //ADC A,
                if (A + r_value > 0xff) {
                    flag_c = true;
                }
                AddandCarry(ref A, (byte)(r_value + (flag_c ? 1 : 0)));
                flag_n = false;
                flag_z = A == 0;
                break;
            case 2:
                //SUB
                flag_n = true;
                flag_z = (A == r_value);
                flag_c = (A < r_value);
                SubtractAndCarry(ref A, r_value);
                break;
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
                break;
            case 6:
                //OR
                throw new NotImplementedException();
            case 7:
                //CP
                byte temp_a = A;
                if(PC > 0xe0) {
                    if(1==1) {

                    }
                }
                flag_n = true;
                flag_z = (A == r_value);
                flag_c = (A < r_value);
                SubtractAndCarry(ref temp_a, r_value);
                break;
        }
        if (isHLorN)
        {
            return 8;
        }
        return 4;
    }

    //private ref byte
    private void runOpcode(byte byteToRun/*, int memPosition*/) {
        cycleLength = -1;
        hasPrefix = false;
        isSecondary = false;
        unknownOP = true;
        opLength = 0;
        if (prefixBytes.Contains(byteToRun)) {
            hasPrefix = true;
            prefixByte = byteToRun;
            //Secondary byte pattern
            if (prefixByte == 0xDD || prefixByte == 0xFD) {
                if (n == 0xCB) {
                    prefixByte2 = n;
                    displacementByte = (sbyte)memory[PC + 2];
                    isSecondary = true;
                }
            }
        }
        //primary byte pattern
        opcode = memory[PC];
        if (hasPrefix) {
            opcode = memory[PC + 1];
            if (isSecondary) {
                opcode = memory[PC + 3];
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
                        do_rot(y, z);
                        unknownOP = false;
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
                if (z == 6)
                {
                    cycleLength = 16;
                } else
                {
                    cycleLength = 8;
                }
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
                                    // Y is 0
                                    unknownOP = false;
                                    operation = "NOP";
                                    opLength = 1;
                                    cycleLength = 4;
                                    break;
                                case 1:

                                    break;
                                case 2:

                                    break;
                                case 3:
                                    displacementByte = (sbyte)memory[PC + 1];
                                    int displacement = displacementByte;
                                    unknownOP = false;
                                    operation = "JR " + displacement;
                                    PC = (ushort)(PC + (displacement + 2));
                                    opLength = 0;
                                    cycleLength = 8;
                                    break;
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                    displacementByte = (sbyte)memory[PC + 1];
                                    int displacementConditional = displacementByte;
                                    operation = "JR " + table_cc[y - 4] + "," + displacementConditional;
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
                                        PC = (ushort)(PC + (displacementConditional + 2));
                                        opLength = 0;
                                    } else {
                                        // DELETE THIS
                                        //Console.WriteLine("DIDNT JUMP");
                                    }
                                    cycleLength = 8;
                                    break;
                            }
                            break;
                        // Z is 1
                        case 1:
                            switch (q) {
                                case 0:
                                    //x0z1q0
                                    operation = "LD " + table_rp_names[p] + "," + nn.ToString("X");
                                    //op_LD(table_rp[p], romData[memPosition + 1] + romData[memPosition + 2]);
                                    opLength = 3;
                                    //data[0] = memory[PC + 1];
                                    //data[1] = memory[PC + 2];
                                    // TODO: FIX THIS - switch statement which could be a table_rp function thing.
                                    // Cant return ref to HL  because it doesn't like that - it's fine with SP because it doesnt have custom get/set.
                                    // Maybe return an anonymous function which has get and set for all of them?
                                    set_table_rp(p, nn);
                                    //SP = Convert.ToUInt16(data1 + data2 << 8);
                                    //TEMPORARY CODE
                                    unknownOP = false;
                                    cycleLength = 12;
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
                                            HL += 1;
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
                                            HL += 1;
                                            break;
                                        case 3:
                                            operation = "LD A,(HL-)";
                                            A = memory[HL];
                                            HL -= 1;
                                            break;
                                    }
                                    break;
                            }
                            cycleLength = 8;
                            break;
                        case 3:
                            // Z is 3
                            opLength = 1;
                            unknownOP = false;
                            int numToAdd = 0;
                            switch (q) {
                                case 0:
                                    operation = "INC " + table_rp_names[p];
                                    numToAdd = 1;
                                    break;
                                case 1:
                                    operation = "DEC " + table_rp_names[p];
                                    numToAdd = -1;
                                    break;
                            }
                            set_table_rp(p, (ushort)(get_table_rp(p) + numToAdd));
                            cycleLength = 8;
                            break;
                        case 4:
                            // Z is 4
                            operation = "INC " + table_r[y];
                            opLength = 1;
                            flag_n = false;
                            byte added = get_table_r(y);
                            AddandCarry(ref added, 0b1);
                            set_table_r(y, added);
                            flag_z = (added == 0);
                            unknownOP = false;
                            cycleLength = 4;
                            if (y == 6)
                            {
                                cycleLength = 12;
                            }
                            break;
                        case 5:
                            // Z is 5
                            operation = "DEC " + table_r[y];
                            opLength = 1;
                            flag_n = true;
                            byte subtracted = get_table_r(y);
                            SubtractAndCarry(ref subtracted, 0b1);
                            set_table_r(y, subtracted);
                            flag_z = (subtracted == 0);
                            unknownOP = false;
                            cycleLength = 4;
                            if(y == 6)
                            {
                                cycleLength = 12;
                            }
                            break;
                        //Z is 6
                        case 6:
                            //System.Exception: 'Was not able to handle operation:38 if applicable, operation is known as:  | x0 | y4 | z6 | p2 | q0'
                            operation = "LD " + table_r[y] + "," + n;
                            opLength = 2;
                            unknownOP = false;
                            //data[0] = memory[PC + 1];
                            set_table_r(y, n);
                            cycleLength = 8;
                            if(y == 6)
                            {
                                cycleLength = 12;
                            }
                            break;
                        case 7:
                            // Z is 7
                            opLength = 1;
                            switch(y) {
                                case 0:
                                    operation = "RLCA";
                                    break;
                                case 1:
                                    operation = "RRCA";
                                    break;
                                case 2:
                                    operation = "RLA";
                                    bool zState = flag_z;
                                    // 2(RL) and 7(A)
                                    do_rot(2,7);
                                    // Since Z flag is unaffected.
                                    flag_z = zState;
                                    unknownOP = false;
                                    break;
                                case 3:
                                    operation = "RRA";
                                    break;
                                case 4:
                                    operation = "DAA";
                                    break;
                                case 5:
                                    operation = "CPL";
                                    break;
                                case 6:
                                    operation = "SCF";
                                    break;
                                case 7:
                                    operation = "CCF";
                                    break;

                            }
                            cycleLength = 4;
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
                    cycleLength = 4;
                    if(y == 6 || z == 6)
                    {
                        cycleLength = 8;
                    }
                    break;
                // X is 2
                case 2:
                    operation = table_alu[y] + " " + table_r[z];
                    // Is the value from the HL pointer
                    if(z == 6)
                    {
                        cycleLength = do_alu(y, get_table_r(z),true);
                    } else
                    {
                        cycleLength = do_alu(y, get_table_r(z),false);
                    }
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
                                    //data[0] = memory[PC + 1];
                                    //data[1] = memory[memPosition + 1];
                                    operation = "LD(0xFF00 + " + n + "),A";
                                    opLength = 2;
                                    memory[0xFF00 + n] = A;
                                    unknownOP = false;
                                    cycleLength = 8;
                                    break;
                                case 5:

                                    break;
                                case 6:
                                    //data[0] = memory[PC + 1];
                                    //data[1] = memory[memPosition + 1];
                                    operation = "LD A,(0xFF00 + " + n + ")";
                                    opLength = 2;
                                    A = memory[0xFF00 + n];
                                    unknownOP = false;
                                    cycleLength = 8;
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
                                    cycleLength = 12;
                                    break;
                                case 1:
                                    switch(p) {
                                        case 0:
                                            operation = "RET";
                                            opLength = 0;
                                            PC = (ushort)(memory[SP] + (memory[SP + 1] * Math.Pow(2, 8)));
                                            SP += 2;
                                            unknownOP = false;
                                            cycleLength = 8;
                                            break;
                                        case 1:

                                            break;
                                        case 2:

                                            break;
                                        case 3:

                                            break;
                                    }
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
                                    cycleLength = 8;
                                    break;
                                case 5:
                                    operation = "LD " + String.Format("{0:X}", nn) + ",A";
                                    memory[nn] = A;
                                    unknownOP = false;
                                    opLength = 3;
                                    cycleLength = 16;
                                    break;
                                case 6:

                                    break;
                                case 7:

                                    break;
                            }
                            break;
                        case 3:
                            // Z is 3
                            switch(y)
                            {
                                case 0:
                                // Y is 0
                                    break;
                                case 6:
                                    // DI
                                    break;
                                case 7:
                                    // EI
                                    break;
                            }
                            break;
                        case 4:

                            break;
                        case 5:
                            // Z is 5
                            switch (q) {
                                case 0:
                                    operation = "PUSH " + table_rp2_names[p];
                                    opLength = 1;
                                    unknownOP = false;
                                    SP -= 2;
                                    data = BitConverter.GetBytes(get_table_rp2(p));
                                    memory[SP] = data[0];
                                    memory[SP + 1] = data[1];
                                    cycleLength = 16;
                                    break;
                                case 1:
                                    if (p != 0) {
                                        throw new Exception("operation is removed (x3z5q1p1-3)");
                                    }
                                    //data[0] = memory[PC + 1];
                                    //data[1] = memory[PC + 2];
                                    // TODO: REPLACE WITH NN but it makes a different result so see why
                                    //ushort callValue = (ushort)(data[0] + (data[1] * Math.Pow(2, 8)));
                                    operation = "CALL " + nn;
                                    byte[] nextAddress = BitConverter.GetBytes(PC + 3);
                                    opLength = 0;
                                    SP -= 2;
                                    memory[SP] = nextAddress[0];
                                    memory[SP + 1] = nextAddress[1];
                                    PC = nn;
                                    unknownOP = false;
                                    cycleLength = 12;
                                    break;
                            }
                            break;
                        case 6:
                            // Z is 6
                            opLength = 2;
                            data[0] = memory[PC + 1];
                            operation = table_alu[y] + data[0];
                            // the value is from a pointer
                            cycleLength = do_alu(y, data[0],true);
                            unknownOP = false;
                            break;
                        case 7:

                            break;
                    }
                    break;
            }
        }
        if(cycleLength < 0)
        {
            throw new Exception("Cycle length shouldn't be less than 0 (haven't implemented the cycle length for current opcode");
        }
        // 16 bit STORED AS LEAST SIGNIFICANT BIT FIRST
        if (unknownOP) {
            throw new Exception("Was not able to handle operation:" + opcode + " if applicable, operation is known as: " + operation + " | x" + x + " | y" + y + " | z" + z + " | p" + p + " | q" + q);
        }
        PC += opLength;
        //cycleLength = 10;
        lcd.ProcessGraphics(cycleLength);
        A = A;
        //Console.WriteLine(operation + " PC = " + memPosition);
    }


}
