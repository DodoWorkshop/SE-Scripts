using System;

namespace IngameScript
{
    public static class TimeUtils
    {
        private const long TicksPerSecond = 10000000L;
        private static long NewEntryThresholdSeconds => Program.NewEntryThresholdSeconds;

        public static string FormatAge(long updateDateTicks, bool shortFormat = false)
        {
            if (updateDateTicks == 0) return "?";
            var ageSeconds = (DateTime.Now.Ticks - updateDateTicks) / TicksPerSecond;
            if (ageSeconds < 0) return "?";
            if (ageSeconds < NewEntryThresholdSeconds) return "NOW";
            var suffix = shortFormat ? "" : " ago";
            if (ageSeconds < 3600) return $"{ageSeconds / 60}m{suffix}";
            return $"{ageSeconds / 3600}h{suffix}";
        }

        public static bool IsNew(long updateDateTicks)
        {
            if (updateDateTicks == 0) return false;
            var ageSeconds = (DateTime.Now.Ticks - updateDateTicks) / TicksPerSecond;
            return ageSeconds >= 0 && ageSeconds < NewEntryThresholdSeconds;
        }
    }
}
