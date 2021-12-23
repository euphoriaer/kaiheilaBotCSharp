using System;
using System.Threading;
using System.Threading.Tasks;

namespace CsharpBot
{
    public class ClientDisConnection : StateBase
    {
        private ClientFSM _clientFsm;
        private CancellationTokenSource cts;
        private Task _reConnect;

        public ClientDisConnection(ClientFSM fsm)
        {
            CurStateType = ClientFSM.StateType.Disconnection;
            _clientFsm = fsm;
        }

        public override void OnEnter(string info)
        {
            _clientFsm.Bot.log.Record("客户端：服务器断开: " + info);
            Console.WriteLine("客户端：服务器断开: " + info);
            Console.WriteLine("进入重连");
            if (cts != null)
            {
                cts.Dispose();
            }
            cts = new CancellationTokenSource();
            //暂按全部断开重连处理，因为不需要中间的离线消息
            _reConnect = Task.Run((() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    StopConnect();
                    Thread.Sleep(10000); //每隔10秒尝试重连一次
                }
            }), cts.Token);
        }

        public override void OnExit()
        {
            Console.WriteLine("重连成功,退出重连");
            if (cts == null)
            {
                return;
            }
            cts.Cancel();
            _reConnect.Wait();
            _reConnect.Dispose();
            cts.Dispose();
            //连接成功退出重连模式
        }

        /// <summary>
        /// 断开重连
        /// </summary>
        internal void StopConnect()
        {
            try
            {
                _clientFsm.Bot.log.Record("尝试重新连接");
                Console.WriteLine("尝试重新连接");
                _clientFsm?.Bot?.Client?.Stop();
                bool isOK = _clientFsm.Bot.Run();
                if (!isOK)
                {
                    Console.WriteLine("重连失败");
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}