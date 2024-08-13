using DragonBallKaiUBDLib;
using Mono.Options;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;

namespace DragonBallKaiUBDUtility
{
    public class ConvertDsoCommand : Command
    {
        private string _dsoFile, _outputFile;
        public ConvertDsoCommand() : base("convert-dso", "Converts a DSO texture into a PNG")
        {
            Options = new()
            {
                { "i|input=", "Input DSO file", i => _dsoFile = i },
                { "o|output=", "Output PNG file", o => _outputFile = o },
            };
        }

        public override int Invoke(IEnumerable<string> arguments)
        {
            Options.Parse(arguments);

            DsoTexture dso = new(File.ReadAllBytes(_dsoFile));
            File.WriteAllBytes(_outputFile, dso.Data);
            //using FileStream fs = File.Create(_outputFile);
            //dso.GetImage().Encode(fs, SKEncodedImageFormat.Png, 1);

            return 0;
        }
    }
}
