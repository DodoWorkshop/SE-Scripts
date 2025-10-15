namespace IngameScript
{
    public class SimpleStateMachine : IStateMachine
    {
        private readonly Program _program;

        public SimpleStateMachine(Program program)
        {
            _program = program;
        }

        public IState CurrentState { get; private set; }

        public void ChangeState(IState newState)
        {
            CurrentState?.OnExit(_program, this);
            CurrentState = newState;
            CurrentState?.OnEnter(_program, this);
        }

        public void SendEvent(IStateEvent stateEvent)
        {
            CurrentState?.OnEvent(_program, this, stateEvent);
        }
    }
}