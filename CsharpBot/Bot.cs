﻿using CsharpBot.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Websocket.Client;

namespace CsharpBot
{
    internal interface IBotFunction
    {
        void Run();

        void CloseBot();
    }

    internal class Kmessage
    {
        public int sn = 0;//消息序号
        public string message;//消息
    }

    public class Bot : IBotFunction
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

        public void Run()
        {
            //websocket 连接 1.Http 获取Gateway,2.解析Gateway url
            DataInit();
            // 开始连接websocket
            Client.ClientStart();
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        private void DataInit()
        {
            KMessageStack.Clear();
            LastSn = 0;
            //通过Gateway 获取websocket 连接地址
            Task<string> gaturl = new Gateway(this).GetGateway();
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
            //解析Gateway 获取到的内容
            JObject jo = (JObject)(JsonConvert.DeserializeObject(gaturl.Result));
            string wss = jo["data"]["url"].ToString();
            Console.WriteLine("客户端:解析websocket链接  " + wss);
            websocketUri = new Uri(wss);

            //websocket 对象
            Client = new Client(this);
            //发送消息对象
            SendMessage = new SendMessage(this);
            //信令分发对象
            Distribute = new DistributeUtil<Action<JObject>, AttrSignal, Bot>(this);
        }

        internal void ReceiveMsg(ResponseMessage msg)
        {
            //todo 每隔36秒 没有收到一次 pong包，视为连接超时,再发两次ping，还没有收到， 需主动重连Resume

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
                case 400100:
                    Console.WriteLine("客户端：缺少参数");
                    Client.CloseClient();
                    break;

                case 400101:
                    Console.WriteLine("客户端：无效的 token");
                    Client.CloseClient();

                    break;

                case 400102:
                    Console.WriteLine("客户端：token 验证失败");
                    Client.CloseClient();

                    break;

                case 400103:
                    Console.WriteLine("客户端：token 过期");
                    Client.CloseClient();
                    break;

                default:
                    Console.WriteLine("客户端：连接成功:" + "状态码，" + code);
                    break;
            }
        }

        [AttrSignal("3")]
        private void Signal3(JObject jo)
        {
            //心跳包
        }

        [AttrSignal("5")]
        private void Signal5(JObject jo)
        {
            Console.WriteLine("客户端：解析消息，需要断开重连:");
            //需要断开重连
            DataInit();//数据初始化
            Client.CloseClient();//开始连接
        }

        [AttrSignal("6")]
        private void Signal6(JObject jo)
        {
            Console.WriteLine("客户端：解析消息，重连成功");
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