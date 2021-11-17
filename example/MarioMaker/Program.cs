using CsharpBot;
using System;
using System.IO;

namespace MarioMaker
{
    internal partial class Program
    {
        private static Bot Bot;
        private static DistributeUtil<Action<string[], string>, MarioAttr, Program> distribute;

        public static Config cfg;

        private const string ChatHelp =
            "马造机器人命令" + "\n" + "\n" +
            "注册用户：.注册 user/password" + "\n" + "\n" +
            "登录用户：.登录 user/password" + "\n";

        private const string ChannelHelp =
            "马造机器人命令" + "\n" + "\n" +
            "添加关卡：.添加 关卡ID 名字 类型" + "\n" + "\n" +
            "修改难度：.sd 关卡id 难度(0.5-10)" + "\n";

        private static void Main(string[] args)
        {
            Console.WriteLine("在开黑啦私聊机器人 .help 查看命令");

            distribute = new DistributeUtil<Action<string[], string>, MarioAttr, Program>(null);
            var configPath = Path.Combine(System.Environment.CurrentDirectory, "Config.Json");
            cfg = new Config(configPath);
            Bot = new CsharpBot.Bot(cfg.Read("BotToken"));
            Bot.ChatlMsg += ChatMsg;
            Bot.ChannelMsg += ChannelMsg;
            Bot.Run();
        }

        private static void ChannelMsg(string msg, string id)
        {
            Console.WriteLine(msg);
            string[] msgs = msg.Split(" ");
            try
            {
                string disName = msgs[0] + ".ChannelMsg";
                var msgMethod = distribute.GetMethod(disName);
                msgMethod(msgs, id);
            }
            catch (Exception e)
            {
                Bot.SendMessage.Chat(id, "回调错误" + e);
            }
        }

        private static void ChatMsg(string msg, string id)
        {
            Console.WriteLine(msg);
            string[] msgs = msg.Split(" ");

            try
            {
                string disName = msgs[0] + ".ChatMsg";
                var msgMethod = distribute.GetMethod(disName);
                msgMethod(msgs, id);
            }
            catch (Exception e)
            {
                Bot.SendMessage.Chat(id, "错误" + e);
            }
        }
    }
}