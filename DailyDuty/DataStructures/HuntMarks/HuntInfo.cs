using System.Linq;
using Dalamud.Utility;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.DataStructures.HuntMarks;

public class HuntInfo
{
    public HuntInfo(HuntMarkType type, byte huntID, bool elite)
    {
        var indexOffset = LuminaCache<MobHuntOrderType>.Instance.GetRow((uint)type)!.OrderStart.Row;
        var targetRow = indexOffset + huntID - 1;

        if (huntID == 0) return;

        if (elite)
        {
            Raw[0] = LuminaCache<MobHuntOrder>.Instance.GetRow(targetRow, 0);
        }
        else
        {
            foreach (var index in Enumerable.Range(0, 5))
            {
                Raw[index] = LuminaCache<MobHuntOrder>.Instance.GetRow(targetRow, (uint)index);
            }
        }
    }
    
    private MobHuntOrder?[] Raw { get; } = new MobHuntOrder[5];
    
    public MobHuntOrder? this[int i] => Raw[i];
}

public static class MobHuntOrderExtensions
{
    public static string GetTargetName(this MobHuntOrder hunt)
    {
        return hunt.Target.Value?.Name.Value?.Singular.ToDalamudString().TextValue ?? "Unable to Read Name";
    }
}