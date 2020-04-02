using System.Collections.Generic;
using System.Text;
using System.Net;

namespace MTGG.Packets
{
    internal class NotifyReplyPacket : Packet
    {
        public NotifyReplyPacket()
        {
            this.clients = new List<ClientState>();
        }
 
        public ClientState[] ClientStates
        {
            get { return this.clients.ToArray(); }
        }
 
        public override void Read()
        {
            base.Read();
            while (this.reader.BaseStream.Position != this.reader.BaseStream.Length)
            {
                ClientState reply = new ClientState();
                reply.Number = reader.ReadUInt32();
                reply.Status = (GGStatus)reader.ReadUInt32();
                reply.Features = (Features)reader.ReadUInt32();
                reply.RemoteIP = new IPAddress(reader.ReadUInt32());
                reply.RemotePort = reader.ReadUInt16();
                reply.ImageSize = reader.ReadByte();
                reply.Unknown = reader.ReadByte();
                reply.Flags = (Channel)reader.ReadUInt32();
                uint len = reader.ReadUInt32();
                reply.Description = UTF8Encoding.UTF8.GetString(reader.ReadBytes((int)len));
                this.clients.Add(reply);
            }
        }
        private List<ClientState> clients;
    }
}
