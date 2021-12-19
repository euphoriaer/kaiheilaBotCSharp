using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CsharpBot.Http
{
    internal class Gateway
    {
        private Bot _bot;
        private string _gatewayUrl = "https://www.kaiheila.cn/api/v3/gateway/index";

        internal Gateway(Bot bot)
        {
            _bot = bot;
        }

        //    internal static string ApiGateway = "";
        internal Task<string> GetGateway()
        {
            string address = _gatewayUrl + "?compress=" + _bot.Query;

            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, address);
                httpRequestMessage.Headers.Add("Authorization", " Bot " + _bot.BotToken);

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