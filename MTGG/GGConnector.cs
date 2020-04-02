using System;
using MTGG.TCP;
using MTGG.Packets;
using System.Net;
using System.IO;

namespace MTGG
{
    internal class GGConnector
    {
        public GGConnector(uint number)
        {
            this.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; pl; rv:1.9.0.4) Gecko/2008102920 Firefox/3.0.4 (.NET CLR 3.5.30729)";
            this.Connector = new TCPConnector();
            this.number = number;
        }
 
        public TCPConnector Connector { get; set; }
        public string UserAgent { get; set; }
 
        public void Open()
        {
            if (!this.Connector.IsOpen && this.GetConnectionParameters())
            {
                this.Connector.Open(this.host.ToString(), this.port);
            }
        }
 
        public void Close()
        {
            this.Connector.Close();
        }
 
        public void WritePacket(Packet packet)
        {
            this.Connector.Write(packet.RawData);
        }
 
        private bool GetConnectionParameters()
        {
            string url = String.Format(ggUrl, this.number);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.UserAgent = this.UserAgent;
 
            using (StreamReader stream = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                string response = stream.ReadToEnd().Trim();
                string[] lines = response.Split(' ');
                string ip = lines[3];
                string port = lines[2].Split(':')[1];
 
                return IPAddress.TryParse(ip, out this.host) && Int32.TryParse(port, out this.port);
            }
        }
 
        private const string ggUrl = "http://appmsg.gadu-gadu.pl/appsvc/appmsg_ver8.asp?fmnumber={0}&amp;version=8.0.0.7669";
        private IPAddress host;
        private int port;
        private uint number;
    }
}
