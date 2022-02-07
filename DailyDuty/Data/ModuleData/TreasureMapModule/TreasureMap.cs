using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Data.ModuleData.TreasureMapModule;

internal class TreasureMap
{
    public Dictionary<HarvestType, List<uint>> HarvestData { get; init; } = new();
    public uint ItemID { get; init; }
    public int Level { get; init; }
}