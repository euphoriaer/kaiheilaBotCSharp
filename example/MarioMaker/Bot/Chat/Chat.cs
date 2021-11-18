using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace MarioMaker
{
    internal partial class Program
    {
        [MarioAttr(".help.PERSON")]
        private static void HelpChat(JToken jObject)
        {
            string kaiheilaId = jObject["author_id"].ToString();
            Bot.SendMessage.Chat(kaiheilaId, ChannelHelp);
        }

        //[MarioAttr(".login.ChatMsg")]
        //private static void SendLogin(JToken jObject)
        //{
        //    var msgs = jObject["content"].ToString().Split(" ");
        //    string kaiheilaId = jObject["author_id"].ToString();

        //    JObject msgJobj = new JObject();
        //    msgJobj.Add("playerName", msgs[1]);
        //    msgJobj.Add("password", msgs[2]);
        //    string msgJson = JsonConvert.SerializeObject(msgJobj);

        //    using (var client = new HttpClient())
        //    {
        //        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, cfg.Read("Login"));
        //        httpRequestMessage.Content = new StringContent(msgJson);
        //        httpRequestMessage.Content.Headers.Remove("Content-type");
        //        httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
        //        var result = client.SendAsync(httpRequestMessage);//返回结果
        //        var res = result.Result.Content.ReadAsStringAsync();
        //        res.Wait();
        //        if (string.IsNullOrEmpty(res.Result))
        //        {
        //            Bot.SendMessage.Chat(kaiheilaId, "回调错误，post返回为空");
        //        }
        //        Bot.SendMessage.Chat(kaiheilaId, "结果：" + res.Result);
        //    }
        //}

        [MarioAttr(".reg.PERSON")]
        public static void SendRegister(JToken jObject)
        {
            var msgs = jObject["content"].ToString().Split(" ");
            string kaiheilaId = jObject["author_id"].ToString();


            JObject msgJobj = new JObject();
            msgJobj.Add("kaiheilaId", kaiheilaId);
            msgJobj.Add("playerName", msgs[1]);
            msgJobj.Add("password", msgs[2]);
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
                    Bot.SendMessage.Chat(kaiheilaId, "回调错误，post返回为空");
                    return;
                }

                Bot.SendMessage.Chat(kaiheilaId, "结果：" + res.Result);
            }
        }
    }
}