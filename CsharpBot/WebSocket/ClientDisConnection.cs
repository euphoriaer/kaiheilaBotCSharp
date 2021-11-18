namespace CsharpBot
{
    public class ClientDisConnection : IState
    {
        private ClientFSM _clientFsm;
        public ClientDisConnection(ClientFSM fsm)
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