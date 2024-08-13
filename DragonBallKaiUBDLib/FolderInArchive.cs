using System.Collections.Generic;

namespace DragonBallKaiUBDLib
{
    public class FolderInArchive(IEnumerable<byte> data, int offset)
    {
        public int FirstFileIndex { get; set; } = IO.ReadInt(data, offset + 0x00);
        public int NumFiles { get; set; } = IO.ReadInt(data, offset + 0x04);
        public string Name { get; set; } = IO.ReadAsciiString(data, IO.ReadInt(data, offset + 0x08) & 0x7FFFFFFF);
        public List<FileInArchive> Files { get; set; } = [];
    }
}
