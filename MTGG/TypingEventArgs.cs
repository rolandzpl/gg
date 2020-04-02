using System;

namespace MTGG
{
    public class TypingEventArgs : EventArgs
    {
        public TypingEventArgs(ushort value, GGContact contact)
        {
            this.Value = value;
            this.Contact = contact;
        }
 
        public ushort Value { get; internal set; }
 
        public GGContact Contact { get; internal set; }
    }
 
    public delegate void TypingEventHandler(object sender, TypingEventArgs e);
}
