using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTGG.Packets
{
    internal class SendMessageAckPacket : Packet
    {
        public MessageStatus Status { get; private set; }

        public uint Recipient { get; private set; }

        public uint Sequence { get; private set; }

        public override void Read()
        {
            base.Read();
            this.Status = (MessageStatus)this.reader.ReadUInt32();
            this.Recipient = this.reader.ReadUInt32();
            this.Sequence = this.reader.ReadUInt32();
        }
    }

    public enum MessageStatus : uint
    {
        Blocked = 0x01,
        Delivered = 0x02,
        Queued = 0x03,
        MBoxFull = 0x04,
        NotDelivered = 0x06
    }
}