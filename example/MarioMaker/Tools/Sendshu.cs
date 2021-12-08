using System.Net.Http;

namespace MarioMaker
{
    internal partial class Program
    {
        public static string? SendShu(string Http, string? msgJson)
        {
            _log.Record("发送鼠消息： 地址：" + Http + "\n msg:" + msgJson);
            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, Http);
                httpRequestMessage.Content = new StringContent(msgJson);
                httpRequestMessage.Content.Headers.Remove("Content-type");
                httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                var result = client.SendAsync(httpRequestMessage); //返回结果
                var res = result.Result.Content.ReadAsStringAsync();
                res.Wait();
                _log.Record("收到鼠消息：" + res.Result);
                return res.Result;
            }
        }
    }
}