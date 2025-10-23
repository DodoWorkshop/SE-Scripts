namespace IngameScript
{
    public class ScriptConfig
    {
        public readonly MyRegex IdentificationRegex
            = new MyRegex(@"\[SI:(.+)\]");

        public const int HistoryLength = 100;

        public const string HistoryPanelTag = "[SI-History]";
    }
}