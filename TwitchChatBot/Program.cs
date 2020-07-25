using System;
using System.IO;

namespace TwitchChatBot
{
    class Program
    {
        static void Main(string[] args)
        {
            TwitchIRCClient client = new TwitchIRCClient();
            client.Connect();
            client.Chat();
        }
    }
}
