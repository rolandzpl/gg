using System;
using MTGG.Packets;

namespace MTGG
{
    internal class PacketEventArgs : EventArgs
    {
        public PacketEventArgs(Packet packet)
        {
            this.Packet = packet;
        }

        public Packet Packet
        {
            get;
            set;
        }
    }
    internal delegate void PacketEventHandler(object sender, PacketEventArgs e);
}