using CsharpBot.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Websocket.Client;

namespace CsharpBot
{
    internal interface IBotFunction
    {
        bool Run();

        void CloseBot();
    }

    internal class Kmessage
    {
        public int sn = 0;//消息序号
        public string message;//消息
    }

    public class Bot : IBotFunction
    {
        internal Gateway gateway;

        internal Timer timer;

        internal Log log;

        internal string BotToken;

        internal Uri websocketUri;

        /// <summary>
        /// 0 不压缩，1 压缩数据
        /// </summary>
        internal int Query = 0;

        internal JToken LastSn = 0;//最后一个sn的计数
        internal List<Kmessage> KMessageStack = new List<Kmessage>();//sn消息队列

        /// <summary>
        /// 监听服务器所有消息(包括心跳，握手，断开重连等等),回传Json字符串
        /// </summary>
        public Action<string> JsonListen;

        /// <summary>
        /// 监听服务器聊天消息,回传Json字符串
        /// </summary>
        public Action<string> MessageListen;

        /// <summary>
        /// 私聊消息，返回1消息,2频道ID
        /// </summary>
        public Action<string, string> ChatlMsg;

        /// <summary>
        /// 监听频道消息，返回1消息,2频道ID
        /// </summary>
        public Action<string, string> ChannelMsg;

        public SendMessage SendMessage;

        internal Client Client;

        private DistributeUtil<Action<JObject>, AttrSignal, Bot> Distribute;

        internal Action InitTimerAction;

        /// <summary>
        ///
        /// </summary>
        /// <param name="botToken">机器人Token</param>
        /// <param name="query">默认不加密</param>
        public Bot(string botToken, int query = 0, string logFolderPath = null)
        {
            //error 将日志作为可选项启动
            BotToken = botToken;
            this.Query = query;
            if (logFolderPath == null)
            {
                return;
            }
            log = new Log(logFolderPath, 30, "Bot");
            //通过Gateway 获取websocket 连接地址
            gateway = new Gateway(this);

            //websocket 对象
            Client = new Client(this);

            //发送消息对象
            SendMessage = new SendMessage(this);

            //信令分发对象
            Distribute = new DistributeUtil<Action<JObject>, AttrSignal, Bot>(this);

            InitTimerAction = InitTimer;

            //todo 写一个Cmd server，将 全部功能都写成Server？
            Task.Run(() =>
             {
                 while (true)
                 {
                     //客户端主动指令
                     string input = Console.ReadLine();
                     if (input == "exit")
                     {
                         Client.CloseClient();
                     }
                 }
             });
        }

