using DailyDuty.Classes;
using DailyDuty.Models;
using FFXIVClientStructs.FFXIV.Client.Game;
using DailyDuty.Localization;
using DailyDuty.Modules.BaseModules;
using ImGuiNET;
using KamiLib.Classes;

namespace DailyDuty.Modules;

public class CustomDeliveryData : ModuleData {
    public int RemainingAllowances = 12;

    protected override void DrawModuleData() {
        DrawDataTable([
            (Strings.AllowancesRemaining, RemainingAllowances.ToString()),
        ]);
    }
}

public class CustomDeliveryConfig : ModuleConfig {
    public int NotificationThreshold = 12;
    public ComparisonMode ComparisonMode = ComparisonMode.LessThan;

    protected override bool DrawModuleConfig() {
        var configChanged = false;

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
        configChanged |= ImGuiTweaks.EnumCombo(Strings.ComparisonMode, ref ComparisonMode);
        
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
        configChanged |= ImGui.SliderInt(Strings.NotificationThreshold, ref NotificationThreshold, 1, 12);

        return configChanged;
    }
}

public unsafe class CustomDelivery : Modules.Weekly<CustomDeliveryData, CustomDeliveryConfig> {
    public override ModuleName ModuleName => ModuleName.CustomDelivery;

    public override void Update() {
        Data.RemainingAllowances = TryUpdateData(Data.RemainingAllowances, SatisfactionSupplyManager.Instance()->GetRemainingAllowances());
        
        base.Update();
    }

    public override void Reset() {
        Data.RemainingAllowances = 12;
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus() => Config.ComparisonMode switch {
        ComparisonMode.LessThan when Config.NotificationThreshold > Data.RemainingAllowances => ModuleStatus.Complete,
        ComparisonMode.EqualTo when Config.NotificationThreshold == Data.RemainingAllowances => ModuleStatus.Complete,
        ComparisonMode.LessThanOrEqual when Config.NotificationThreshold >= Data.RemainingAllowances => ModuleStatus.Complete,
        _ => ModuleStatus.Incomplete,
    };

    protected override StatusMessage GetStatusMessage() => new() {
        Message = $"{Data.RemainingAllowances} {Strings.AllowancesRemaining}",
    };
}