using CsharpBot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace MarioMaker
{
    internal partial class Program
    {
        private static Bot Bot;
        private static DistributeUtil<Action<JToken>, AttrMario, Program> distribute;

        public static Config cfg;
        //error 机器人冗余消息
        private const string ChatHelp =
            "马造机器人命令" + "\n" + "\n" +
            @"注册用户：.reg 用户名\密码" + "\n" + "\n";

        private const string ChannelHelp =
            "马造机器人命令" + "\n" + "\n" +
            @"添加关卡：.add 关卡ID\名字\类型" + "\n" + "\n";

        private static void Main(string[] args)
        {
            Console.WriteLine("在开黑啦私聊机器人 .help 查看命令");

            distribute = new DistributeUtil<Action<JToken>, AttrMario, Program>(null);
            var configPath = Path.Combine(System.Environment.CurrentDirectory, "Config.Json");
            cfg = new Config(configPath);
            string botToken = cfg.Read("BotToken");
#if DEBUG
            botToken = "1/MTA1NTg=/LZ2fsaN2Te7hM7mh8bflnA==";//测试机器人Token
#endif

            Bot = new Bot(botToken);

            Bot.MessageListen += Message;
            Bot.Run();
        }

        private static void Message(string msg)
        {
            var serverJson = JsonConvert.DeserializeObject<JToken>(msg);

            var jo = serverJson["d"];

            //组合
            var cmd = jo["content"].ToString().Split(" ")[0];
            var channelType = jo["channel_type"].ToString();
#if !DEBUG
//error  私聊是否会受到影响？
         if (jo["target_id"].ToString() != "8871082168907917"&&channelType!="PERSON")
            {
                return;
            }
#endif

            try
            {
                //分发消息
                var msgMethod = distribute.GetMethod(cmd + "." + channelType);
                msgMethod(jo);
            }
            catch (Exception e)
            {
            }
        }
    }
}