using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
 
namespace MTGG
{
    public class ClientState
    {
        public uint Number { get; set; }
 
        public GGStatus Status { get; set; }
 
        public Features Features { get; set; }
 
        public IPAddress RemoteIP { get; set; }
 
        public ushort RemotePort { get; set; }
 
        public byte ImageSize { get; set; }
 
        public byte Unknown { get; set; }
 
        public Channel Flags { get; set; }
 
        public string Description { get; set; }
    }
}
