namespace IngameScript
{
    public interface IStateMachine : IContainerItem
    {
        void ChangeState(IState newState);

        void SendEvent(IStateEvent stateEvent);
    }
}