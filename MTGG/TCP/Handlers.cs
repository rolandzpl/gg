using System;

namespace MTGG.TCP
{
    internal class DataEventArgs : EventArgs
    {
        public DataEventArgs(byte[] data)
        {
            this.Data = data;
            this.Entry = DateTime.Now;
        }
 
        public DateTime Entry { get; private set; }
 
        public byte[] Data { get; set; }
    }
 
    internal class ExceptionEventArgs: EventArgs
    {
        public ExceptionEventArgs(Exception exception)
        {
            this.Exception = exception;
        }
 
        public Exception Exception { get; set; }
    }
    internal delegate void DataEventHandler(object sender, DataEventArgs e);
    internal delegate void ExceptionEventHandler(object sender, ExceptionEventArgs e);
}

