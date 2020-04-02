using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
 
namespace MTGG.Packets
{
    internal class SendMessagePacket : Packet
    {
        public SendMessagePacket(uint number, uint[] recipients, string html, string plain, uint sequence)
        {
            this.PacketType = PacketType.SendMessage;
            this.Class = ClassMessage.Chat;
            this.Recipient = number;
            this.sequence = sequence;
            this.recipients = recipients;
            this.attributes = new List<RichTextFormat>();
            this.HtmlMessage = html;
            this.PlainMessage = plain;
        }
 
        public uint Recipient { get; set; }
 
        public ClassMessage Class { get; set; }
 
        public string HtmlMessage { get; set; }
 
        public string PlainMessage { get; set; }
 
        public List<RichTextFormat> Attributes
        {
            get { return this.attributes; }
        }
 
        public override void Write()
        {
            byte[] msgHtml = UTF8Encoding.UTF8.GetBytes(this.HtmlMessage);
            byte[] msgPlain = Encoding.GetEncoding(1250).GetBytes(this.PlainMessage);
            int offsetConference = 0;
            if (this.recipients != null && this.recipients.Count() != 0)
            {
                offsetConference = 8 + 4 * this.recipients.Count();
            }
 
            base.Write();
            writer.Write(this.Recipient);
            writer.Write(this.sequence);
            writer.Write((uint)this.Class);
 
            writer.Write(msgHtml.Length + 20 + 1);
            writer.Write(msgHtml.Length + msgPlain.Length + 20 + 2 + offsetConference);
 
            writer.Write(msgHtml);
            writer.Write(' ');
            writer.Write(msgPlain);
            writer.Write(' ');
 
            if (offsetConference != 0)
            {
                this.writer.Write((byte)0x01);
                this.writer.Write((uint)this.recipients.Count());
                foreach (uint number in this.recipients)
                {
                    this.writer.Write(number);
                }
            }
 
            foreach (RichTextFormat attr in this.Attributes)
            {
                writer.Write(attr.Position);
                writer.Write((byte)attr.FormatType);
                if (attr.Color != null)
                {
                    writer.Write(attr.Color.R);
                    writer.Write(attr.Color.G);
                    writer.Write(attr.Color.B);
                }
            }
        }
 
        private uint[] recipients;
        private List<RichTextFormat> attributes;
        private uint sequence;
    }
}
