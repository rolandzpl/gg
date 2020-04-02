using System;
using System.Collections.Generic;
using MTGG.Packets;

namespace MTGG
{
    public class GGContact
    {
        public GGContact(uint number, ContactType type)
        {
            this.Number = number;
            this.ContactType = type;
        }
 
        public uint Number { get; set; }
 
        public ContactType ContactType { get; set; }
 
        public ClientState State { get; internal set; }
 
        public List<UserDataAttributes> ExtendedInfo { get; internal set; }
    }
 
    [Flags]
    public enum ContactType : byte
    {
        Buddy = 0x01,
        Friend = 0x02,
        Blocked = 0x04
    }
 
    public class ContactEventArgs : EventArgs
    {
        public ContactEventArgs(GGContact contact)
        {
            this.Contact = contact;
        }
 
        public GGContact Contact { get; private set; }
    }
 
    public delegate void ContactEventHandler(object sender, ContactEventArgs e);
}
