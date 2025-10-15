using System.Collections.Generic;

namespace IngameScript
{
    public class DisplayRepository : IRepository
    {
        public List<HistoryDisplay> History { get; set; } = new List<HistoryDisplay>();

        public int HistoryPage { get; set; }
    }
}