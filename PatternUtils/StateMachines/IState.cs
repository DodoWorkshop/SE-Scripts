namespace IngameScript
{
    public interface IState
    {
        void OnEnter(Program program, IStateMachine stateMachine);

        void OnEvent(Program program, IStateMachine stateMachine, IStateEvent stateEvent);

        void OnExit(Program program, IStateMachine stateMachine);
    }
}