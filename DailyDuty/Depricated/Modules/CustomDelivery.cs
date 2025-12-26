// using DailyDuty.Classes;
// using DailyDuty.Enums;
// using DailyDuty.Models;
// using FFXIVClientStructs.FFXIV.Client.Game;
// using DailyDuty.Modules.BaseModules;
// using Dalamud.Bindings.ImGui;
// using KamiLib.Classes;
//
// namespace DailyDuty.Modules;
//
// public class CustomDeliveryData : ModuleData {
//     public int RemainingAllowances = 12;
//
//     protected override void DrawModuleData() {
//         DrawDataTable(("Allowances Remaining", RemainingAllowances.ToString()));
//     }
// }
//
// public class CustomDeliveryConfig : ModuleConfig {
//     public int NotificationThreshold = 12;
//     public ComparisonMode ComparisonMode = ComparisonMode.Below;
//
//     protected override void DrawModuleConfig() {
//         ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
//         SavePending |= ImGuiTweaks.EnumCombo("Comparison Mode", ref ComparisonMode);
//         
//         ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
//         SavePending |= ImGui.SliderInt("Notification Threshold", ref NotificationThreshold, 1, 12);
//     }
// }
//
// public unsafe class CustomDelivery : BaseModules.Modules.Weekly<CustomDeliveryData, CustomDeliveryConfig> {
//     public override ModuleName ModuleName => ModuleName.CustomDelivery;
//
//     public override void Update() {
//         Data.RemainingAllowances = TryUpdateData(Data.RemainingAllowances, SatisfactionSupplyManager.Instance()->GetRemainingAllowances());
//         
//         base.Update();
//     }
//
//     public override void Reset() {
//         Data.RemainingAllowances = 12;
//         
//         base.Reset();
//     }
//
//     protected override ModuleStatus GetModuleStatus() => Config.ComparisonMode switch {
//         ComparisonMode.Below when Config.NotificationThreshold > Data.RemainingAllowances => ModuleStatus.Complete,
//         ComparisonMode.Equal when Config.NotificationThreshold == Data.RemainingAllowances => ModuleStatus.Complete,
//         ComparisonMode.Above when Config.NotificationThreshold >= Data.RemainingAllowances => ModuleStatus.Complete,
//         _ => ModuleStatus.Incomplete,
//     };
//
//     protected override StatusMessage GetStatusMessage() 
//         => $"{Data.RemainingAllowances} Allowances Remaining";
// }
