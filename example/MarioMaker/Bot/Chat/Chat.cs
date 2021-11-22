using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MarioMaker
{
    internal partial class Program
    {
       

        [AttrMario(".reg.PERSON")]
        public static void SendRegister(JToken jObject)
        {
            var msgs = jObject["content"].ToString().Split(" ")[1].Split(@"\");
            string kaiheilaId = jObject["author_id"].ToString();


            JObject msgJobj = new JObject();
            msgJobj.Add("kaiheilaId", kaiheilaId);
            msgJobj.Add("playerName", msgs[0]);
            msgJobj.Add("password", msgs[1]);
            string msgJson = JsonConvert.SerializeObject(msgJobj);

            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, cfg.Read("Reg"));
                httpRequestMessage.Content = new StringContent(msgJson);
                httpRequestMessage.Content.Headers.Remove("Content-type");
                httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                var result = client.SendAsync(httpRequestMessage); //返回结果
                var res = result.Result.Content.ReadAsStringAsync();
                res.Wait();

                if (string.IsNullOrEmpty(res.Result))
                {
                    Bot.SendMessage.Chat(kaiheilaId, "回调错误，post返回为空");
                    return;
                }

                Bot.SendMessage.Chat(kaiheilaId, "结果：" + res.Result);
            }
        }
    }
}