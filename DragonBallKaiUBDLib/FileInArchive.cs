using AuroraLib.Compression.Algorithms;
using System.Collections.Generic;
using System.Linq;

namespace DragonBallKaiUBDLib
{
    public class FileInArchive
    {
        public string Name { get; set; }
        public IEnumerable<byte> CompressedData { get; }
        public byte[] Data { get; set; }
        public int Offset { get; set; }
        public int CompressedLength { get; set; }

        public int PrepopulatedNameOffsetOrSomething { get; set; }
        public int DecompressedLength { get; set; }

        public FileInArchive(IEnumerable<byte> data, int entryOffset)
        {
            int nameLength = IO.ReadInt(data, entryOffset);
            PrepopulatedNameOffsetOrSomething = IO.ReadInt(data, entryOffset + 0x04);
            DecompressedLength = IO.ReadInt(data, entryOffset + 0x08);
            CompressedLength = IO.ReadInt(data, entryOffset + 0x0C);
            Offset = IO.ReadInt(data, entryOffset + 0x10);
            if (nameLength > 0)
            {
                Name = IO.ReadAsciiString(data, Offset);
            }
            else
            {
                Name = IO.ReadAsciiString(data, PrepopulatedNameOffsetOrSomething);
            }
            CompressedData = data.Skip(Offset + nameLength).Take(CompressedLength);
        }

        public void Decompress()
        {
            LZ10 lz10 = new();
            Data = lz10.Decompress(CompressedData.Skip(Offset + Name.Length + 1).Take(CompressedLength).ToArray());
        }
    }
}
