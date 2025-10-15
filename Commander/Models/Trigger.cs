using System.Collections.Generic;

namespace IngameScript
{
    public class Trigger
    {
        public long Id { get; }
        
        public TriggerType Type { get; }
        
        private Dictionary<string, string> Parameters { get; }
        
        public string CommandLine { get; }
    }
}