using DragonBallKaiUBDLib;
using Mono.Options;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;

namespace DragonBallKaiUBDUtility
{
    public class CompressDsoCommand : Command
    {
        private string _dsoFile, _outputFile;
        public CompressDsoCommand() : base("compress-dso", "Compresses a DSO texture into a DSO")
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

            DsoTextureToComp dso = new(File.ReadAllBytes(_dsoFile));
            File.WriteAllBytes(_outputFile, dso.Data);
            //using FileStream fs = File.Create(_outputFile);
            //dso.GetImage().Encode(fs, SKEncodedImageFormat.Png, 1);

            return 0;
        }
    }
}
