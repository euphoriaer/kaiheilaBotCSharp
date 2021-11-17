using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MarioMaker
{
    internal partial class Program
    {
        [MarioAttr(".help.ChatMsg")]
        private static void HelpChat(string[] msgs, string id)
        {
            Bot.SendMessage.Chat(id, ChannelHelp);
        }

        [MarioAttr(".login.ChatMsg")]
        private static void SendLogin(string[] msgs, string id)
        {
            if (msgs.Length == 2)
            {
                string user = msgs[1].Split("/")[0];
                string pwd = msgs[1].Split("/")[1];
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pwd))
                {
                    Bot.SendMessage.Chat(id, "错误输入");
                }

                JObject msgJobj = new JObject();
                msgJobj.Add("playerName", user);
                msgJobj.Add("password", pwd);
                string msgJson = JsonConvert.SerializeObject(msgJobj);

                using (var client = new HttpClient())
                {
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, cfg.Read("Login"));
                    httpRequestMessage.Content = new StringContent(msgJson);
                    httpRequestMessage.Content.Headers.Remove("Content-type");
                    httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                    var result = client.SendAsync(httpRequestMessage);//返回结果
                    var res = result.Result.Content.ReadAsStringAsync();
                    res.Wait();
                    if (string.IsNullOrEmpty(res.Result))
                    {
                        Bot.SendMessage.Chat(id, "回调错误，post返回为空");
                    }
                    Bot.SendMessage.Chat(id, "结果：" + res.Result);
                }
            }
        }

        [MarioAttr(".reg.ChatMsg")]
        public static void SendRegister(string[] msgs, string id)
        {
            if (msgs.Length == 2)
            {
                string[] userData = msgs[1].Split("/");
                if (userData.Length != 2)
                {
                    Bot.SendMessage.Chat(id, "错误输入");
                    return;
                }
                string user = msgs[1].Split("/")[0];
                string pwd = msgs[1].Split("/")[1];
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pwd))
                {
                    Bot.SendMessage.Chat(id, "错误输入");
                    return;
                }

                JObject msgJobj = new JObject();
                msgJobj.Add("kaiheilaId", id);
                msgJobj.Add("playerName", user);
                msgJobj.Add("password", pwd);
                string msgJson = JsonConvert.SerializeObject(msgJobj);

                using (var client = new HttpClient())
                {
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, cfg.Read("Register"));
                    httpRequestMessage.Content = new StringContent(msgJson);
                    httpRequestMessage.Content.Headers.Remove("Content-type");
                    httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                    var result = client.SendAsync(httpRequestMessage); //返回结果
                    var res = result.Result.Content.ReadAsStringAsync();
                    res.Wait();

                    if (string.IsNullOrEmpty(res.Result))
                    {
                        Bot.SendMessage.Chat(id, "回调错误，post返回为空");
                        return;
                    }

                    Bot.SendMessage.Chat(id, "结果：" + res.Result);
                }
            }
        }
    }
}