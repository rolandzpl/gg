using System;
using System.Text;
using System.Net;
using MTGG.Security;

namespace MTGG.Packets
{
    internal class LoginPacket : Packet
    {
        public LoginPacket(uint number, string password, uint seed, GGStatus status, Channel channel)
        {
            this.PacketType = PacketType.Login80;
            this.Number = number;
            this.Password = password;
            this.Seed = seed;
            this.Language = "pl";
            this.HashType = HashType.SHA1;
            this.Status = status;
            this.Channels = channel;
            this.Features =
                Features.ReceiveMessage |
                Features.ChangeStatus |
                Features.ChangeStatus2 |
                Features.ExtInfoContact |
                Features.NotifyTalk |
                Features.ConfirmReceive |
                Features.MultiLogon |
                Features.Unknown;
            this.LocalIP = IPAddress.None;
            this.LocalPort = 0;
            this.ExternalIP = IPAddress.None;
            this.ExternalPort = 0;
            this.ImageSize = Byte.MaxValue;
            this.Unknown = 0x64;
            this.Client = "Gadu-Gadu Client build 8.0.0.7669";
            this.Description = String.Empty;
        }
 
        public uint Number { get; set; }
 
        public string Password { get; set; }
 
        public uint Seed { get; set; }
 
        public string Language { get; set; }
 
        public HashType HashType { get; set; }
 
        public GGStatus Status { get; set; }
 
        public Channel Channels { get; set; }
 
        public Features Features { get; set; }
 
        public IPAddress LocalIP { get; set; }
 
        public short LocalPort { get; set; }
 
        public IPAddress ExternalIP { get; set; }
 
        public short ExternalPort { get; set; }
 
        public byte ImageSize { get; set; }
 
        public byte Unknown { get; private set; }
 
        public string Client { get; set; }
 
        public string Description { get; set; }
 
        public override void Write()
        {
            base.Write();
 
            byte[] hash;
            if (this.HashType == HashType.GG32)
            {
                hash = GGHash.GG32(this.Password, this.Seed);
            }
            else
            {
                hash = GGHash.SHA1(this.Password, this.Seed);
            }
 
            byte[] description = UTF8Encoding.UTF8.GetBytes(this.Description);
 
            this.writer.Write(this.Number);
            this.writer.Write(UTF8Encoding.UTF8.GetBytes(this.Language));
            this.writer.Write((byte)this.HashType);
            this.writer.Write(hash);
 
            for (int i = 0; i < 64 - hash.Length; ++i)
            {
                this.writer.Write(Byte.MinValue);
            }
 
            this.writer.Write((uint)this.Status);
            this.writer.Write((uint)this.Channels);
            this.writer.Write((uint)this.Features);
            this.writer.Write(BitConverter.ToUInt32(this.LocalIP.GetAddressBytes(), 0));
            this.writer.Write(this.LocalPort);
            this.writer.Write(BitConverter.ToUInt32(this.ExternalIP.GetAddressBytes(), 0));
            this.writer.Write(this.ExternalPort);
            this.writer.Write(this.ImageSize);
            this.writer.Write(this.Unknown);
            this.writer.Write(this.Client.Length);
            this.writer.Write(UTF8Encoding.UTF8.GetBytes(this.Client));
            this.writer.Write(description.Length);
            this.writer.Write(description);
        }
    }
}
