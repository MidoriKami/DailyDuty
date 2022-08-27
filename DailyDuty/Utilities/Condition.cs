using Dalamud.Game.ClientState.Conditions;

namespace DailyDuty.Utilities;

internal static class Condition
{
    public static bool IsBoundByDuty()
    {
        var baseBoundByDuty = Service.Condition[ConditionFlag.BoundByDuty];
        var boundBy56 = Service.Condition[ConditionFlag.BoundByDuty56];
        var boundBy95 = Service.Condition[ConditionFlag.BoundByDuty95];

        if (Service.ClientState.TerritoryType == 1055)
            return false;

        return baseBoundByDuty || boundBy56 || boundBy95;
    }

    public static bool InCutsceneOrQuestEvent()
    {
        return Service.Condition[ConditionFlag.OccupiedInCutSceneEvent] ||
               Service.Condition[ConditionFlag.WatchingCutscene] ||
               Service.Condition[ConditionFlag.WatchingCutscene78] ||
               Service.Condition[ConditionFlag.OccupiedInQuestEvent];
    }
}