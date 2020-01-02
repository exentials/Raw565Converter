using SixLabors.ImageSharp.PixelFormats;

namespace RawConverter
{
    internal static class Utils
    {
        public static ushort Rgb888toRgb565(byte red, byte green, byte blue)
        {
            return (ushort)(
                (((blue >> 3) & 0x1f) << 11)
                | (((green >> 2) & 0x3f) << 5)
                | ((red >> 3) & 0x1f)
            );
        }

        public static ushort GetPixel565(IPixel value)
        {
            Rgba32 pixel = new Rgba32();
            value.ToRgba32(ref pixel);
            return Rgb888toRgb565(pixel.B, pixel.G, pixel.R);
        }

    }
}