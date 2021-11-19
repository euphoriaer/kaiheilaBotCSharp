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
        public string Channel(string target_id, string content, string url = "https://www.kaiheila.cn/api/v3/message/create")
        {
            string address = url;

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
        /// 私聊回话,用户id,内容
        /// </summary>
        /// <param name="target_id">用户id</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public string Chat(string target_id, string content, string url = "https://www.kaiheila.cn/api/v3/direct-message/create")
        {
            string address = url;

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
        /// 通用Post方法
        /// </summary>
        /// <param name="httpUrl">地址</param>
        /// <param name="contentJson">Json</param>
        /// <returns></returns>
        public string Post(string httpUrl, string contentJson)
        {
            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, httpUrl.ToString());
                httpRequestMessage.Headers.Add("Authorization", " Bot " + Bot.BotToken);
                httpRequestMessage.Content = new StringContent(contentJson);
                httpRequestMessage.Content.Headers.Remove("Content-type");
                httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                var result = client.SendAsync(httpRequestMessage);//返回结果
                var res = result.Result.Content.ReadAsStringAsync();
                res.Wait();
                return res.Result;
            }
        }

        /// <summary>
        /// 通用Get方法
        /// </summary>
        /// <param name="httpUrl">地址</param>
        /// <param name="contentJson">Json</param>
        /// <returns></returns>
        public string Get(string httpUrl, string contentJson)
        {
            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, httpUrl.ToString());
                httpRequestMessage.Headers.Add("Authorization", " Bot " + Bot.BotToken);
                httpRequestMessage.Content = new StringContent(contentJson);
                httpRequestMessage.Content.Headers.Remove("Content-type");
                httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                var result = client.SendAsync(httpRequestMessage);//返回结果
                var res = result.Result.Content.ReadAsStringAsync();
                res.Wait();
                return res.Result;
            }
        }
    }
}