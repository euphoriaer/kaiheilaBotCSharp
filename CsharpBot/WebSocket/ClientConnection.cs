using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Websocket.Client;

namespace CsharpBot
{
    public class ClientConnection : IState
    {
        private ClientFSM _clientFsm;

        CancellationTokenSource cts;
        private Task _pingTask;
        private bool isStopTask;
        public ClientConnection(ClientFSM fsm)
        {
            _clientFsm = fsm;
        }
        void IState.OnEnter(string info)
        {
            isStopTask = false;
            _clientFsm.Bot.log.Record("客户端： 服务器连接: " + info);
            Console.WriteLine("客户端： 服务器连接: " + info);
            //连接中每30s发送一次Ping
            cts = new CancellationTokenSource();
            _pingTask = new Task(() =>
            {
                while (true)
                {
                    Ping();
                    Thread.Sleep(30000); //30秒一次心跳包
                }
            }, cts.Token);
            _pingTask.Start();
        }

        internal void Ping()
        {
            JObject pingJobj = new JObject();
            pingJobj.Add("s", 2);
            pingJobj.Add("sn", _clientFsm.Bot.LastSn);
            string pingJson = JsonConvert.SerializeObject(pingJobj);

            Console.WriteLine("客户端：发送ping" + pingJson);
            _clientFsm.Bot.Client.WebsocketClient.Send(pingJson);
        }

        void IState.OnExit()
        {
            if (cts==null)
            {
                return;
            }
            cts.Cancel();
            //退出连接状态不需要发送Ping 
        }
    }
}