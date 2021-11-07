using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
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

    internal class Program
    {
        private static string BotToken = "1/MTA1NTg=/LZ2fsaN2Te7hM7mh8bflnA==";
        private static string baseUrl = "https://www.kaiheila.cn";
        private static string apiGateway = "/api/v3/gateway/index";

        private static Uri wevsocketUrl;
        private static string query = "0";//0 不压缩，1 压缩数据

        private static JToken lastSn = 0;//最后一个sn的计数

        private static List<Kmessage> kMessageStack = new List<Kmessage>();//sn消息队列
        private static WebsocketClient client;

        private static void Main(string[] args)
        {
            //websocket 连接 1.Http 获取Gateway,2.解析Gateway url
            DataInit();
            // 开始连接websocket
            ClientStart();
        }

        private static void DataInit()
        {
            kMessageStack.Clear();
            lastSn = 0;
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

        private static void ClientStart()
        {
            var exitEvent = new ManualResetEvent(false);
            if (client != null)
            {
                var tast = Task.Run(client.Reconnect);
                tast.Wait();
            }
            else
            {
                client = new WebsocketClient(wevsocketUrl);

                client.DisconnectionHappened.Subscribe((info) => { Console.WriteLine("客户端： 断开服务器: " + info.Type); });

                client.ReconnectionHappened.Subscribe((info) => { Console.WriteLine("客户端： 连接服务器: " + info.Type); });

                client.MessageReceived.Subscribe(msg =>
                {
                    //分发消息
                    ReceiveMsg(msg);
                });
                var startTast = client.Start();
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
                Console.WriteLine(input);
                if (input == "exit")
                {
                    client.StopOrFail(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, null);
                    Environment.Exit(0);
                }
            });

            exitEvent.WaitOne();
        }

        private static void ReceiveMsg(ResponseMessage msg)
        {
            //todo 每隔36秒 没有收到一次 pong包，视为连接超时， 需主动重连Resume

            //解析
            JObject jo = (JObject)(JsonConvert.DeserializeObject(msg.ToString()));

            //使用反射分发
            jo.TryGetValue("sn", out lastSn);
            Console.WriteLine("客户端：收到消息:" + "sn:" + lastSn + msg.ToString());
            if ((int)jo["s"] == 3)
            {
                //心跳包
            }
            if ((int)jo["s"] == 0)
            {
                //todo 聊天消息，通知，反射分发消息
                if (jo["d"]["content"].ToString() == "大傻逼")
                {
                    string targetId = jo["d"]["target_id"].ToString();
                    Console.WriteLine("客户端：频道id" + targetId);
                    var result= PostSendMessage(targetId, "你是大傻逼");
                    Console.WriteLine("客户端：收到回调" + result);
                }

                if (jo["d"]["content"].ToString() == "gu")
                {
                    string targetId = jo["d"]["target_id"].ToString();
                    Console.WriteLine("客户端：频道id" + targetId);
                    var result = PostSendMessage(targetId, "我是机器人");
                    Console.WriteLine("客户端：收到回调" + result);
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
                        DataInit();
                        // 重新连接websocket
                        ClientStart();
                        break;

                    case 400101:
                        Console.WriteLine("客户端：无效的 token");
                        DataInit();
                        // 重新连接websocket
                        ClientStart();
                        return;
                        break;

                    case 400102:
                        Console.WriteLine("客户端：token 验证失败");
                        DataInit();
                        // 重新连接websocket
                        ClientStart();
                        return;
                        break;

                    case 400103:
                        Console.WriteLine("客户端：token 过期");
                        DataInit();
                        // 重新连接websocket
                        ClientStart();
                        return;
                        break;
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

        private static void Ping()
        {
            JObject pingJobj = new JObject();
            pingJobj.Add("s", 2);
            pingJobj.Add("sn", lastSn);
            string pingJson = JsonConvert.SerializeObject(pingJobj);

            Console.WriteLine("客户端：发送ping" + pingJson);
            client.Send(pingJson);
        }

        /// <summary>
        /// 主动重连
        /// </summary>
        private static void Resume()
        {
            JObject pingJobj = new JObject();
            pingJobj.Add("s", 4);
            pingJobj.Add("sn", lastSn);
            string pingJson = JsonConvert.SerializeObject(pingJobj);

            Console.WriteLine("客户端：发送Resume" + pingJson);
            client.Send(pingJson);
        }

        #endregion websocket 指令

        #region http 指令

        private static Task<string> GetGateway()
        {
            string address = baseUrl + apiGateway + "?compress=0";

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

        ///// <summary>
        ///// 指定Post地址使用Get 方式获取全部字符串
        ///// </summary>
        ///// <param name="url">请求后台地址</param>
        ///// <param name="content">Post提交数据内容(utf-8编码的)</param>
        ///// <returns></returns>
        //public static string Post(string url, string content)
        //{
        //    string result = "";
        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        //    req.Method = "Post";
        //    req.Headers.Add("Authorization", " Bot " + BotToken);
     
        //    #region 添加Post 参数

        //    byte[] data = Encoding.UTF8.GetBytes(content);
        //    req.ContentLength = data.Length;
        //    using (Stream reqStream = req.GetRequestStream())
        //    {
        //        reqStream.Write(data, 0, data.Length);
        //        reqStream.Close();
        //    }

        //    #endregion 添加Post 参数

        //    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
        //    Stream stream = resp.GetResponseStream();
        //    //获取响应内容
        //    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        //    {
        //        result = reader.ReadToEnd();
        //    }
        //    return result;
        //}

        private static string PostSendMessage(string target_id, string content)
        {
            string address = baseUrl + "/api/v3/message/create";

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

                Console.WriteLine(res);

                return res.Result;
            }
        }

        #endregion http 指令
    }
}