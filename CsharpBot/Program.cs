using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

namespace CsharpBot
{
    internal class Kmessage
    {
        public int sn = 0;//消息序号
        public string message;//消息
    }

    public class Bot
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="botToken">机器人Token</param>
        /// <param name="query">默认不加密</param>
        public Bot(string botToken, int query = 0)
        {
            BotToken = botToken;
            this.Query = query;
        }

        private string BotToken;
        private string BaseUrl = "https://www.kaiheila.cn";
        private string ApiGateway = "/api/v3/gateway/index";

        private Uri wevsocketUrl;

        /// <summary>
        /// 0 不压缩，1 压缩数据
        /// </summary>
        private int Query = 0;

        private JToken LastSn = 0;//最后一个sn的计数

        private List<Kmessage> KMessageStack = new List<Kmessage>();//sn消息队列
        private WebsocketClient Client;

        /// <summary>
        /// 监听服务器,回传Json字符串
        /// </summary>
        public Action<string> JsonListen;

        /// <summary>
        /// 监听频道消息，返回1消息,2频道ID
        /// </summary>
        public Action<string, string> ChatMsg;
        
        public void Run()
        {
            //websocket 连接 1.Http 获取Gateway,2.解析Gateway url
            DataInit();
            // 开始连接websocket
            ClientStart();
        }
        /// <summary>
        /// 数据初始化
        /// </summary>
        private void DataInit()
        {
            KMessageStack.Clear();
            LastSn = 0;
            Task<string> gaturl = GetGateway();
            gaturl.Wait();
            Console.WriteLine(gaturl.Result);
            if (string.IsNullOrEmpty(gaturl.Result))
            {
                Console.WriteLine("Gateway获取失败");
                Environment.Exit(0);
            }
            else if (gaturl.Result.Length <= 0)
            {
            }
            JObject jo = (JObject)(JsonConvert.DeserializeObject(gaturl.Result));
            string wss = jo["data"]["url"].ToString();
            Console.WriteLine("客户端:解析websocket链接  " + wss);

            wevsocketUrl = new Uri(wss);
        }

        private void ClientStart()
        {
            var exitEvent = new ManualResetEvent(false);
            if (Client != null)
            {
                var tast = Task.Run(Client.Reconnect);
                tast.Wait();
            }
            else
            {
                Client = new WebsocketClient(wevsocketUrl);

                Client.DisconnectionHappened.Subscribe((info) => { Console.WriteLine("客户端： 断开服务器: " + info.Type); });

                Client.ReconnectionHappened.Subscribe((info) => { Console.WriteLine("客户端： 连接服务器: " + info.Type); });

                Client.MessageReceived.Subscribe(msg =>
                {
                    //分发消息
                    ReceiveMsg(msg);
                });
                var startTast = Client.Start();
                startTast.Wait();
            }

            Task.Run(() =>
            {
                while (true)
                {
                    Ping();
                    Thread.Sleep(30000);//30秒一次心跳包
                }
            });

            Task.Run(() =>
            {   //客户端主动指令
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    CloseClient();
                }
            });

            exitEvent.WaitOne();
        }

        public void CloseClient()
        {
            Client.StopOrFail(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, null);
            Environment.Exit(0);
        }

        private void ReceiveMsg(ResponseMessage msg)
        {
            //todo 每隔36秒 没有收到一次 pong包，视为连接超时， 需主动重连Resume

            //解析
            JObject jo = (JObject)(JsonConvert.DeserializeObject(msg.ToString()));

            //使用反射分发
            jo.TryGetValue("sn", out LastSn);
            if (JsonListen != null)
            {
                JsonListen(jo.ToString());
            }
            Console.WriteLine("客户端：收到消息:" + "sn:" + LastSn + msg.ToString());
            if ((int)jo["s"] == 3)
            {
                //心跳包
            }
            if ((int)jo["s"] == 0)
            {
                string chatMsg = jo["d"]["content"].ToString();
                string targetId = jo["d"]["target_id"].ToString();
                Console.WriteLine("客户端：频道id" + targetId);
                if (ChatMsg != null)
                {
                    ChatMsg(chatMsg, targetId);
                }
            }
            if ((int)jo["s"] == 1)
            {
                //握手结果 400103
                jo.TryGetValue("d", out JToken? d);
                var code = d.Value<int>("code");

                switch (code)
                {
                    case 400100:
                        Console.WriteLine("客户端：缺少参数");
                        CloseClient();
                        break;

                    case 400101:
                        Console.WriteLine("客户端：无效的 token");
                        CloseClient();

                        break;

                    case 400102:
                        Console.WriteLine("客户端：token 验证失败");
                        CloseClient();

                        break;

                    case 400103:
                        Console.WriteLine("客户端：token 过期");
                        CloseClient();
                        break;

                    default:
                        Console.WriteLine("客户端：连接成功:" + "状态码，" + code);
                        break;
                }
            }
            if ((int)jo["s"] == 5)
            {
                Console.WriteLine("客户端：解析消息，需要断开重连:" + msg.ToString());
                //需要断开重连
                DataInit();//数据初始化
                ClientStart();//开始连接
            }
            if ((int)jo["s"] == 6)
            {
                Console.WriteLine("客户端：解析消息，重连成功：" + msg.ToString());
                //主动重连成功
            }
        }

        #region websocket 指令

        private void Ping()
        {
            JObject pingJobj = new JObject();
            pingJobj.Add("s", 2);
            pingJobj.Add("sn", LastSn);
            string pingJson = JsonConvert.SerializeObject(pingJobj);

            Console.WriteLine("客户端：发送ping" + pingJson);
            Client.Send(pingJson);
        }

        /// <summary>
        /// 主动重连
        /// </summary>
        private void Resume()
        {
            JObject pingJobj = new JObject();
            pingJobj.Add("s", 4);
            pingJobj.Add("sn", LastSn);
            string pingJson = JsonConvert.SerializeObject(pingJobj);

            Console.WriteLine("客户端：发送Resume" + pingJson);
            Client.Send(pingJson);
        }

        #endregion websocket 指令

        #region http 指令

        private Task<string> GetGateway()
        {
            string address = BaseUrl + ApiGateway + "?compress=" + Query;

            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, address);
                httpRequestMessage.Headers.Add("Authorization", " Bot " + BotToken);

                var result = client.SendAsync(httpRequestMessage);
                //请求结果client
                //string result = client.GetAsync(address).Result.Content.ReadAsStringAsync().Result;
                var webUrl = result.Result.Content.ReadAsStringAsync();

                Console.WriteLine(webUrl);
                return webUrl;
            }
            return null;
        }

        /// <summary>
        /// 频道id，内容
        /// </summary>
        /// <param name="target_id">频道id</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public string SendMessage(string target_id, string content)
        {
            string address = BaseUrl + "/api/v3/message/create";

            JObject msgJobj = new JObject();
            msgJobj.Add("target_id", target_id);
            msgJobj.Add("content", content);
            string msgJson = JsonConvert.SerializeObject(msgJobj);

            using (var client = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, address);
                httpRequestMessage.Headers.Add("Authorization", " Bot " + BotToken);
                httpRequestMessage.Content = new StringContent(msgJson);
                httpRequestMessage.Content.Headers.Remove("Content-type");
                httpRequestMessage.Content.Headers.Add("Content-type", "application/json");
                var result = client.SendAsync(httpRequestMessage);//返回结果
                var res = result.Result.Content.ReadAsStringAsync();
                return res.Result;
            }
        }

        #endregion http 指令
    }
}