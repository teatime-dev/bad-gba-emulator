﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class gbMemory {
    // just a random sequence of bytes am i right lads
    public enum MBC {ROMONLY,MBC1,MBC2,MBC3};
    static byte[] BIOS = new byte[] {
0x31, 0xFE, 0xFF, 0xAF, 0x21, 0xFF, 0x9F, 0x32, 0xCB, 0x7C, 0x20, 0xFB, 0x21, 0x26, 0xFF, 0x0E,
0x11,0x3E,0x80,0x32,0xE2,0x0C,0x3E,0xF3,0xE2,0x32,0x3E,0x77,0x77,0x3E,0xFC,0xE0
,0x47,0x11,0x04,0x01,0x21,0x10,0x80,0x1A,0xCD,0x95,0x00,0xCD,0x96,0x00,0x13,0x7B
,0xFE,0x34,0x20,0xF3,0x11,0xD8,0x00,0x06,0x08,0x1A,0x13,0x22,0x23,0x05,0x20,0xF9
,0x3E,0x19,0xEA,0x10,0x99,0x21,0x2F,0x99,0x0E,0x0C,0x3D,0x28,0x08,0x32,0x0D,0x20
,0xF9,0x2E,0x0F,0x18,0xF3,0x67,0x3E,0x64,0x57,0xE0,0x42,0x3E,0x91,0xE0,0x40,0x04
,0x1E,0x02,0x0E,0x0C,0xF0,0x44,0xFE,0x90,0x20,0xFA,0x0D,0x20,0xF7,0x1D,0x20,0xF2
,0x0E,0x13,0x24,0x7C,0x1E,0x83,0xFE,0x62,0x28,0x06,0x1E,0xC1,0xFE,0x64,0x20,0x06
,0x7B,0xE2,0x0C,0x3E,0x87,0xE2,0xF0,0x42,0x90,0xE0,0x42,0x15,0x20,0xD2,0x05,0x20
,0x4F,0x16,0x20,0x18,0xCB,0x4F,0x06,0x04,0xC5,0xCB,0x11,0x17,0xC1,0xCB,0x11,0x17
,0x05,0x20,0xF5,0x22,0x23,0x22,0x23,0xC9,0xCE,0xED,0x66,0x66,0xCC,0x0D,0x00,0x0B
,0x03,0x73,0x00,0x83,0x00,0x0C,0x00,0x0D,0x00,0x08,0x11,0x1F,0x88,0x89,0x00,0x0E
,0xDC,0xCC,0x6E,0xE6,0xDD,0xDD,0xD9,0x99,0xBB,0xBB,0x67,0x63,0x6E,0x0E,0xEC,0xCC
,0xDD,0xDC,0x99,0x9F,0xBB,0xB9,0x33,0x3E,0x3C,0x42,0xB9,0xA5,0xB9,0xA5,0x42,0x3C
,0x21,0x04,0x01,0x11,0xA8,0x00,0x1A,0x13,0xBE,0x20,0xFE,0x23,0x7D,0xFE,0x34,0x20
,0xF5,0x06,0x19,0x78,0x86,0x23,0x05,0x20,0xFB,0x86,0x20,0xFE,0x3E,0x01,0xE0,0x50
};
    public MBC mbc = MBC.ROMONLY;
    public bool timer = false;
    public bool ram = false;
    public bool battery = false;
    public byte mbcByte;
    public byte[] memoryRaw;
    private byte[] romData;
    private bool biosEnabled = true;
    public byte this[int key] {
        get {
            if (biosEnabled && key < 256) {
                return BIOS[key];
            }
            return memoryRaw[key];
        }
        set {
            if(biosEnabled && key < 256) {

            } else {
                if (key == 0xFF44)
                { // Scanline counter, if written to it's set to 0
                    memoryRaw[key] = 0;
                }
                else if (key == 0xFF46)
                { // Direct Memory Access Transfer
                    ushort addressToWrite = (ushort)(value << 8);
                    for( int i = 0; i < 0xA0; i++ )
                    {
                        // damn this feels strange but it should work??
                        this[0xFE00 + i] = this[addressToWrite + i];
                    }
                }
                else
                {
                    memoryRaw[key] = value;
                    if (biosEnabled)
                    {
                        if (memoryRaw[0xFF50] > 0)
                        {
                            biosEnabled = false;
                            Console.WriteLine("BIOS disabled.");
                        }
                    }
                }
            }

        }
    }
    public gbMemory() {
        memoryRaw = new byte[0x10000];
        //Array.Copy(BIOS, memoryRaw, BIOS.Length);
    }

    public void loadCatridge(byte[] romFile) {
#warning not properly done, doesnt manage catridge banks
        romData = new byte[romFile.Length];
        Array.Copy(romFile, romData,romFile.Length);
        bool knownMBC = false;
        mbcByte = romData[0x147];
        Console.WriteLine("MBC Type is: " + String.Format("#{0:X}", mbcByte));
        switch(mbcByte) {
            case 0x00:
                //ROM ONLY
                knownMBC = true;
                mbc = MBC.ROMONLY;
                break;
            case 0x13:
                knownMBC = true;
                //MBC3+RAM+BATTERY
                mbc = MBC.MBC3;
                ram = true;
                battery = true;
                break;
        }
        if(!knownMBC) {
            throw new Exception("MBC Type is not known:" + String.Format("#{0:X}", mbcByte));
        }
        switch(mbc) {
            case MBC.MBC1:

                break;
            case MBC.MBC2:

                break;
            case MBC.MBC3:
                Array.Copy(romData, 0, memoryRaw, 0, 0x4000);
                break;
            case MBC.ROMONLY:
                Array.Copy(romData, memoryRaw, romData.Length);
                break;
        }
    }
}
