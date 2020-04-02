using System;
using MTGG;

namespace GG
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("password:");
            var passwd = Console.ReadLine();
            var client = new GGClient(70143135, passwd, Channel.Mobile);
            client.Connected += OnConnected;
            client.Disconnected += OnDisconnected;
            client.LoggedFail += OnLoggedFail;
            client.ContactStateChanged += OnContactStateChanged;
            client.ContactInfoReceived += OnContactInfoReceived;
            client.AddContact(9876543, ContactType.Buddy);
            client.AddContact(8765432, ContactType.Buddy);
            client.AddContact(7654321, ContactType.Buddy);
            client.LogOn(GGStatus.Busy);
            Console.ReadLine();
            client.LogOut("Bye!");
        }

        private static void OnContactInfoReceived(object sender, ContactEventArgs e)
        {
            Console.WriteLine($"Contact info: {e.Contact.Number}");
        }

        private static void OnContactStateChanged(object sender, ContactEventArgs e)
        {
            Console.WriteLine($"{e.Contact.Number} changed state to {e.Contact.State}");
        }

        private static void OnLoggedFail(object sender, EventArgs e)
        {
            Console.WriteLine("Login failed");
        }

        private static void OnDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected");
        }

        private static void OnConnected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected");
        }
    }
}
