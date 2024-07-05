using System;
using System.Drawing;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using KamiLib.Classes;

namespace DailyDuty.Modules;

public class LevequestData : ModuleData {
    public int NumLevequestAllowances;
    public int AcceptedLevequests;
    
    protected override void DrawModuleData() {
        DrawDataTable([
            (Strings.LevequestAllowances, NumLevequestAllowances.ToString()),
            (Strings.AcceptedLevequests, AcceptedLevequests.ToString()),
        ]);
        
        ImGuiHelpers.ScaledDummy(5.0f);
        
        if (AcceptedLevequests is -1) {
            ImGui.TextColored(KnownColor.Orange.Vector(), "Accepted Levequests is currently unavailable.\nAn update will be release later to re-enable this feature.");
        }
    }
}

public class LevequestConfig : ModuleConfig { 
    public int NotificationThreshold = 95;
    public ComparisonMode ComparisonMode = ComparisonMode.EqualTo;

    protected override bool DrawModuleConfig() {
        var configChanged = false;

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
        configChanged |= ImGuiTweaks.EnumCombo(Strings.ComparisonMode, ref ComparisonMode, Strings.ResourceManager);

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
        configChanged |= ImGui.SliderInt(Strings.NotificationThreshold, ref NotificationThreshold, 0, 100);

        return configChanged;
    }
}

public unsafe class Levequest : Modules.Special<LevequestData, LevequestConfig> {
    public override ModuleName ModuleName => ModuleName.Levequest;
    
    public override DateTime GetNextReset() => Time.NextLeveAllowanceReset();

    public override TimeSpan GetModulePeriod() => TimeSpan.FromHours(12);

    public override void Update() {
            Data.NumLevequestAllowances = TryUpdateData(Data.NumLevequestAllowances, QuestManager.Instance()->NumLeveAllowances);
            Data.AcceptedLevequests = -1; //TryUpdateData(Data.AcceptedLevequests, QuestManager.Instance()->NumAcceptedLeveQuests);
        
            base.Update();
        }

    protected override ModuleStatus GetModuleStatus() => Config.ComparisonMode switch {
        ComparisonMode.LessThan when Config.NotificationThreshold > Data.NumLevequestAllowances => ModuleStatus.Complete,
        ComparisonMode.EqualTo when Config.NotificationThreshold == Data.NumLevequestAllowances => ModuleStatus.Complete,
        ComparisonMode.LessThanOrEqual when Config.NotificationThreshold >= Data.NumLevequestAllowances => ModuleStatus.Complete,
        _ => ModuleStatus.Incomplete,
    };

    protected override StatusMessage GetStatusMessage() => new() {
        Message = $"{Data.NumLevequestAllowances} {Strings.AllowancesRemaining}",
    };
}