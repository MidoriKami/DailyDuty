using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.MJI;

namespace DailyDuty.Extensions;

public static unsafe class ConditionExtensions {
    extension(ICondition condition) {
        public bool IsBoundByDuty => condition.Any(ConditionFlag.BoundByDuty, ConditionFlag.BoundByDuty56, ConditionFlag.BoundByDuty95);
        public bool IsInCombat => condition.Any(ConditionFlag.InCombat);
        public bool IsInCutscene => condition.Any(ConditionFlag.OccupiedInCutSceneEvent, ConditionFlag.WatchingCutscene, ConditionFlag.WatchingCutscene78);
        public bool IsBetweenAreas => condition.Any(ConditionFlag.BetweenAreas, ConditionFlag.BetweenAreas51);
        public bool IsCrafting => condition.Any(ConditionFlag.Crafting, ConditionFlag.ExecutingCraftingAction, ConditionFlag.PreparingToCraft);
        public bool IsCrossWorld => condition.Any(ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance);
        public bool IsGathering => condition.Any(ConditionFlag.Gathering, ConditionFlag.ExecutingGatheringAction);
        public bool IsInBardPerformance => condition.Any(ConditionFlag.Performing);
        public bool IsInQuestEvent => condition.Any(ConditionFlag.OccupiedInQuestEvent) || IsIslandDoingSomethingMode;
        public bool IsInCutsceneOrQuestEvent => condition.IsInCutscene || condition.IsInQuestEvent;
        public bool IsDutyRecorderPlayback => condition.Any(ConditionFlag.DutyRecorderPlayback);
    }

    public static bool IsIslandDoingSomethingMode => MJIManager.Instance()->CurrentMode is not 0 && MJIManager.Instance()->IsPlayerInSanctuary;
}
