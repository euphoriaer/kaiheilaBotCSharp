using CsharpBot;
using System;

namespace HelloBot
{
    internal class Program
    {
        public static Bot bot;

        private static void Main(string[] args)
        {
            bot = new Bot("1/MTA1NTg=/LZ2fsaN2Te7hM7mh8bflnA==");
            bot.ChannelMsg += ChannelReceive;
            bot.ChatlMsg += ChatMsg;
            bot.Run();
        }

        private static void ChatMsg(string chat, string targetID)
        {
           //登录
        }

        private static void ChannelReceive(string chat, string targetID)
        {
            Console.WriteLine("Bot:" + chat);
            if (chat == "你好")
            {
                bot.SendMessage.Channel(targetID, "我是机器人。你好");
            }
        }
    }
}