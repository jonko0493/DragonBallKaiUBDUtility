using DragonBallKaiUBDLib;
using Mono.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DragonBallKaiUBDUtility
{
    public class DsaUnpackCommand : Command
    {
        private string _dsaFile, _outputDirectory;
        public DsaUnpackCommand() : base("dsa-unpack", "Unpacks a DSA archive")
        {
            Options = new()
            {
                { "i|d|input-archive|dsa=", "The input DSA archive to unpack", i => _dsaFile = i },
                { "o|output|output-dir=", "The output directory to unpack to", o => _outputDirectory = o },
            };
        }

        public override int Invoke(IEnumerable<string> arguments)
        {
            Options.Parse(arguments);

            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }

            DsaArchive dsa = new(File.ReadAllBytes(_dsaFile));
            List<string> csvLines = [.. dsa.Folders.Select((f, i) => $"0x{i * 0x0C + DsaArchive.START_OFFSET:X8},{f.Name},{f.FirstFileIndex},{f.NumFiles}")];
            foreach (FolderInArchive folder in dsa.Folders)
            {
                csvLines.Add(folder.Name);
                csvLines.AddRange(folder.Files.Select((f, i) => $"0x{(i + folder.FirstFileIndex) * 0x14 + dsa.FileEntryTableOffset},{f.Name},{f.CompressedLength},0x{f.PrepopulatedNameOffsetOrSomething:X8},{f.DecompressedLength},{f.Offset}"));
                if (!Directory.Exists(Path.Combine(_outputDirectory, folder.Name[1..])))
                {
                    Directory.CreateDirectory(Path.Combine(_outputDirectory, folder.Name[1..]));
                }
                foreach (FileInArchive file in folder.Files)
                {
                    File.WriteAllBytes(Path.Combine(_outputDirectory, folder.Name[1..], file.Name), [.. file.CompressedData]);
                }
            }
            File.WriteAllLines(Path.Combine(_outputDirectory, "files.csv"), csvLines);

            return 0;
        }
    }
}
