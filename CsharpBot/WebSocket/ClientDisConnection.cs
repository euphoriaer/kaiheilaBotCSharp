using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CsharpBot
{
    public class ClientDisConnection : IState
    {
        private ClientFSM _clientFsm;
        private CancellationTokenSource cts;
        private Task _reConnect;

        public ClientDisConnection(ClientFSM fsm)
        {
            _clientFsm = fsm;
        }

        void IState.OnEnter(string info)
        {
            _clientFsm.Bot.log.Record("客户端：服务器断开: " + info);
            Console.WriteLine("客户端：服务器断开: " + info);
            Console.WriteLine("进入重连");
            if (cts != null)
            {
                cts.Cancel();
            }
            cts = new CancellationTokenSource();
            //暂按全部断开重连处理，因为不需要中间的离线消息
            _reConnect = Task.Run((() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    try
                    {
                        StopConnect();
                        Thread.Sleep(10000); //每隔10秒尝试重连一次
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }), cts.Token);

            return;
            //todo 每隔36秒 没有收到一次 pong包，视为连接超时,再发两次ping 2 4，

            //todo 还没有收到，Gateway一次，主动重连Resume 2次，接收离线消息
            Thread.Sleep(8000);//8秒
            Resume();
            Thread.Sleep(16000);//16秒
            Resume();
        }

        void IState.OnExit()
        {
            if (cts == null)
            {
                return;
            }
            cts.Cancel();
            Console.WriteLine("退出重连");
            //连接成功退出重连模式
        }

        /// <summary>
        /// 主动重连
        /// </summary>
        internal void Resume()
        {
            JObject pingJobj = new JObject();
            pingJobj.Add("s", 4);
            pingJobj.Add("sn", _clientFsm.Bot.LastSn);
            string pingJson = JsonConvert.SerializeObject(pingJobj);

            _clientFsm.Bot.log.Record("客户端：发送Resume" + pingJson);
            _clientFsm.Bot.Client.WebsocketClient.Send(pingJson);
        }

        /// <summary>
        /// 断开重连
        /// </summary>
        internal void StopConnect()
        {
            _clientFsm.Bot.log.Record("尝试重新连接");
            Console.WriteLine("尝试重新连接");
            _clientFsm.Bot.Client.Stop();
            _clientFsm.Bot.Run();
        }
    }
}