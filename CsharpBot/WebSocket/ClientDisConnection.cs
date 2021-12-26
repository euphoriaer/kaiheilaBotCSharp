using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CsharpBot
{
    public class ClientDisConnection : StateBase
    {
        private ClientFSM _clientFsm;
        private Task _reConnect;

        private ManualResetEvent singelResetEvent;

        public ClientDisConnection(ClientFSM fsm)
        {
            singelResetEvent = new ManualResetEvent(false);
            CurStateType = ClientFSM.StateType.Disconnection;
            _clientFsm = fsm;
            _reConnect = new Task((() =>
            {
                while (true)
                {
                    singelResetEvent.WaitOne();
                    StopConnect();
                    Thread.Sleep(10000); //每隔10秒尝试重连一次
                }
            }));
            _reConnect.Start();
        }

        public override void OnEnter(string info)
        {
            _clientFsm.Bot.log.Record("客户端：服务器断开: " + info);
            Console.WriteLine("客户端：服务器断开: " + info);
            Console.WriteLine("进入重连");// error 第二次进入重连失败
            bool isOK = singelResetEvent.Set();
            if (!isOK)
            {
                Console.WriteLine("ERROR !! Stop Singel Fail --------");
                //定义自己的 错误
                try
                {
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            //暂按全部断开重连处理，因为不需要中间的离线消息
        }

        public override void OnExit()
        {
            Console.WriteLine("重连成功,退出重连");
            singelResetEvent.Reset();
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