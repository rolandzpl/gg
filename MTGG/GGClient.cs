using System;
using System.Timers;
using MTGG.Packets;

namespace MTGG
{
    public enum State
    {
        Connected,
        Disconnected
    }
 
    public class GGClient
    {
        public GGClient(uint number, string password) : this(number, password, Channel.None) { }
 
        public GGClient(uint number, string password, Channel options)
        {
            this.number = number;
            this.password = password;
            this.channels = options;
            this.State = State.Disconnected;
 
            this.connector = new GGConnector(number);
            this.packetManager = new PacketManager(this.connector);
            this.packetManager.PacketReceived += new PacketEventHandler(packetManager_PacketReceived);
 
            this.timerPing = new Timer(PingInterval);
            this.timerPing.Elapsed += new ElapsedEventHandler(this.timerPing_Elapsed);
        }
 
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler LoggedFail;
 
        public State State { get; private set; }
 
        public void LogOn(GGStatus status)
        {
            this.loginStatus = status;
            this.connector.Open();
        }
 
        public void LogOut(string description)
        {
            GGStatus status = String.IsNullOrEmpty(description) ? GGStatus.NotAvailable :
            GGStatus.NotAvailableDescription;
            StatusPacket packet = new StatusPacket(status, description);
            this.packetManager.AddPacket(packet);
            this.State = State.Disconnected;
            this.packetManager.Stop();
            this.connector.Close();
        }
 
        public void ChangeStatus(GGStatus status, string description)
        {
            StatusPacket packet = new StatusPacket(status, description);
            this.packetManager.AddPacket(packet);
        }
 
        private void packetManager_PacketReceived(object sender, PacketEventArgs e)
        {
            switch (e.Packet.PacketType)
            {
                case PacketType.Welcome:
                    WelcomePacket welcome = e.Packet as WelcomePacket;
                    LoginPacket packet = new LoginPacket(this.number, this.password, welcome.Seed,
                        this.loginStatus, this.channels);
                    this.packetManager.AddPacket(packet);
                    break;
 
                case PacketType.Login80_OK:
                    this.State = State.Connected;
                    this.timerPing.Start();
                    if (this.Connected != null)
                    {
                        this.Connected(this, EventArgs.Empty);
                    }
                    break;
 
                case PacketType.Login80_Fail:
                    if (this.LoggedFail != null)
                    {
                        this.LoggedFail(this, EventArgs.Empty);
                    }
                    break;
 
                case PacketType.Disconnect:
                    this.State = State.Disconnected;
                    this.timerPing.Stop();
                    if (this.Disconnected != null)
                    {
                        this.Disconnected(this, EventArgs.Empty);
                    }
                    break;
            }
        }
 
        private void timerPing_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.State == State.Connected)
            {
                this.packetManager.AddPacket(new Packet(PacketType.Ping));
            }
        }
 
        private GGStatus loginStatus;
        private Channel channels;
        private const double PingInterval = 240000;
        private PacketManager packetManager;
        private Timer timerPing;
        private GGConnector connector;
        private uint number;
        private string password;
    }
}
