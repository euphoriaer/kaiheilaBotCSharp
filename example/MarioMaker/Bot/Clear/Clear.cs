using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MarioMaker
{
    internal partial class Program
    {
        [AttrMario(".add.GROUP")]
        public static void Clear(JToken jObject)
        {
            var msgs = jObject["content"].ToString().Split(" ");
            string kaiheilaId = jObject["author_id"].ToString();
            var com = msgs[1].Split(@"/");//字符分割为数组

            JObject msgJobj = new JObject();
            msgJobj.Add("levelId", com[0]);
            msgJobj.Add("defficultyVote", com[1]);
            msgJobj.Add("isLike", com[2]);
            msgJobj.Add("kaiheilaId", kaiheilaId);
            string msgJson = JsonConvert.SerializeObject(msgJobj);

            using (var client = new HttpClient())//转发到鼠宝
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, cfg.Read("Clear"));
                httpRequestMessage.Content = new StringContent(msgJson);
                httpRequestMessage.Content.Headers.Remove("Content-type");
                httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                var result = client.SendAsync(httpRequestMessage); //返回结果
                var res = result.Result.Content.ReadAsStringAsync();
                res.Wait();

                if (string.IsNullOrEmpty(res.Result))//转发消息给bot
                {
                    Bot.SendMessage.Channel(jObject["target_id"].ToString(), "回调错误，post返回为空");
                    return;
                }

                Bot.SendMessage.Channel(jObject["target_id"].ToString(), "" + res.Result);
            }
        }

        [AttrMario(".help.GROUP")]
        private static void ClearChannel(JToken jObject)
        {
            Bot.SendMessage.Channel(jObject["target_id"].ToString(), ChannelHelp);
        }
    }
}