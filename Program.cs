using System;
using MTGG;

namespace GG
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new GGClient(70143135, "", Channel.Mobile);
            client.Connected += OnConnected;
            client.Disconnected += OnDisconnected;
            client.LoggedFail += OnLoggedFail;
            client.LogOn(GGStatus.Busy);
            Console.ReadLine();
            client.LogOut("Bye!");
        }

        private static void OnLoggedFail(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Login failed");
        }

        private static void OnDisconnected(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Disconnected");
        }

        private static void OnConnected(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Connected");
        }
    }
}
