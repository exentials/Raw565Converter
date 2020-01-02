using CommandLine;

namespace RawConverter
{
    public class CommanLineOptions
    {
        [Option('c', "compress", HelpText = "Use compressed image format")]
        public byte Compress { get; set; }
        [Option('t', "transparent", HelpText = "Set transparent color:\n0 - none\n1 - black (#000000)\n2 - magic pink (#FF00FF)")]
        public byte Transparent { get; set; }
        [Option('s', "single", HelpText = "Build a single file with offset header file definitions.")]
        public bool Single { get; set; }
        [Option('p', "path", Required = true, HelpText = "Path of PNG image files.")]
        public string ImagesPath { get; set; }
        [Option('o', "output-path", Default = null, HelpText = "Path of PNG image files.")]
        public string OutputPath { get; set; }
    }
}