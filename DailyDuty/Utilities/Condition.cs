using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.GeneratedSheets;
using ObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;

namespace DailyDuty.Utilities;

internal static class Condition
{
    private static readonly string ExitNodeText;

    public static bool IsBoundByDuty()
    {
        var baseBoundByDuty = Service.Condition[ConditionFlag.BoundByDuty];
        var boundBy56 = Service.Condition[ConditionFlag.BoundByDuty56];
        var boundBy95 = Service.Condition[ConditionFlag.BoundByDuty95];

        // Triggers when Queue is started
        //var boundBy97 = Service.Condition[ConditionFlag.BoundToDuty97];

        return baseBoundByDuty || boundBy56 || boundBy95;
    }

    public static bool IsDutyCompleted()
    {
        var exitNodeText = Service.DataManager.GetExcelSheet<EObjName>()!
            .GetRow(2000139)!
            .Singular;

        return Service.ObjectTable
            .Where(o => o.ObjectKind == ObjectKind.EventObj)
            .Where(IsTargetable)
            .Any(o =>  ExitNodeText == o.Name.ToString().ToLower() );
    }

    private static unsafe bool IsTargetable(Dalamud.Game.ClientState.Objects.Types.GameObject gameObject)
    {
        var playerTargetable = ((GameObject*)gameObject.Address)->GetIsTargetable();

        return playerTargetable;
    }
}