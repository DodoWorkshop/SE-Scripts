namespace IngameScript
{
    public class DisplayCycleSettings
    {
        #region mdk preserve

        public static readonly DisplayMode[] CycleOrder =
        {
            DisplayMode.General,
            DisplayMode.Map,
            DisplayMode.Map3D,
            DisplayMode.Database,
            DisplayMode.Detection
        };

        #endregion

        public DisplayMode CurrentMode { get; set; } = DisplayMode.General;
    }
}