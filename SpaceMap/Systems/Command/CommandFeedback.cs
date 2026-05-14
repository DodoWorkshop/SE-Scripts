namespace IngameScript
{
    public class CommandFeedback
    {
        private const int DisplayDuration = 30;

        public string Message { get; private set; }
        public bool IsError { get; private set; }
        private int _ticksRemaining;

        public bool HasMessage => _ticksRemaining > 0;

        public void SetSuccess(string message)
        {
            Message = message;
            IsError = false;
            _ticksRemaining = DisplayDuration;
        }

        public void SetError(string message)
        {
            Message = message;
            IsError = true;
            _ticksRemaining = DisplayDuration;
        }

        public void Tick()
        {
            if (_ticksRemaining > 0) _ticksRemaining--;
        }
    }
}
