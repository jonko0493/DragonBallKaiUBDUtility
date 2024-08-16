using DragonBallKaiUBDLib;
using Mono.Options;
using System.Collections.Generic;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DragonBallKaiUBDUtility
{
    public class ConvertDsbCommand : Command
    {
        private string _input, _output;
        public ConvertDsbCommand() : base("convert-dsb", "Converst a DSB or directory of DSBs")
        {
            Options = new()
            {
                { "i|input=", "Input DSB file or directory of DSBs to convert", i => _input = i },
                { "o|output=", "Output directory to write the file to", o => _output = o },
            };
        }

        public override int Invoke(IEnumerable<string> arguments)
        {
            Options.Parse(arguments);

            if (!Directory.Exists(_output))
            {
                Directory.CreateDirectory(_output);
            }

            if (Path.HasExtension(_input))
            {
                ConvertDsb(_input, _output);
            }
            else
            {
                foreach (string file in Directory.GetFiles(_input))
                {
                    ConvertDsb(file, _output);
                }
            }

            return 0;
        }

        private void ConvertDsb(string input, string outputDir)
        {
            string outputFile = Path.Combine(outputDir, Path.GetFileName(input));
            DsbBackground dsb = new(File.ReadAllBytes(input), input);
            File.WriteAllBytes(outputFile, dsb.Data);
            using FileStream fs = File.Create(Path.Combine(Path.GetDirectoryName(outputFile), $"{Path.GetFileNameWithoutExtension(outputFile)}.png"));
            dsb.GetImage().Encode(fs, SkiaSharp.SKEncodedImageFormat.Png, 1);
        }
    }
}
