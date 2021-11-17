using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MarioMaker
{
    internal partial class Program
    {
        [MarioAttr(".add.ChannelMsg")]
        public static void AddGuanqia(string[] msgs, string id)
        {
            JObject msgJobj = new JObject();
            msgJobj.Add("levelId", msgs[1]);
            msgJobj.Add("levelName", msgs[2]);
            msgJobj.Add("levelType", msgs[3]);
            string msgJson = JsonConvert.SerializeObject(msgJobj);

            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, cfg.Read("Add"));
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

                Bot.SendMessage.Channel(id, "结果：" + res.Result);
            }
        }

        [MarioAttr(".help.ChannelMsg")]
        private static void HelpChannel(string[] msgs, string id)
        {
            Bot.SendMessage.Channel(id, ChannelHelp);
        }
    }
}