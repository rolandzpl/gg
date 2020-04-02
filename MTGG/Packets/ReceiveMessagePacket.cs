using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
 
namespace MTGG.Packets
{
    internal class ReceiveMessagePacket : Packet
    {
        public ReceiveMessagePacket()
        {
            this.attributes = new List<RichTextFormat>();
            this.recipients = new List<uint>();
        }
 
        public uint Sender { get; private set; }
 
        public uint[] Recipients
        {
            get { return this.recipients.ToArray(); }
        }
 
        public uint Sequence { get; private set; }
 
        public DateTime Time { get; private set; }
 
        public ClassMessage Class { get; private set; }
 
        public string HtmlMessage { get; private set; }
 
        public string PlainMessage { get; private set; }
 
        public RichTextFormat[] Attributes
        {
            get { return this.attributes.ToArray(); }
        }
 
        public override void Read()
        {
            base.Read();
            this.Sender = reader.ReadUInt32();
            this.Sequence = reader.ReadUInt32();
            uint time = reader.ReadUInt32();
            this.Time = UTC.UnixTimestampToDate(time);
            this.Class = (ClassMessage)reader.ReadUInt32();
            uint offsetPlain = reader.ReadUInt32();
            uint offsetAttributes = reader.ReadUInt32();
 
            long currentPos = this.reader.BaseStream.Position - 8;
 
            List<byte> message = new List<byte>();
            byte tmp;
 
            if (currentPos != offsetPlain)
            {
                while ((tmp = reader.ReadByte()) != 0)
                {
                    message.Add(tmp);
                }
                this.HtmlMessage = UTF8Encoding.UTF8.GetString(message.ToArray());
            }
 
            message.Clear();
            while ((tmp = reader.ReadByte()) != 0)
            {
                message.Add(tmp);
            }
            this.PlainMessage = Encoding.GetEncoding(1250).GetString(message.ToArray());
 
            message.Clear();
 
            while (reader.PeekChar() != -1)
            {
                byte attributeType = this.reader.ReadByte(); //0x02 -> attr
 
                if (attributeType == 0x01)
                {
                    uint count = this.reader.ReadUInt32();
                    for (uint i = 0; i < count; ++i)
                    {
                        this.recipients.Add(this.reader.ReadUInt32());
                    }
                }
                else if(attributeType == 0x02)
                {
                    ushort length = this.reader.ReadUInt16(); // length
                    long range = this.reader.BaseStream.Position + length;
                    while (this.reader.BaseStream.Position < range)
                    {
                        ushort position = reader.ReadUInt16();
                        FormatType type = (FormatType)reader.ReadByte();
                        RichTextFormat block = new RichTextFormat(position, type);
 
                        if (type.HasFlag(FormatType.FontColor))
                        {
                            block.Color = Color.FromArgb(reader.ReadByte(),
                                reader.ReadByte(), reader.ReadByte());
                        }
 
                        if (type.HasFlag(FormatType.Image))
                        {
                            RichTextImage image = new RichTextImage();
                            image.Size = this.reader.ReadUInt32();
                            image.CRC32 = this.reader.ReadUInt32();
                            block.Image = image;
                        }
                        this.attributes.Add(block);
                    }
                }
            }
            this.HtmlMessage = RTF.PlainToHtml(this.PlainMessage, this.Attributes);
        }
        private List<RichTextFormat> attributes;
        private List<uint> recipients;
    }
}
