using DragonBallKaiUBDLib;
using Mono.Options;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace DragonBallKaiUBDUtility
{
    public class ConvertDsoCommand : Command
    {
        private string _dsoFile, _outputFile, _inputDirectory, _outputDirectory;
        public ConvertDsoCommand() : base("convert-dso", "Converts a DSO texture into a PNG")
        {
            Options = new()
            {
                { "i|input=", "Input DSO file or directory", i => _dsoFile = i },
                { "o|output=", "Output PNG file or directory", o => _outputFile = o },
                { "id|input-dir=", "Input directory containing DSO files", id => _inputDirectory = id },
                { "od|output-dir=", "Output directory for PNG files", od => _outputDirectory = od }
            };

            /*Options = new()
            {
                { "i|input=", "Input DSO file", i => _dsoFile = i },
                { "o|output=", "Output PNG file", o => _outputFile = o },
            };*/
        }

        public override int Invoke(IEnumerable<string> arguments)
        {
            Options.Parse(arguments);

            if (!string.IsNullOrEmpty(_inputDirectory) && !string.IsNullOrEmpty(_outputDirectory))
            {
                ProcessDirectory(_inputDirectory, _outputDirectory);
            }
            else if (!string.IsNullOrEmpty(_dsoFile) && !string.IsNullOrEmpty(_outputFile))
            {
                ProcessFile(_dsoFile, _outputFile);
            }
            else
            {
                Console.WriteLine("Invalid arguments. Please specify either input/output files or input/output directories.");
                return 1;
            }

            return 0;

            /*DsoTexture dso = new(File.ReadAllBytes(_dsoFile));
            File.WriteAllBytes(_outputFile, dso.Data);
            //using FileStream fs = File.Create(_outputFile);
            //dso.GetImage().Encode(fs, SKEncodedImageFormat.Png, 1);

            return 0;*/
        }

        private void ProcessFile(string inputFile, string outputFile)
        {
            DsoTexture dso = new(File.ReadAllBytes(inputFile));
            File.WriteAllBytes(outputFile, dso.Data);
            //using FileStream fs = File.Create(_outputFile);
            //dso.GetImage().Encode(fs, SKEncodedImageFormat.Png, 1);
        }

        private void ProcessDirectory(string inputDirectory, string outputDirectory)
        {
            var dsoFiles = Directory.GetFiles(inputDirectory, "*.dso");
            foreach (var dsoFile in dsoFiles)
            {
                var outputFileName = Path.GetFileNameWithoutExtension(dsoFile) + ".dso";
                var outputFilePath = Path.Combine(outputDirectory, outputFileName);
                ProcessFile(dsoFile, outputFilePath);
            }
        }
    }
}
