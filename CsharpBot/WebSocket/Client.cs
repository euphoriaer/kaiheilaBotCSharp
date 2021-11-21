using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

namespace CsharpBot
{
    public class Client
    {
        private Bot Bot;
        private ClientFSM ClientFSM;//有限状态机管理websocket连接状态

        internal Client(Bot bot)
        {
            Bot = bot;
            ClientFSM = new ClientFSM(bot);
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
                WebsocketClient = new WebsocketClient(Bot.websocketUri);
                //error WebsocketClient 使用 有限状态机
                WebsocketClient.DisconnectionHappened.Subscribe((info) =>
                {
                    ClientFSM.TransitionState(ClientFSM.StateType.Disconnection, info.Type.ToString());
                    //Console.WriteLine("客户端： 断开服务器: " + info.Type);
                });

                WebsocketClient.ReconnectionHappened.Subscribe((info) =>
                {
                    ClientFSM.TransitionState(ClientFSM.StateType.Connection, info.Type.ToString());
                    //Console.WriteLine("客户端： 连接服务器: " + info.Type);
                });

                WebsocketClient.MessageReceived.Subscribe(msg =>
                {
                    //分发消息
                    Bot.ReceiveMsg(msg);
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
            WebsocketClient.StopOrFail(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, null);
            Environment.Exit(0);
        }

    }
}