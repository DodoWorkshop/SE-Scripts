namespace IngameScript
{
    public interface IStateMachine
    {
        void ChangeState(IState newState);

        void SendEvent(IStateEvent stateEvent);
    }
}