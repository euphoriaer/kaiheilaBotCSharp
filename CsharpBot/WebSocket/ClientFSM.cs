using System;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Cmp;
using Websocket.Client.Models;

namespace CsharpBot
{
    public class ClientFSM
    {
        private IState _currentState;
        private ClientDisConnection _clientDisConnection;
        private ClientConnection _clientConnection;

        internal Bot bot;
        private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();
        public enum StateType
        {
            Connection, Disconnection
        }

        public ClientFSM(Bot bot)
        {
            this.bot = bot;
            // 初始状态对象
            _clientDisConnection = new ClientDisConnection(this);
            _clientConnection = new ClientConnection(this);
            states.Add(StateType.Disconnection,_clientDisConnection);
            states.Add(StateType.Connection,_clientConnection);
            _currentState = _clientDisConnection;//初始状态未连接
        }

        public void TransitionState(StateType type, string reconnectionInfo)
        {
            if (_currentState != null)
            {
                _currentState.OnExit();
            }
            else
            {
                Console.WriteLine("对象是空的");
            }
            _currentState = states[type];
            _currentState.OnEnter(reconnectionInfo);
        }
    }
}