using System.Collections.Generic;

namespace MTGG.Packets
{
    internal class NotifyPacket : Packet
    {
        public NotifyPacket(PacketType type)
        {
            this.Contacts = new List<GGContact>();
            this.PacketType = type;
        }
 
        public List<GGContact> Contacts { get; private set; }
 
        public override void Write()
        {
            base.Write();
            foreach (GGContact contact in this.Contacts)
            {
                this.writer.Write(contact.Number);
                this.writer.Write((byte)contact.ContactType);
            }
        }
    }
}
