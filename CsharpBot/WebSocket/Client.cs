﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

namespace CsharpBot
{
    public class Client
    {
        private Bot _bot;
        public ClientFSM _clientFsm;//有限状态机管理websocket连接状态

        internal Client(Bot bot)
        {
            _bot = bot;
            _clientFsm = new ClientFSM(bot);
        }

        internal WebsocketClient WebsocketClient;

        internal void ClientStart()
        {
            var exitEvent = new ManualResetEvent(false);

            if (WebsocketClient != null)
            {
                
                var tast = Task.Run(WebsocketClient.Reconnect);
                tast.Wait();
            }
            else
            {
                WebsocketClient = new WebsocketClient(_bot.websocketUri);
                //使用 有限状态机管理websocket 状态
                WebsocketClient.DisconnectionHappened.Subscribe((info) =>
                {
                    _clientFsm.TransitionState(ClientFSM.StateType.Disconnection, info.Type.ToString());
                });

                WebsocketClient.ReconnectionHappened.Subscribe((info) =>
                {
                    _clientFsm.TransitionState(ClientFSM.StateType.Connection, info.Type.ToString());
                });

                WebsocketClient.MessageReceived.Subscribe(msg =>
                {
                    //分发消息
                    _bot.ReceiveMsg(msg);
                });
                var startTast = WebsocketClient.Start();
                startTast.Wait();
            }
            //todo 写一个Cmd server，将 全部功能都写成Server？
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

        internal void CloseClient()
        {
            //停止计时器
            _bot.timer.Dispose();
            WebsocketClient.StopOrFail(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, null);
            Environment.Exit(0);
        }
    }
}