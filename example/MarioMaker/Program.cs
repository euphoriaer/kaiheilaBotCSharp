using CsharpBot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace MarioMaker
{
    internal class Program
    {
        private static string Register;

        private static string Login;
        private static Bot Bot;

        private static string HelpCmd =
            "马造机器人命令" + "\n" +
            ".注册 user/password——注册用户" + "\n" +
            ".登录 user/password——登录用户，然后可以查询" + "\n";

        private static void Main(string[] args)
        {
            Console.WriteLine("输入注册Post地址");
            Register = Console.ReadLine();
            Console.WriteLine("输入登录Post地址");
            Login = Console.ReadLine();
            Console.WriteLine("在开黑啦私聊机器人 .help 查看命令");

            Bot = new Bot("1/MTA1NTg=/LZ2fsaN2Te7hM7mh8bflnA==");
            Bot.ChatlMsg += ChatMsg;
            Bot.Run();
        }

        private static void ChatMsg(string msg, string id)
        {
            Console.WriteLine(msg);
            string[] msgs = msg.Split(" ");
            //todo 优化，使用反射分发消息
            if (msgs[0] == ".help")
            {
                Bot.SendMessage.Chat(id, HelpCmd);
            }

            if (msgs[0] == ".注册" && msgs.Length == 2)
            {
                string user = msgs[1].Split("/")[0];
                string pwd = msgs[1].Split("/")[1];
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pwd))
                {
                    Bot.SendMessage.Chat(id, "错误输入");
                }

                string res = SendRegister(id, user, pwd);
                Bot.SendMessage.Chat(id, "注册结果" + res);
            }

            if (msgs[0] == ".登录" && msgs.Length == 2)
            {
                string user = msgs[1].Split("/")[0];
                string pwd = msgs[1].Split("/")[1];
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pwd))
                {
                    Bot.SendMessage.Chat(id, "错误输入");
                }

                string res = SendLogin( user, pwd);
                Bot.SendMessage.Chat(id, "登录结果" + res);
            }

        }

        private static string SendLogin(string user, string pwd)
        {
            JObject msgJobj = new JObject();
            msgJobj.Add("password", user);
            msgJobj.Add("playerName", pwd);
            string msgJson = JsonConvert.SerializeObject(msgJobj);

            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, Login);
                httpRequestMessage.Content = new StringContent(msgJson);
                httpRequestMessage.Content.Headers.Remove("Content-type");
                httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                var result = client.SendAsync(httpRequestMessage);//返回结果
                var res = result.Result.Content.ReadAsStringAsync();
                return res.Result;
            }
        }


        public static string SendRegister(string targetId, string user, string pwd)
        {
            JObject msgJobj = new JObject();
            msgJobj.Add("kaiheilaId", targetId);
            msgJobj.Add("password", user);
            msgJobj.Add("playerName", pwd);
            string msgJson = JsonConvert.SerializeObject(msgJobj);

            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, Register);
                httpRequestMessage.Content = new StringContent(msgJson);
                httpRequestMessage.Content.Headers.Remove("Content-type");
                httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                var result = client.SendAsync(httpRequestMessage);//返回结果
                var res = result.Result.Content.ReadAsStringAsync();
                return res.Result;
            }
        }
    }
}