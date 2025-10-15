namespace IngameScript
{
    public class ScriptConfig : IContainerItem
    {
        public readonly System.Text.RegularExpressions.Regex IdentificationRegex
            = new System.Text.RegularExpressions.Regex(@"\[SI:(.+)\]");

        public const int HistoryLength = 100;

        public const string HistoryPanelTag = "[SI-History]";
    }
}