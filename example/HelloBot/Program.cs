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

        /// <summary>
        /// 私聊消息
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="targetID"></param>
        private static void ChatMsg(string chat, string targetID)
        {
            Console.WriteLine("Bot:" + chat);
            if (chat == "你好")
            {
                bot.SendMessage.Chat(targetID, "我是机器人。你好");
            }
        }

        /// <summary>
        /// 频道消息
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="targetID"></param>
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