using System;
using System.Text;
using System.IO;

namespace MTGG.Packets
{
    internal class Packet
    {
        public Packet()
        {
            this.stream = new MemoryStream();
            this.reader = new BinaryReader(this.stream, ASCIIEncoding.ASCII);
            this.writer = new BinaryWriter(this.stream, ASCIIEncoding.ASCII);
        }
 
        public Packet(PacketType type) : this()
        {
            this.PacketType = type;
        }
 
        public void Finish()
        {
            long len = this.stream.Length - 8;
            this.writer.Seek(4, SeekOrigin.Begin);
            this.writer.Write((uint)len);
        }
 
        public virtual void Write()
        {
            this.stream.SetLength(0);
            this.writer.Write((uint)this.PacketType);
            this.writer.Write((uint)0);
        }
 
        public virtual void Read()
        {
            this.stream.Seek(0, SeekOrigin.Begin);
            this.PacketType = (PacketType)reader.ReadUInt32();
            reader.ReadUInt32();
        }
 
        public override string ToString()
        {
            return String.Format("Packet {0}  {1}", this.PacketType.ToString(), PacketType.ToString("X2"));
        }
 
        public PacketType PacketType
        {
            get;
            protected set;
        }
 
        public byte[] RawData
        {
            get { return this.stream.ToArray(); }
            set { this.stream.Write(value, 0, value.Length); }
        }
        private MemoryStream stream;
        protected BinaryWriter writer;
        protected BinaryReader reader;
    }
}
