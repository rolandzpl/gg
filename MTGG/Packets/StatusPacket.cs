using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
namespace MTGG.Packets
{
    internal class StatusPacket : Packet
    {
        public StatusPacket(GGStatus status) : this(status, String.Empty) { }
 
        public StatusPacket(GGStatus status, string description)
        {
            this.Status = status;
            this.Description = description;
            this.Flags = Channel.None;
            this.PacketType = PacketType.ChangeStatus;
        }
 
        public GGStatus Status { get; set; }
 
        public Channel Flags { get; set; }
 
        public string Description { get; set; }
 
        public override void Write()
        {
            base.Write();
            byte[] description = UTF8Encoding.UTF8.GetBytes(this.Description);
            writer.Write((uint)this.Status);
            writer.Write((uint)this.Flags);
            writer.Write(description.Length);
            writer.Write(description);
        }
    }
}
