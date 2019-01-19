using System;
using System.IO;

public class GBA
{
    byte[] romData;
    private CPU gbaCPU;
	public GBA(string ROM)
	{
        using (BinaryReader reader = new BinaryReader(File.Open(ROM, FileMode.Open))) {
            romData = reader.ReadBytes(int.MaxValue);
        }
            gbaCPU = new CPU(romData);
	}
}
