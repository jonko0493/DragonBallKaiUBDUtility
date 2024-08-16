using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonBallKaiUBDLib
{
    public class DsaArchive
    {
        public const string MAGIC = "DSA ";
        public const int START_OFFSET = 0x10;

        public int Unknown04 { get; set; }
        public int FirstFileOffset { get; set; }
        public int NumDirectories { get; set; }
        public int FileEntryTableOffset => START_OFFSET + 0x0C * NumDirectories;

        public List<FolderInArchive> Folders { get; set; } = [];

        public DsaArchive(IEnumerable<byte> data)
        {
            if (!Encoding.ASCII.GetString([.. data.Take(4)]).Equals(MAGIC))
            {
                throw new ArgumentException("File is not DSA archive!");
            }

            Unknown04 = IO.ReadInt(data, 0x04);
            FirstFileOffset = IO.ReadInt(data, 0x08);
            NumDirectories = IO.ReadInt(data, 0x0C);

            for (int i = 0; i < NumDirectories; i++)
            {
                Folders.Add(new(data, START_OFFSET + i * 0x0C));
            }
            foreach (FolderInArchive folder in Folders)
            {
                for (int i = 0; i < folder.NumFiles; i++)
                {
                    folder.Files.Add(new(data, FileEntryTableOffset + 0x14 * (i + folder.FirstFileIndex)));
                }
            }
        }
    }
}
