using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CsharpBot
{
    public class ClientConnection : StateBase
    {
        private ClientFSM _clientFsm;
        private Task _pingTask;

        private ManualResetEvent _resetEvent;

        public ClientConnection(ClientFSM fsm)
        {
            base.CurStateType = ClientFSM.StateType.Connection;
            _clientFsm = fsm;
            _resetEvent = new ManualResetEvent(false);
            _pingTask = new Task(() =>
            {
                while (true)
                {
                    _resetEvent.WaitOne();
                    Ping();
                    Thread.Sleep(30000); //30秒一次心跳包
                                         //error Ping Pong 放在一起，超时即退出
                }
            });
            _pingTask.Start();
        }

        public override void OnEnter(string info)
        {
            _clientFsm.Bot.log.Record("客户端： 服务器连接: " + info);
            Console.WriteLine("客户端： 服务器连接: " + info);
            Console.WriteLine("开启ping");
            Ping();
            _resetEvent.Set();
            //连接中每30s发送一次Ping

            //error 启动 Pong 计时器  error Ping Pong 重启不生效
            _clientFsm.Bot.InitTimerAction();
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

        public override void OnExit()
        {
            _resetEvent.Reset();

            _clientFsm.Bot.timer?.Stop();
            _clientFsm.Bot.timer?.Dispose();

            _clientFsm.Bot.Client.Stop();
            Console.WriteLine("取消ping");
            //退出连接状态不需要发送Ping
        }
    }
}