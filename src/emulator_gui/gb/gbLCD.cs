using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class gbLCD
{
	/// <summary>
	/// Lots of this stuff is thanks to the fine fellow at
	/// http://codeslinger.co.uk
	/// doesn't seem to be a proper reference elsewhere :)
	/// </summary>
	static int SCANLINE_TIME = 456; // Time to do a scanline in cycles
	/// <summary>
	/// LCD Display bits, 0xFF40
	/// </summary>
	static int LCD_DISPLAY = 0xFF40; // LCD Display Status byte
	/// <summary>
	/// LCD Status/Control, 0xFF41
	/// </summary>
	static int LCD_STATUS = 0xFF41; // Status byte
				/* 
				Bit 7 - LCD Display Enable (0=Off, 1=On)
				Bit 6 - Window Tile Map Display Select (0=9800-9BFF, 1=9C00-9FFF)
				Bit 5 - Window Display Enable (0=Off, 1=On)
				Bit 4 - BG & Window Tile Data Select (0=8800-97FF, 1=8000-8FFF)
				Bit 3 - BG Tile Map Display Select (0=9800-9BFF, 1=9C00-9FFF)
				Bit 2 - OBJ (Sprite) Size (0=8x8, 1=8x16)
				Bit 1 - OBJ (Sprite) Display Enable (0=Off, 1=On)
				Bit 0 - BG Display (for CGB see below) (0=Off, 1=On)
				*/
	/// <summary>
	/// Current Scanline address 0xFF44
	/// </summary>
	static int SCANLINE_MEM = 0xFF44; // Current scanline address
	static int LY_COMPARE_MEM = 0xFF45; // Coincidence byte register
	/// <summary>
	/// Y position to start drawing viewing area 0xFF42
	/// </summary>
	static int SCROLL_Y = 0xFF42;
	/// <summary>
	/// X position to start drawing viewing area 0xFF43
	/// </summary>
	static int SCROLL_X = 0xFF43;
	/// <summary>
	/// Y position to actually show in the window from the scrolling area 0xFF4A
	/// </summary>
	static int WINDOW_Y = 0xFF4A;
	/// <summary>
	/// X position to actually show in the window from the scrolling area 0xFF4B
	/// </summary>
	static int WINDOW_X = 0xFF4B;
	static int mode2end = SCANLINE_TIME - 80;
	static int mode3end = mode2end - 172;
	private int current_time;
	byte status;
	bool VBlankInterrupt, LCDInterrupt;
	int current_mode;
	bool mode0enable;
	bool mode1enable;
	bool mode2enable;
	byte currentScanline;
	gbCPU cpu;
	public gbLCD(gbCPU cpu)
	{
		cpu.memory.memoryRaw[SCANLINE_MEM] = 0;
		current_time = SCANLINE_TIME;
		this.cpu = cpu;
	}
	public bool LCDEnabled(gbCPU cpu)
	{
		if (cpu == null)
		{
			throw new ArgumentNullException(nameof(cpu));
		}

		return gbCPU.GetBit(cpu.memory[LCD_DISPLAY], 7);
	}
	public void ProcessGraphics(int cycleLength)
	{
		status = cpu.memory[LCD_STATUS];
		if (!LCDEnabled(cpu))
		{
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
		if (currentScanline < 144)
		{
			if (current_time > mode2end && current_mode != 2)
			{
				//starting new scanline
				current_mode = 2;
				if (mode2enable)
				{
					LCDInterrupt = true;
				}
			}
			if (current_time > mode3end && current_time <= mode2end && current_mode != 3)
			{
				//transferring to lcd driver
				current_mode = 3;
			}
			if (current_time <= mode3end && current_mode != 0)
			{
				// H Blank
				current_mode = 0;
				if (mode0enable)
				{
					LCDInterrupt = true;
				}
			}
		}
		// Coincidence flag Check
		if (currentScanline == cpu.memory[LY_COMPARE_MEM])
		{
			gbCPU.SetBit(ref status, 2);
			if (gbCPU.GetBit(status, 6))
			{
				LCDInterrupt = true;
			}
		}
		/** END LCD STATUS STUFF **/

		if (current_time < 0)
		{
			//A scanline has been drawn
			currentScanline += 1;
			if (currentScanline == 144)
			{
				//VBLANK interrrupt
				if (mode1enable)
				{
					VBlankInterrupt = true;
				}
				status &= 0b1111_1100;
				status += 1;
				// Set status to 1
			}
			else if (currentScanline > 153)
			{
				currentScanline = 0;
			}
			else if (currentScanline < 144)
			{
				//Draw the line
				drawScanline();
			}
			cpu.memory.memoryRaw[SCANLINE_MEM] = currentScanline;
		}

		if (VBlankInterrupt)
		{
			cpu.interrupt(gbCPU.INTERRUPTS.VBLANK);
		}
		if (LCDInterrupt)
		{
			cpu.interrupt(gbCPU.INTERRUPTS.LCDC);
		}
		cpu.memory[LCD_STATUS] = status;
	}

	private void drawScanline()
	{
		byte displayStatus = cpu.memory[LCD_DISPLAY];
		// if background is enabled
		if (gbCPU.GetBit(displayStatus, 0))
		{
			drawTiles();
		}
		// if sprites are enabled
		if (gbCPU.GetBit(displayStatus, 1))
		{
			drawSprites();
		}
	}

	private void drawSprites()
	{
		throw new NotImplementedException();
	}

	private void drawTiles()
	{
		ushort tileData = 0;
		ushort backgroundMemory = 0;
		bool unsigned = true; // is the tile location stored as unsigned or signed?

		byte scrollY = cpu.memory[SCROLL_Y];
		byte scrollX = cpu.memory[SCROLL_X];
		byte windowY = cpu.memory[WINDOW_Y];
		byte windowX = cpu.memory[WINDOW_X - 7];

		bool usingWindow = false;

		if(gbCPU.GetBit(cpu.memory[LCD_STATUS],5))
		{
			// Window is enabled
			
			//are we drawing inside the window?
			if(windowY <= cpu.memory[SCANLINE_MEM])
			{
				usingWindow = true;
			}
		}

		if(gbCPU.GetBit(cpu.memory[LCD_STATUS], 4))
		{
			//tile data starts at 0x8000
			tileData = 0x8000;
		} else
		{
			//tile data starts at 0x8800
			// using signed bytes
			tileData = 0x8000;
			unsigned = false;
		}
		// which background memory is being used?
		// the memory which has pointers to each tile, so tiles can be repeated
		if (!usingWindow)
		{
			
			if(gbCPU.GetBit(cpu.memory[LCD_STATUS],3))
			{
				backgroundMemory = 0x9C00;
			} else
			{
				backgroundMemory = 0x9800;
			}

		} else
		{
			if (gbCPU.GetBit(cpu.memory[LCD_STATUS], 6))
			{
				backgroundMemory = 0x9C00;
			}
			else
			{
				backgroundMemory = 0x9800;
			}
		}

		byte yPos = 0;

		// ypos is used to calculate which of the 32 vertical tiles the current scanline is drawing
		if(!usingWindow)
		{
			yPos = Convert.ToByte(scrollY + cpu.memory[SCANLINE_MEM]);
		} else
		{
			yPos = Convert.ToByte(cpu.memory[SCANLINE_MEM] - windowY);
		}
		//Which row of the tiles is the scanline on right now?
		ushort tileRow = Convert.ToUInt16(((yPos / 8) * 32));

		throw new NotImplementedException();
	}
}
