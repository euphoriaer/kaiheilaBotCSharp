using System.Collections.Generic;

namespace CsharpBot
{
    public class ClientFSM
    {
        private StateBase _currentStateBase;
        private ClientDisConnection _clientDisConnection;
        private ClientConnection _clientConnection;

        internal Bot Bot;
        private Dictionary<StateType, StateBase> states = new Dictionary<StateType, StateBase>();

        public enum StateType
        {
            Connection, Disconnection
        }

        public ClientFSM(Bot bot)
        {
            this.Bot = bot;
            // 初始状态对象
            _clientDisConnection = new ClientDisConnection(this);
            _clientConnection = new ClientConnection(this);
            states.Add(StateType.Disconnection, _clientDisConnection);
            states.Add(StateType.Connection, _clientConnection);
            _currentStateBase = _clientDisConnection;//初始状态未连接
        }

        public void TransitionState(StateType type, string reconnectionInfo)
        {
            
            if (_currentStateBase.CurStateType == type)
            {
                //已经处于当前状态，避免重复进入
                return;
            }

            if (_currentStateBase != null)
            {
                _currentStateBase.OnExit();
            }
            else
            {
                Bot.log.Record("对象是空的");
            }
            _currentStateBase = states[type];
            _currentStateBase.OnEnter(reconnectionInfo);
        }
    }
}