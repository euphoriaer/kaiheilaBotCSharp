using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CsharpBot.Http
{
    internal class Gateway
    {
        private Bot Bot;
        private string GatewayUrl= "https://www.kaiheila.cn/api/v3/gateway/index";
        internal Gateway(Bot bot)
        {
            Bot = bot;
        }

        //    internal static string ApiGateway = "";
        internal Task<string> GetGateway()
        {
            string address = GatewayUrl + "?compress=" + Bot.Query;

            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, address);
                httpRequestMessage.Headers.Add("Authorization", " Bot " + Bot.BotToken);

                var result = client.SendAsync(httpRequestMessage);
                //请求结果client
                var webUrl = result.Result.Content.ReadAsStringAsync();

                Console.WriteLine(webUrl);
                return webUrl;
            }
            return null;
        }
    }
}