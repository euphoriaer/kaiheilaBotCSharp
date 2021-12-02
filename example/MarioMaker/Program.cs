using CsharpBot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace MarioMaker
{
    internal partial class Program
    {
        private static Bot _bot;
        private static DistributeUtil<Action<JToken>, AttrMario, Program> _distributeUtil;
        private static string _baseUrl = "https://www.kaiheila.cn";
        public static Config Cfg;
        private static string RegSuccess = "[{\"type\":\"card\",\"theme\":\"info\",\"size\":\"lg\",\"modules\":[{\"type\":\"section\",\"text\":{\"type\":\"kmarkdown\",\"content\":\"注册成功！\"}},{\"type\":\"divider\"}]}]";
        private static string RegDefault = "[{\"type\":\"card\",\"theme\":\"danger\",\"size\":\"lg\",\"modules\":[{\"type\":\"header\",\"text\":{\"type\":\"plain-text\",\"content\":\"出现错误!\"}},{\"type\":\"section\",\"text\":{\"type\":\"kmarkdown\",\"content\":\"#{msg}\"}},{\"type\":\"divider\"}]}]";
        private static string ClearSuccess = "[{\"type\":\"card\",\"theme\":\"info\",\"size\":\"lg\",\"modules\":[{\"type\":\"section\",\"text\":{\"type\":\"kmarkdown\",\"content\":\"恭喜你！头发又掉了一撮！\"}},{\"type\":\"divider\"}]}]";
        private static string SdSuccess = "[{\"type\":\"card\",\"theme\":\"info\",\"size\":\"lg\",\"modules\":[{\"type\":\"section\",\"text\":{\"type\":\"kmarkdown\",\"content\":\"修改成功！\"}},{\"type\":\"divider\"}]}]";
        private static string RSuccess = "[{\"type\":\"card\",\"theme\":\"secondary\",\"size\":\"lg\",\"color\":\"#98FB98\",\"modules\":[{\"type\":\"section\",\"text\":{\"type\":\"kmarkdown\",\"content\":\"**你随机到了 ***#{levelId}***,以下是关卡信息： **\"}}]},{\"type\":\"card\",\"theme\":\"secondary\",\"size\":\"lg\",\"color\":\"#98FB98\",\"modules\":[{\"type\":\"section\",\"text\":{\"type\":\"kmarkdown\",\"content\":\"\"}}]}]";
        private static string DSuccess = "[{\"type\":\"card\",\"theme\":\"secondary\",\"size\":\"lg\",\"color\":\"#98FB98\",\"modules\":[{\"type\":\"section\",\"text\":{\"type\":\"kmarkdown\",\"content\":\"**你查询的关卡名为 ***#{levelId}***,以下是关卡信息： **\"}}]},{\"type\":\"card\",\"theme\":\"secondary\",\"size\":\"lg\",\"color\":\"#98FB98\",\"modules\":[{\"type\":\"section\",\"text\":{\"type\":\"kmarkdown\",\"content\":\"\"}}]}]";

        //error 机器人冗余消息
        private const string ChatHelp =
            "马造机器人命令" + "\n" + "\n" +
            @"注册：.reg 用户名\密码" + "\n" + "\n";

        private const string ChannelHelp =
            "马造机器人命令" + "\n" + "\n" +
            @"添加关卡：.add 关卡ID\名字\类型" + "\n" + "\n" +
            @"过关：.clear 关卡id\关卡难度\是否喜欢" + "\n" + "\n" +
            @"设置关卡难度以及分数：.sd 关卡id\关卡难度" + "\n" + "\n" +
            @"设置关卡状态：.ss 关卡id\关卡状态" + "\n" + "\n" +
            @"设置关卡视频地址以及简介：.sv 关卡id\视频地址\简介" + "\n" + "\n" +
            @"随机难度为输入的数字：.r 关卡难度" + "\n" + "\n" +
            @"查看关卡详情：.d 关卡名" + "\n" + "\n" +
            @"随机类型为？的地图：.rt 类型" + "\n" + "\n";

        private static void Main(string[] args)
        {
            Console.WriteLine("在开黑啦私聊机器人 .help 查看命令");

            _distributeUtil = new DistributeUtil<Action<JToken>, AttrMario, Program>(null);
            var configPath = Path.Combine(System.Environment.CurrentDirectory, "config.json");

            Cfg = new Config(configPath);
            string botToken = Cfg.Read("BotToken");
#if DEBUG
            botToken = Cfg.Read("TestBotToken");//测试机器人Token "1/MTA1NTg=/LZ2fsaN2Te7hM7mh8bflnA=="
#endif

            _bot = new Bot(botToken);

            _bot.MessageListen += Message;
            _bot.Run();
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
                var msgMethod = _distributeUtil.GetMethod(cmd + "." + channelType);
                msgMethod(jo);
            }
            catch (Exception e)
            {
            }
        }
    }
}