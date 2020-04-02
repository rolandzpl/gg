using System;
using System.Linq;
using System.Net.Sockets;

namespace MTGG.TCP
{
    internal class TCPConnector
    {
        public TCPConnector() { }
 
        public void Open(string host, int port)
        {
            if (!this.startConnect)
            {
                this.tcp = new TcpClient();
                this.startConnect = true;
                this.tcp.BeginConnect(host, port, ConnectCallback, null);
            }
        }
 
        public void Close()
        {
            this.Disconnected(this, null);
            this.startConnect = false;
            this.stream.Close();
            this.tcp.Close();
        }
 
        public void Write(byte[] bytes)
        {
            this.stream.BeginWrite(bytes, 0, bytes.Length, WriteCallback, null);
        }
 
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event ExceptionEventHandler Error;
        public event DataEventHandler DataReceived;
 
        public bool IsOpen
        {
            get { return this.tcp != null && this.tcp.Connected; }
        }
 
        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                this.tcp.EndConnect(result);
            }
            catch (Exception ex)
            {
                this.Error(this, new ExceptionEventArgs(ex));
                return;
            }
 
            this.stream = this.tcp.GetStream();
            this.Connected(this, null);
            byte[] buffer = new byte[this.tcp.ReceiveBufferSize];
            this.stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
        }
 
        private void ReadCallback(IAsyncResult result)
        {
            int read;
            try
            {
                read = this.stream.EndRead(result);
            }
            catch (Exception ex)
            {
                this.Error(this, new ExceptionEventArgs(ex));
                this.Close();
                return;
            }
 
            if (read == 0)
            {
                return;
            }
 
            byte[] buffer = result.AsyncState as byte[];
            this.DataReceived(this, new DataEventArgs(buffer.Take(read).ToArray()));
            Array.Clear(buffer, 0, buffer.Length);
            this.stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
        }
 
        private void WriteCallback(IAsyncResult result)
        {
            this.stream.EndWrite(result);
        }
 
        private NetworkStream stream;
        private TcpClient tcp;
        private bool startConnect;
    }
}
