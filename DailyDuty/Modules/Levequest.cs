using System;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using KamiLib.Classes;

namespace DailyDuty.Modules;

public class LevequestData : ModuleData {
    public int NumLevequestAllowances;
    public int AcceptedLevequests;
    
    protected override void DrawModuleData() {
        DrawDataTable(
            (Strings.LevequestAllowances, NumLevequestAllowances.ToString()),
            (Strings.AcceptedLevequests, AcceptedLevequests.ToString()));
    }
}

public class LevequestConfig : ModuleConfig { 
    public int NotificationThreshold = 95;
    public ComparisonMode ComparisonMode = ComparisonMode.EqualTo;

    protected override void DrawModuleConfig() {
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
        ConfigChanged |= ImGuiTweaks.EnumCombo(Strings.ComparisonMode, ref ComparisonMode);

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
        ConfigChanged |= ImGui.SliderInt(Strings.NotificationThreshold, ref NotificationThreshold, 0, 100);
    }
}

public unsafe class Levequest : BaseModules.Modules.Special<LevequestData, LevequestConfig> {
    public override ModuleName ModuleName => ModuleName.Levequest;
    
    public override DateTime GetNextReset() => Time.NextLeveAllowanceReset();

    public override void Update() {
            Data.NumLevequestAllowances = TryUpdateData(Data.NumLevequestAllowances, QuestManager.Instance()->NumLeveAllowances);
            Data.AcceptedLevequests = TryUpdateData(Data.AcceptedLevequests, QuestManager.Instance()->NumAcceptedLeveQuests);
        
            base.Update();
        }

    protected override ModuleStatus GetModuleStatus() => Config.ComparisonMode switch {
        ComparisonMode.LessThan when Config.NotificationThreshold > Data.NumLevequestAllowances => ModuleStatus.Complete,
        ComparisonMode.EqualTo when Config.NotificationThreshold == Data.NumLevequestAllowances => ModuleStatus.Complete,
        ComparisonMode.LessThanOrEqual when Config.NotificationThreshold >= Data.NumLevequestAllowances => ModuleStatus.Complete,
        _ => ModuleStatus.Incomplete,
    };

    protected override StatusMessage GetStatusMessage()
        => $"{Data.NumLevequestAllowances} {Strings.AllowancesRemaining}";
}