        public bool Run()
        {
            //websocket 连接 1.Http 获取Gateway,2.解析Gateway url
            if (DataInit() == false)
            {
                return false;
            }

            try
            {
                // 开始连接websocket
                Client.ClientStart();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        internal bool DataInit()
        {
            KMessageStack.Clear();
            LastSn = 0;
            Task<string> gaturl = gateway.GetGateway();
            gaturl?.Wait();
            log.Record(gaturl?.Result);
            if (string.IsNullOrEmpty(gaturl?.Result))
            {
                log.Record("Gateway获取失败");
                Console.WriteLine("Gateway获取失败");
                return false;
            }

            JObject jo = (JObject)(JsonConvert.DeserializeObject(gaturl.Result));

            //解析Gateway 获取到的内容
            if (jo["message"].ToString() == "你的用户凭证不正确")
            {
                log.Record("用户凭证错误 " + BotToken);
                Console.WriteLine("用户凭证错误 " + BotToken);
                Environment.Exit(0);
            }

            string wss = jo["data"]["url"].ToString();
            log.Record("客户端:解析websocket链接  " + wss);
            Console.WriteLine("客户端:解析websocket链接  " + wss);
            websocketUri = new Uri(wss);
            return true;
        }

        /// <summary>
        /// Ping Pong 计时器初始化
        /// </summary>
        internal void InitTimer()
        {
            Console.WriteLine("计时器初始化");
            //设置定时间隔(毫秒为单位)
            int interval = 36000;

            timer?.Stop();
            timer?.Dispose();

            timer = new Timer(interval);
            //设置执行一次（false）还是一直执行(true)
            timer.AutoReset = true;
            //设置是否执行System.Timers.Timer.Elapsed事件
            timer.Enabled = true;
            //绑定Elapsed事件
            timer.Elapsed += PongTimeOut;

            timer.Start();
        }

        private void PongTimeOut(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Ping超时");
            timer?.Stop();
            timer?.Dispose();
            Client._clientFsm.TransitionState(ClientFSM.StateType.Disconnection, "超时");

            //Pong 超时
        }

        internal void ReceiveMsg(ResponseMessage msg)
        {
            //解析
            
            JObject jo = (JObject)(JsonConvert.DeserializeObject(msg.ToString()));

            jo.TryGetValue("sn", out LastSn);
            if (JsonListen != null)
            {
                JsonListen(jo.ToString());
            }
            Console.WriteLine("客户端：收到消息:" + "sn:" + LastSn + msg.ToString());
            //使用 通用消息分发
            var method = Distribute.GetMethod(jo["s"].ToString());
            method(jo);
        }

        [AttrSignal("0")]
        private void Signal0(JObject jo)
        {
            string msgContent = jo["d"]["content"].ToString();

            string channelType = jo["d"]["channel_type"].ToString();

            if (MessageListen != null)
            {
                MessageListen(jo.ToString());
            }

            if (ChannelMsg != null && channelType == "GROUP")
            {
                string targetId = jo["d"]["target_id"].ToString();
                ChannelMsg(msgContent, targetId);
            }

            if (ChatlMsg != null && channelType == "PERSON")
            {
                string targetId = jo["d"]["author_id"].ToString();
                ChatlMsg(msgContent, targetId);
            }
        }

        [AttrSignal("1")]
        private void Signal1(JObject jo)
        {
            //握手结果 400103
            jo.TryGetValue("d", out JToken? d);
            var code = d.Value<int>("code");
            switch (code)
            {
                case 40100:
                    log.Record("客户端：缺少参数");
                    Console.WriteLine("客户端：缺少参数");
                    Client.CloseClient();
                    break;

                case 40101:
                    log.Record("客户端：无效的 token");
                    Console.WriteLine("客户端：无效的 token");
                    Client.CloseClient();

                    break;

                case 40102:
                    log.Record("客户端：token 验证失败");
                    Console.WriteLine("客户端：token 验证失败");
                    Client.CloseClient();

                    break;

                case 40103:
                    Client._clientFsm.TransitionState(ClientFSM.StateType.Disconnection, "40103重启");
                    log.Record("客户端：token 过期");
                    Console.WriteLine("客户端：token 过期");
                    break;

                default:
                    log.Record("客户端：连接成功:" + "状态码，" + code);
                    Console.WriteLine("客户端：连接成功:" + "状态码，" + code);
                    Client._clientFsm.TransitionState(ClientFSM.StateType.Connection, "连接成功");
                    break;
            }
        }

        [AttrSignal("3")]
        private void Signal3(JObject jo)
        {
            //每次收到Pong 重置Timer时间
            if (timer!=null)
            {
                timer.Interval = 36000;
            }
            Console.WriteLine("计时重置");
            //心跳包
        }

        [AttrSignal("5")]
        private void Signal5(JObject jo)
        {
            //记录Bot的日志 单独开一个日志对象
            log.Record("客户端：解析消息，需要断开重连:");
            //需要断开重连
            Client._clientFsm.TransitionState(ClientFSM.StateType.Disconnection, "需要断开重连");
        }

        [AttrSignal("6")]
        private void Signal6(JObject jo)
        {
            log.Record("客户端：解析消息，重连成功");
            //主动重连成功
        }

        public void CloseBot()
        {
            //退出Bot

            //关闭socket连接
            Client.CloseClient();
        }
    }
}