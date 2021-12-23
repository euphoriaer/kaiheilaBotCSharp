namespace CsharpBot
{
    public  class StateBase
    {
        public ClientFSM.StateType CurStateType;

        public virtual void OnEnter(string reconnectionInfo)
        {
        }

        public virtual void OnExit()
        {
        }
    }
}