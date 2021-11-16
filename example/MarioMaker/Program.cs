using CsharpBot;
using System;

namespace MarioMaker
{
    internal partial class Program
    {
        private static string Register;

        private static string Login;
        private static Bot Bot;
        private static DistributeUtil<Action<string[], string>, MarioAttr, Program> distribute;

        private const string ChatHelp =
            "马造机器人命令" + "\n" + "\n" +
            "注册用户：.注册 user/password" + "\n" + "\n" +
            "登录用户：.登录 user/password" + "\n";

        private const string ChannelHelp =
            "马造机器人命令" + "\n" + "\n" +
            "添加关卡：.add 关卡id(XXX-XXX-XXX) 关卡名字 类型(关卡种类: 1:Kaizo，2跑酷，3微操，4极限，5解密，6工艺，7平台跳跃，8自由风格,类型只要输入数字)" + "\n" + "\n" +
            "修改难度：.sd 关卡id 难度(0.5-10)" + "\n";

        private static void Main(string[] args)
        {
            Console.WriteLine("输入机器人Token");
            var bottoken = Console.ReadLine();
            Console.WriteLine("输入注册Post地址");
            Register = Console.ReadLine();
            Console.WriteLine("输入登录Post地址");
            Login = Console.ReadLine();
            Console.WriteLine("在开黑啦私聊机器人 .help 查看命令");

            distribute = new DistributeUtil<Action<string[], string>, MarioAttr, Program>(null);

            Bot = new CsharpBot.Bot(bottoken);
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
                var msgMethod = distribute.GetMethod(msgs[0]);
                msgMethod(msgs, id);
            }
            catch (Exception e)
            {
                Bot.SendMessage.Chat(id, "错误" + e);
            }
        }
    }
}