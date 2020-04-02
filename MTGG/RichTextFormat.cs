using System.Drawing;

namespace MTGG
{
    internal class RichTextFormat
    {
        public RichTextFormat(ushort position, FormatType type)
        {
            this.Position = position;
            this.FormatType = type;
        }

        public ushort Position { get; set; }

        public FormatType FormatType { get; set; }

        public Color Color { get; set; }

        public RichTextImage Image { get; set; }
    }

    internal class RichTextImage
    {
        public uint Size { get; set; }

        public uint CRC32 { get; set; }
    }
}