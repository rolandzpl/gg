using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
namespace MTGG.Packets
{
    internal class TypingNotifyPacket : Packet
    {
        public TypingNotifyPacket()
        {
            this.PacketType = PacketType.TypingNotify;
        }
 
        public TypingNotifyPacket(uint number, ushort length) : this()
        {
            this.Value = length;
            this.Number = number;
        }
 
        public ushort Value { get; set; }
 
        public uint Number { get; set; }
 
        public override void Read()
        {
            base.Read();
            this.Value = this.reader.ReadUInt16();
            this.Number = this.reader.ReadUInt32();
        }
 
        public override void Write()
        {
            base.Write();
            this.writer.Write(this.Value);
            this.writer.Write(this.Number);
        }
    }
}
