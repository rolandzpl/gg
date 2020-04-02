using MTGG.Packets;
using System;
using System.Linq;

namespace MTGG
{
    public class GGMessage
    {
        public GGMessage(GGContact recipient) : this(new GGContact[] { recipient }) { }
        public GGMessage(uint recipient) : this(new uint[] { recipient }) { }
 
        public GGMessage(GGContact[] recipients) :
            this(recipients.Select(x => x.Number).ToArray()) { }
 
        public GGMessage(uint[] recipients)
        {
            this.Recipients = recipients;
            this.Time = DateTime.Now;
            this.Sequence = UTC.DateToUnixTimestamp(this.Time);
            this.PlainMessage = String.Empty;
            this.HtmlMessage = String.Empty;
        }
 
        public uint Sender { get; internal set; }
 
        public uint[] Recipients { get; private set; }
 
        public DateTime Time { get; internal set; }
 
        public uint Sequence { get; internal set; }
 
        public string HtmlMessage { get; set; }
 
        public string PlainMessage { get; set; }
    }
 
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(MessageStatus status, GGMessage message)
        {
            this.Status = status;
            this.Message = message;
        }
 
        public MessageStatus Status { get; internal set; }
 
        public GGMessage Message { get; internal set; }
    }
    public delegate void MessageEventHandler(object sender, MessageEventArgs e);
}
