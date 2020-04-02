using System;
using System.Collections.Generic;
using System.Linq;
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
        public event ContactEventHandler ContactStateChanged;
        public event ContactEventHandler ContactInfoReceived;

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

                // case PacketType.Login80_OK:
                //     this.State = State.Connected;
                //     this.timerPing.Start();
                //     if (this.Connected != null)
                //     {
                //         this.Connected(this, EventArgs.Empty);
                //     }
                //     break;

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

                case PacketType.Login80_OK:
                    this.State = State.Connected;
                    this.SendContactList();
                    this.timerPing.Start();
                    if (this.Connected != null)
                    {
                        this.Connected(this, EventArgs.Empty);
                    }
                    break;

                case PacketType.UserData:
                    UserDataPacket userData = e.Packet as UserDataPacket;
                    foreach (UserData data in userData.Attributes)
                    {
                        if (this.contacts.ContainsKey(data.Number))
                        {
                            this.contacts[data.Number].ExtendedInfo = data.Attributes;
                            if (this.ContactInfoReceived != null)
                            {
                                this.ContactInfoReceived(this, new ContactEventArgs(this.contacts[data.Number]));
                            }
                        }
                    }
                    break;

                case PacketType.NotifyReply80:
                case PacketType.Status:
                    NotifyReplyPacket notify = e.Packet as NotifyReplyPacket;
                    foreach (ClientState state in notify.ClientStates)
                    {
                        this.contacts[state.Number].State = state;
                        if (this.ContactStateChanged != null)
                        {
                            this.ContactStateChanged(this, new ContactEventArgs(this.contacts[state.Number]));
                        }
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

        private Dictionary<uint, GGContact> contacts;

        public GGContact[] Contacts
        {
            get { return this.contacts.Values.ToArray(); }
        }

        public void AddContact(uint number, ContactType type)
        {
            this.AddContact(new GGContact(number, type));
        }

        public void AddContact(GGContact contact)
        {
            this.contacts.Add(contact.Number, contact);
            if (this.State == State.Connected)
            {
                NotifyPacket notify = new NotifyPacket(PacketType.NotifyAdd);
                notify.Contacts.Add(contact);
                this.packetManager.AddPacket(notify);
            }
        }

        public void RemoveContact(uint number)
        {
            GGContact contact = this.contacts[number];
            this.RemoveContact(contact);
        }

        public void RemoveContact(GGContact contact)
        {
            if (this.State == State.Connected)
            {
                NotifyPacket notify = new NotifyPacket(PacketType.NotifyRemove);
                notify.Contacts.Add(contact);
                this.packetManager.AddPacket(notify);
            }
            this.contacts.Remove(contact.Number);
        }

        public void BlockContact(uint number)
        {
            GGContact contact = this.contacts[number];
        }

        public void BlockContact(GGContact contact)
        {
            NotifyPacket notifyRemove = new NotifyPacket(PacketType.NotifyRemove);
            notifyRemove.Contacts.Add(contact);
            this.packetManager.AddPacket(notifyRemove);

            contact.ContactType = ContactType.Blocked;
            NotifyPacket notifyAdd = new NotifyPacket(PacketType.NotifyAdd);
            notifyRemove.Contacts.Add(contact);
            this.packetManager.AddPacket(notifyAdd);
        }

        private void SendContactList()
        {
            if (this.contacts.Count == 0)
            {
                this.packetManager.AddPacket(new Packet(PacketType.EmptyContactList));
            }
            else
            {
                int countPackets = (int)Math.Ceiling(this.contacts.Count / 400.0);
                for (int i = 0; i < countPackets; ++i)
                {
                    PacketType type = PacketType.NotifyFirst;
                    if (i == (countPackets - 1))
                    {
                        type = PacketType.NotifyLast;
                    }
                    NotifyPacket packet = new NotifyPacket(type);
                    packet.Contacts.AddRange(this.contacts.Values.Skip(i * 400).Take(400));
                    this.packetManager.AddPacket(packet);
                }
            }
        }
    }
}
