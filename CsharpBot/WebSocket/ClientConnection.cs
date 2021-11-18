namespace CsharpBot
{
    public class ClientConnection : IState
    {
        private ClientFSM _clientFsm;
        public ClientConnection(ClientFSM fsm)
        {
            _clientFsm = fsm;
        }
        void IState.OnEnter()
        {
            throw new System.NotImplementedException();
        }

        void IState.OnExit()
        {
            throw new System.NotImplementedException();
        }
    }
}