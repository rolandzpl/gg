using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MTGG.TCP;
using MTGG.Packets;
using System.Diagnostics;

namespace MTGG
{
    internal class PacketManager
    {
        public PacketManager(GGConnector connector)
        {
            this.connector = connector;
            this.connector.Connector.DataReceived += new DataEventHandler(this.connector_DataReceived);

            this.outputs = new Queue<Packet>();
            this.inputs = new Queue<Packet>();
            this.bufferData = new List<byte>();

            this.packets = new Dictionary<PacketType, Type>();
            this.RegisterPackets();

            this.connector.Connector.Connected += new EventHandler(Connector_Connected);
            this.connector.Connector.Disconnected += new EventHandler(Connector_Disconnected);
        }

        public event PacketEventHandler PacketReceived;

        public void Start()
        {
            this.threadProcess = new Thread(new ThreadStart(this.DispatchPackets));
            this.threadSend = new Thread(new ThreadStart(this.SendPackets));
            this.active = true;

            this.threadProcess.Start();
            this.threadSend.Start();
        }

        public void Stop()
        {
            this.active = false;
            this.threadProcess.Join();
            this.threadSend.Join();
        }

        public void AddPacket(Packet packet)
        {
            packet.Write();
            packet.Finish();
            lock (this.outputs)
            {
                this.outputs.Enqueue(packet);
            }
        }

        private void DispatchPackets()
        {
            while (this.active)
            {
                Packet packet = null;
                lock (this.inputs)
                {
                    if (this.inputs.Count != 0)
                    {
                        packet = this.inputs.Dequeue();
                    }
                }

                if (packet != null)
                {
                    this.PacketReceived(this, new PacketEventArgs(packet));


                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        private void SendPackets()
        {
            while (this.active)
            {
                Packet packet = null;
                lock (this.outputs)
                {
                    if (this.outputs.Count != 0)
                    {
                        packet = this.outputs.Dequeue();
                    }
                }

                if (packet != null)
                {
                    this.connector.WritePacket(packet);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        private Packet CreatePacket(byte[] data)
        {
            Packet packet = null;
            uint messageCode = BitConverter.ToUInt32(data, 0);

            Debug.Assert(Enum.IsDefined(typeof(PacketType), messageCode), String.Format("Unhandled Enum PacketType: {0}", messageCode));
            Debug.Assert(this.packets.ContainsKey((PacketType)messageCode), String.Format("Not implemented packet: {0}", messageCode));

            Type type = this.packets[(PacketType)messageCode];
            packet = Activator.CreateInstance(type) as Packet;
            packet.RawData = data;
            packet.Read();

            return packet;
        }

        private void ProcessData()
        {
            byte[] data = null;
            lock (this.bufferData)
            {
                if (this.bufferData.Count >= 8)
                {
                    int length = BitConverter.ToInt32(bufferData.ToArray(), 4) + 8;
                    if (this.bufferData.Count >= length)
                    {
                        data = this.bufferData.Take(length).ToArray();
                        this.bufferData.RemoveRange(0, length);
                    }
                }
            }

            if (data != null)
            {
                Packet packet = this.CreatePacket(data);
                lock (this.inputs)
                {
                    this.inputs.Enqueue(packet);
                }
            }
        }

        private void RegisterPackets()
        {
            this.packets.Add(PacketType.Welcome, typeof(WelcomePacket));
            this.packets.Add(PacketType.Login80, typeof(LoginPacket));
            this.packets.Add(PacketType.Login80_OK, typeof(Packet));
            this.packets.Add(PacketType.Login80_Fail, typeof(Packet));
            this.packets.Add(PacketType.Login_Fail, typeof(Packet));
            this.packets.Add(PacketType.Status, typeof(NotifyReplyPacket));
            this.packets.Add(PacketType.NotifyReply80, typeof(NotifyReplyPacket));
            this.packets.Add(PacketType.UserData, typeof(UserDataPacket));
        }

        private void Connector_Disconnected(object sender, EventArgs e)
        {
            this.Stop();
        }

        private void Connector_Connected(object sender, EventArgs e)
        {
            this.Start();
        }

        private void connector_DataReceived(object sender, DataEventArgs e)
        {
            lock (this.bufferData)
            {
                this.bufferData.AddRange(e.Data);
            }
            this.ProcessData();
        }

        private bool active;
        private GGConnector connector;
        private Thread threadProcess;
        private Thread threadSend;
        private Dictionary<PacketType, Type> packets;
        private Queue<Packet> outputs;
        private Queue<Packet> inputs;
        private List<byte> bufferData;
    }
}
