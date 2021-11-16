using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace CsharpBot
{
    public class SendMessage
    {
        internal Bot Bot;

        internal SendMessage(Bot bot)
        {
            Bot = bot;
        }

        /// <summary>
        /// 频道id，内容
        /// </summary>
        /// <param name="target_id">频道id</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public string Channel(string target_id, string content)
        {
            string address = KhlApi.BaseUrl + KhlApi.ChannelCreate;

            JObject msgJobj = new JObject();
            msgJobj.Add("target_id", target_id);
            msgJobj.Add("content", content);
            string msgJson = JsonConvert.SerializeObject(msgJobj);

            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, address);
                httpRequestMessage.Headers.Add("Authorization", " Bot " + Bot.BotToken);
                httpRequestMessage.Content = new StringContent(msgJson);
                httpRequestMessage.Content.Headers.Remove("Content-type");
                httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                var result = client.SendAsync(httpRequestMessage);//返回结果
                var res = result.Result.Content.ReadAsStringAsync();
                return res.Result;
            }
        }

        /// <summary>
        /// 私聊回话
        /// </summary>
        /// <param name="target_id">用户id</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public string Chat(string target_id, string content)
        {
            string address = KhlApi.BaseUrl + KhlApi.ChatCreate;

            JObject msgJobj = new JObject();
            msgJobj.Add("target_id", target_id);
            msgJobj.Add("content", content);
            string msgJson = JsonConvert.SerializeObject(msgJobj);

            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, address);
                httpRequestMessage.Headers.Add("Authorization", " Bot " + Bot.BotToken);
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