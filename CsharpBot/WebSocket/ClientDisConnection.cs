using System;
using System.Threading;
using System.Threading.Tasks;

namespace CsharpBot
{
    public class ClientDisConnection : StateBase
    {
        private ClientFSM _clientFsm;
        private bool _rebort=false;
        public ClientDisConnection(ClientFSM fsm)
        {
          
            CurStateType = ClientFSM.StateType.Disconnection;
            _clientFsm = fsm;
        }

        public override void OnEnter(string info)
        {
            _clientFsm.Bot.log.Record("客户端：服务器断开: " + info);
            Console.WriteLine("客户端：服务器断开: " + info);
            Console.WriteLine("进入重连");// error 第二次进入重连失败
            _rebort = true;
            while (_rebort)
            {
                StopConnect();
                Thread.Sleep(2000); //每隔2秒尝试重连一次
            }
            //暂按全部断开重连处理，因为不需要中间的离线消息
        }

        public override void OnExit()
        {
            Console.WriteLine("重连成功,退出重连");
            _rebort = false;
            //连接成功退出重连模式
        }

        /// <summary>
        /// 断开重连
        /// </summary>
        internal void StopConnect()
        {
            _clientFsm.Bot.log.Record("尝试重新连接");
            Console.WriteLine("尝试重新连接");
            bool isOK = _clientFsm.Bot.Run();
            if (!isOK)
            {
                Console.WriteLine("重连失败");
            }
        }
    }
}