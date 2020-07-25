using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace TwitchChatBot
{
    public class TwitchInit
    {
        public const string Host = "irc.twitch.tv";
        public const int port = 6667;
        //Insert here NickName using camel case of your bot
        public const string BotNick = "BotNickName";
        //Insert here channel you wanna connect using lower case 
        public const string ChannelName = "channelname";
    }

    public class TwitchIRCClient
    {
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;
        private string passToken = string.Empty;
        private Dictionary<string, string> answers = new Dictionary<string, string>
        {
            { "!xyu", "PogChamp" },
        };
        private Dictionary<string, BullsAndCows> bullsAndCows = new Dictionary<string, BullsAndCows>();

        public TwitchIRCClient()
        {
            client = new TcpClient(TwitchInit.Host, TwitchInit.port);
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;
            //inser here your oauth:... token or read it from file
            passToken = File.ReadAllText("auth.ps");
        }

        public void Connect()
        {
            SendCommand("PASS", passToken);
            SendCommand("USER", string.Format("{0} 0 * {0}", TwitchInit.BotNick));
            SendCommand("NICK", TwitchInit.BotNick);
            SendCommand("JOIN", "#" + TwitchInit.ChannelName);
        }

        public void CheckCommand(string msg)
        {
            string reply = null;
            foreach (var pair in answers)
            {
                if (msg.Contains(pair.Key))
                {
                    reply = pair.Value;
                    break;
                }
            }
            if (reply != null)
            {
                SendMessage(reply);
            }
        }

        public void Chat()
        {
            while (true)
            {
                string message = reader.ReadLine();
                if (message != null)
                {
                    System.Console.Out.WriteLine(message);
                    CheckCommand(message);
                    if (message == "PING :tmi.twitch.tv")
                    {
                        SendCommand("PONG", ":tmi.twitch.tv");
                    }
                    else if (message.Contains("!cows"))
                    {
                        HandleCows(message);
                    }
                }
            }
        }

        private void HandleCows(string message)
        {
            string replyMessage = string.Empty;
            string userName = message.Split(new char[] { ':', '!' }, StringSplitOptions.RemoveEmptyEntries)[0];
            string chatMessage = message.Split("#", StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(":", StringSplitOptions.RemoveEmptyEntries)[1];
            Regex gueessRegex = new Regex(@"!cowsguess(\s+)([1-9]{1}[0-9]{3})");
            MatchCollection matches = gueessRegex.Matches(chatMessage);
            //"!cowsgues 1234"
            if (matches.Count > 0)
            {
                string guessCommand = matches[0].Value;
                replyMessage = guessCommand == chatMessage
                    ? HandleCowsGuess(userName, guessCommand)
                    : string.Format("Пользователь: {0} неверный формат команды", userName);
            }
            else if (!chatMessage.Contains("!cowsguess"))
            {
                if (bullsAndCows.ContainsKey(userName))
                {
                    bullsAndCows.Remove(userName);
                }
                bullsAndCows.Add(userName, new BullsAndCows(userName));
                replyMessage = string.Format("Пользователь: {0} начинает игру Быки и Коровы " +
                    "используйте команду !cowsguess (number), дабы сделать попытку", userName);
            }
            SendMessage(replyMessage);
        }

        private bool CheckDigits(string number)
        {
            for (int i = 0; i < number.Length; i++)
            {
                for (int j = i + 1; j < number.Length; j++)
                {
                    if (number[i] == number[j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private string HandleCowsGuess(string userName, string command)
        {
            if (!bullsAndCows.ContainsKey(userName))
            {
                return string.Format("Пользователь: {0} еще не начал игру", userName);
            }
            Regex numberRegex = new Regex(@"([1-9]{1}[0-9]{3})");
            MatchCollection matches = numberRegex.Matches(command);
            if (matches.Count > 0)
            {
                string number = matches[0].Value;
                string replyMessage = string.Empty;
                if (CheckDigits(number))
                {
                    replyMessage = bullsAndCows[userName].Guess(number);
                    if (bullsAndCows[userName].IsWin)
                    {
                        bullsAndCows.Remove(userName);
                    }
                }
                else
                {
                    replyMessage = string.Format("Пользователь: {0} ввёл неверный формат числа", userName);
                }
                
                return replyMessage;
            }
            return string.Format("Пользователь: {0} ввёл неверный формат числа", userName);
        }

        private void SendMessage(string message)
        {
            SendCommand("PRIVMSG", string.Format("#{0} :{1}", TwitchInit.ChannelName, message));
        }

        private void SendCommand(string cmd, string param)
        {
            writer.WriteLine(cmd + " " + param);
        }

    }
}
