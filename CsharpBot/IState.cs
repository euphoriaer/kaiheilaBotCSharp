namespace CsharpBot
{
    public interface IState
    {
        void OnEnter();
        void OnExit();
    }
}