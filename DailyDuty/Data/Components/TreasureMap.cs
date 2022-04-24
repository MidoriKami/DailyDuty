using System.Collections.Generic;

namespace DailyDuty.Data.Components
{
    internal class TreasureMap
    {
        public Dictionary<HarvestType, List<uint>> HarvestData { get; init; } = new();
        public uint ItemID { get; init; }
        public int Level { get; init; }
    }
}