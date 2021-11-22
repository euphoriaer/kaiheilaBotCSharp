using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MarioMaker
{
    internal partial class Program
    {
        [AttrMario(".add.GROUP")]
        public static void AddGuanqia(JToken jObject)
        {
            var msgs = jObject["content"].ToString().Split(" ")[1].Split(@"\");
            string kaiheilaId = jObject["author_id"].ToString();
           string targetID= jObject["target_id"].ToString();

            JObject msgJobj = new JObject();
            msgJobj.Add("levelId", msgs[0]);
            msgJobj.Add("levelName", msgs[1]);
            msgJobj.Add("levelType", msgs[2]);
            msgJobj.Add("kaiheilaId", kaiheilaId);
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
                    Bot.SendMessage.Channel(targetID, "回调错误，post返回为空");
                    return;
                }

                Bot.SendMessage.Channel(jObject["target_id"].ToString(), "结果：" + res.Result);
            }
        }

       
    }
}