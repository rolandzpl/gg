using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
namespace MTGG.Packets
{
    internal class WelcomePacket : Packet
    {
        public uint Seed { get; private set; }
 
        public override void Read()
        {
            base.Read();
            this.Seed = this.reader.ReadUInt32();
        }
    }
}
