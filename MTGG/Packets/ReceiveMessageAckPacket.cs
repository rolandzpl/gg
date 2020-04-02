namespace MTGG.Packets
{
    internal class ReceiveMessageAckPacket : Packet
    {
        public ReceiveMessageAckPacket(uint sequence)
        {
            this.PacketType = PacketType.RecvMessageAck;
            this.Sequence = sequence;
        }
 
        public uint Sequence { get; set; }
 
        public override void Write()
        {
            base.Write();
            this.writer.Write(this.Sequence);
        }
    }
}
