using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CsharpBot;

namespace HelloBot
{
    internal class Program
    {
        public static Bot bot;

        private static void Main(string[] args)
        {
            bot = new Bot("1/MTA1NTg=/LZ2fsaN2Te7hM7mh8bflnA==");
            bot.ChatMsg += Receive;
            bot.Run();
        }

        private static void Receive(string chat, string targetID)
        {
            Console.WriteLine("Bot:" + chat);
            if (chat == "你好")
            {
                bot.SendMessage(targetID, "我是机器人。你好");
            }
        }
    }
}