using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

namespace RawConverter
{
    internal class Bgr565Encoder : IImageEncoder
    {
        private readonly Bgr565EncoderOptions _options;

        public Bgr565Encoder() : this(new Bgr565EncoderOptions()) { }

        public Bgr565Encoder(Bgr565EncoderOptions options)
        {
            _options = options;
        }

        public void Encode<TPixel>(Image<TPixel> image, Stream stream) where TPixel : struct, IPixel<TPixel>
        {
            // 0000 : With-LO_Byte
            // 0001 : With-HI_Byte
            // 0002 : Height-LO_Byte
            // 0003 : Height-HI_Byte
            // 0004 : Image Encoding Format
            // 0005 -> Image data

            ushort width = (ushort)image.Width;
            ushort height = (ushort)image.Height;

            byte format = _options.EncodingFormat;

            using BinaryWriter bw = new BinaryWriter(stream);
            bw.Write(width);
            bw.Write(height);
            bw.Write(format);

            ushort[] pixels = image.GetPixelSpan().ToArray().Select(p => Utils.GetPixel565(p)).ToArray();

            if (_options.Compress > 0)
            {
                WriteImageCompressed(bw, pixels);
            }
            else
            {
                WriteImage(bw, pixels);
            }

        }

        private void WriteImageCompressed(BinaryWriter writer, ushort[] pixels)
        {
            int pixelOffset = 0;
            ushort maxConstantSize = ushort.MaxValue;
            int imageSize = pixels.Length;

            while (pixelOffset < imageSize)
            {
                ushort pixelColor = pixels[pixelOffset];
                ushort constantPixelCount = 0;
                while (
                    (pixelOffset + constantPixelCount < imageSize)
                    && (constantPixelCount < maxConstantSize)
                    && (pixels[pixelOffset + constantPixelCount] == pixelColor)
                )
                {
                    constantPixelCount++;
                }

                writer.Write(constantPixelCount);
                writer.Write(pixelColor);

                pixelOffset += constantPixelCount;
            }

        }

        private void WriteImage(BinaryWriter writer, ushort[] pixels)
        {
            foreach (var p in pixels.ToArray())
            {
                writer.Write(p);
            }
        }

    }
}