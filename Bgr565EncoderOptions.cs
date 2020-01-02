namespace RawConverter
{
    internal class Bgr565EncoderOptions
    {
        public byte Compress { get; set; }
        public byte TransparentColor { get; set; }

        /// <summary>
        /// Get the Econding format 
        /// 7 6 5 4 3 2 1 0 b <br>
        /// . . . . . . 0 0 > Not compressed 
        /// . . . . . . 0 1 > Compact repeated color
        /// . . . . . . 1 0 > TODO: Compact repeated color + optimize non repeated
        /// . . . . . . 1 1 > free for use
        /// . . . . 0 0 . . > No transparent color
        /// . . . . 0 1 . . > use black (0x0000) as transparent color
        /// . . . . 1 0 . . > use magic pink (0xF81F) as transparent color
        /// . . . . 1 1 . . > free for use
        /// </summary>
        /// <returns></returns>
        public byte EncodingFormat => (byte)((Compress & 0x03) | ((TransparentColor & 0x03) << 2));
    }
}