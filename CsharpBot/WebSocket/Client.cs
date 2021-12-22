using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

namespace CsharpBot
{
    public class Client
    {
        private Bot _bot;
        public ClientFSM _clientFsm;//有限状态机管理websocket连接状态
        private Task cmd;
        private CancellationTokenSource cts;

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
                WebsocketClient.Dispose();
                //var tast = Task.Run(WebsocketClient.Reconnect);
                //tast.Wait();
            }

            WebsocketClient = new WebsocketClient(_bot.websocketUri);
            //使用 有限状态机管理websocket 状态
            WebsocketClient.DisconnectionHappened.Subscribe((info) =>
            {
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

            cts = new CancellationTokenSource();
            //todo 写一个Cmd server，将 全部功能都写成Server？
            cmd = Task.Run(() =>
              {
                  while (!cts.IsCancellationRequested)
                  {
                      //客户端主动指令
                      string input = Console.ReadLine();
                      if (input == "exit")
                      {
                          CloseClient();
                      }
                  }
              }, cts.Token);

            exitEvent.WaitOne();
        }

        internal void Stop()
        {
            cts.Cancel();
            //停止计时器
            _bot.timer.Stop();
            _bot.timer.Dispose();
            WebsocketClient.StopOrFail(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, null);
            var task = WebsocketClient.Stop(WebSocketCloseStatus.Empty, "close");
            task.Wait();
        }

        internal void CloseClient()
        {
            WebsocketClient.StopOrFail(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, null);
            Environment.Exit(0);
        }
    }
}