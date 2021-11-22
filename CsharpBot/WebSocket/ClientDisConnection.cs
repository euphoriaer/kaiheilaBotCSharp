using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CsharpBot
{
    public class ClientDisConnection : IState
    {
        private ClientFSM _clientFsm;
        public ClientDisConnection(ClientFSM fsm)
        {
            _clientFsm = fsm;
        }
        void IState.OnEnter(string info)
        {
            Console.WriteLine("客户端：服务器断开: " + info);
            //error 需要尝试主动重连
        }

        void IState.OnExit()
        {
            
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

            Console.WriteLine("客户端：发送Resume" + pingJson);
            _clientFsm.Bot.Client.WebsocketClient.Send(pingJson);
        }

    }
}