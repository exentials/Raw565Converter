using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using SixLabors.ImageSharp;

namespace RawConverter
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default
                .ParseArguments<CommanLineOptions>(args).MapResult(
                    (CommanLineOptions opt) => RunOptionsAndReturnExitCode(opt),
                    errs => 1
                );
        }

        static int RunOptionsAndReturnExitCode(CommanLineOptions opt)
        {
            string path = opt.ImagesPath;
            string destPath;

            if (string.IsNullOrEmpty(opt.OutputPath))
            {
                destPath = Path.Combine(path, "raw");
            }
            else
            {
                destPath = opt.OutputPath;
            }
            Directory.CreateDirectory(destPath);

            Bgr565EncoderOptions options = new Bgr565EncoderOptions
            {
                Compress = opt.Compress,
                TransparentColor = opt.Transparent
            };

            var files = Directory.GetFiles(path, "*.png").OrderBy(t => t);

            if (opt.Single)
            {
                using StreamWriter rawImageDefinition = File.CreateText(Path.Combine(destPath, "RawImagesOffset.h"));
                string fileName = "images.raw";
                string destinationFile = Path.Combine(destPath, fileName);
                using FileStream rawFileStream = File.Create(destinationFile);
                long offset = 0;
                foreach (string file in files)
                {
                    string fileDef = GetDefNameFormFile(file);
                    using MemoryStream ms = new MemoryStream();
                    RawInfo info = Convert(file, ms, options);
                    byte[] buffer = ms.ToArray();
                    rawFileStream.Write(buffer);
                    rawImageDefinition.WriteLine($"#define {fileDef}\t0x{offset:x8}\t// {info.Width}x{info.Height}");
                    offset += buffer.Length;
                }
            }
            else
            {
                using StreamWriter rawImageDefinition = File.CreateText(Path.Combine(destPath, "RawImages.h"));
                int rawCount = 0;
                foreach (string file in files)
                {
                    string fileDef = GetDefNameFormFile(file);
                    string fileName = $"{rawCount:x2}";
                    string destinationFile = Path.Combine(destPath, fileName);
                    RawInfo info = Convert(file, destinationFile, options);
                    rawImageDefinition.WriteLine($"#define {fileDef}\t\"/{fileName}\"\t// {info.Width}x{info.Height} ");
                    rawCount++;
                }
            }
            return 0;
        }

        private static RawInfo Convert(string file, string fileDestination, Bgr565EncoderOptions options)
        {
            using FileStream rawFileStream = File.Create(fileDestination);
            return Convert(file, rawFileStream, options);
        }

        private static RawInfo Convert(string file, Stream streamDestination, Bgr565EncoderOptions options)
        {
            using Image image = Image.Load(file);
            image.Save(streamDestination, new Bgr565Encoder(options));
            return new RawInfo { Width = image.Width, Height = image.Height };
        }

        private static string GetDefNameFormFile(string file)
        {
            return Path.GetFileNameWithoutExtension(file).Replace(" ", "").Replace("-", "_");
        }

    }
}
