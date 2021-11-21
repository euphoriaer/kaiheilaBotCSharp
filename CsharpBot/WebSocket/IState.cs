namespace CsharpBot
{
    public interface IState
    {
        void OnEnter(string reconnectionInfo);
        void OnExit();
    }
}