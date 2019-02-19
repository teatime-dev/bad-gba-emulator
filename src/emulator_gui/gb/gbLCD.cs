using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class gbLCD {
    static int SCANLINE_TIME = 456; // Time to do a scanline in cycles
    static int LCD_DISPLAY = 0xFF40; // LCD Display Status byte
    static int LCD_STATUS = 0xFF41; // Status byte
    static int SCANLINE_MEM = 0xFF44; // Current scanline address
    static int LY_COMPARE_MEM = 0xFF45; // Coincidence byte register
    static int mode2end = SCANLINE_TIME - 80;
    static int mode3end = mode2end - 172;
    private int current_time;
    byte status;
    bool VBlankInterrupt,LCDInterrupt;
    int current_mode;
    bool mode0enable;
    bool mode1enable;
    bool mode2enable;
    byte currentScanline;
    public gbLCD(gbCPU cpu) {
        cpu.memory.memoryRaw[SCANLINE_MEM] = 0;
        current_time = SCANLINE_TIME;
        }
    public bool LCDEnabled(ref gbCPU cpu) {
        if (cpu == null) {
            throw new ArgumentNullException(nameof(cpu));
        }

        return gbCPU.GetBit(cpu.memory[LCD_DISPLAY], 7);
    }
    public void ProcessGraphics(int cycleLength, ref gbCPU cpu) {
        status = cpu.memory[LCD_STATUS];
        if(!LCDEnabled(ref cpu)) {
            current_time = SCANLINE_TIME;
            cpu.memory[SCANLINE_MEM] = 0;
            status &= 0b1111_1100;
            status += 1;
            cpu.memory[LCD_STATUS] = status;
            return;
        }
        VBlankInterrupt = false;
        LCDInterrupt = false;
        current_time -= cycleLength;
        currentScanline = cpu.memory[SCANLINE_MEM];
        /** DO LCD STATUS STUFF **/
        current_mode = status & 0b0000_0011;
        mode0enable = gbCPU.GetBit(status, 3);
        mode1enable = gbCPU.GetBit(status, 4);
        mode2enable = gbCPU.GetBit(status, 5);
        if(currentScanline < 144) {
            if (current_time > mode2end && current_mode != 2) {
                //starting new scanline
                current_mode = 2;
                if (mode2enable) {
                    LCDInterrupt = true;
                }
            }
            if (current_time > mode3end && current_time <= mode2end && current_mode != 3) {
                //transferring to lcd driver
                current_mode = 3;
            }
            if (current_time <= mode3end && current_mode != 0) {
                // H Blank
                current_mode = 0;
                if (mode0enable) {
                    LCDInterrupt = true;
                }
            }
        }
        // Coincidence flag Check
        if(currentScanline == cpu.memory[LY_COMPARE_MEM]) {
            gbCPU.SetBit(ref status, 2);
            if (gbCPU.GetBit(status, 6)) {
                LCDInterrupt = true;
            }
        }
        /** END LCD STATUS STUFF **/
        if (current_time < 0) {
            //A scanline has been drawn
            currentScanline += 1;
            if(currentScanline == 144) {
                //VBLANK interrrupt
                if(mode1enable) {
                    VBlankInterrupt = true;
                }
                status &= 0b1111_1100;
                status += 1;
                // Set status to 1
            }
            else if(currentScanline > 153) {
                currentScanline = 0;
            } else if (currentScanline < 144) {
                //Draw the line
            }
            cpu.memory.memoryRaw[SCANLINE_MEM] = currentScanline;
        }

        if(VBlankInterrupt) {
            cpu.interrupt(gbCPU.INTERRUPTS.VBLANK);
        }
        if (LCDInterrupt) {
            cpu.interrupt(gbCPU.INTERRUPTS.LCDC);
        }
        cpu.memory[LCD_STATUS] = status;
    }
    }
