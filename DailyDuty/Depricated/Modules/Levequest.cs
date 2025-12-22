// using System;
// using DailyDuty.Classes;
// using DailyDuty.Enums;
// using DailyDuty.Models;
// using DailyDuty.Modules.BaseModules;
// using Dalamud.Bindings.ImGui;
// using FFXIVClientStructs.FFXIV.Client.Game;
// using KamiLib.Classes;
//
// namespace DailyDuty.Modules;
//
// public class LevequestData : ModuleData {
//     public int NumLevequestAllowances;
//     public int AcceptedLevequests;
//     
//     protected override void DrawModuleData() {
//         DrawDataTable(
//             ("Levequest Allowances", NumLevequestAllowances.ToString()),
//             ("Accepted Levequests", AcceptedLevequests.ToString()));
//     }
// }
//
// public class LevequestConfig : ModuleConfig { 
//     public int NotificationThreshold = 95;
//     public ComparisonMode ComparisonMode = ComparisonMode.EqualTo;
//
//     protected override void DrawModuleConfig() {
//         ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
//         ConfigChanged |= ImGuiTweaks.EnumCombo("Comparison Mode", ref ComparisonMode);
//
//         ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
//         ConfigChanged |= ImGui.SliderInt("Notification Threshold", ref NotificationThreshold, 0, 100);
//     }
// }
//
// public unsafe class Levequest : BaseModules.Modules.Special<LevequestData, LevequestConfig> {
//     public override ModuleName ModuleName => ModuleName.Levequest;
//     
//     public override DateTime GetNextReset() => Time.NextLeveAllowanceReset();
//
//     public override void Update() {
//             Data.NumLevequestAllowances = TryUpdateData(Data.NumLevequestAllowances, QuestManager.Instance()->NumLeveAllowances);
//             Data.AcceptedLevequests = TryUpdateData(Data.AcceptedLevequests, QuestManager.Instance()->NumAcceptedLeveQuests);
//         
//             base.Update();
//         }
//
//     protected override ModuleStatus GetModuleStatus() => Config.ComparisonMode switch {
//         ComparisonMode.LessThan when Config.NotificationThreshold > Data.NumLevequestAllowances => ModuleStatus.Complete,
//         ComparisonMode.EqualTo when Config.NotificationThreshold == Data.NumLevequestAllowances => ModuleStatus.Complete,
//         ComparisonMode.LessThanOrEqual when Config.NotificationThreshold >= Data.NumLevequestAllowances => ModuleStatus.Complete,
//         _ => ModuleStatus.Incomplete,
//     };
//
//     protected override StatusMessage GetStatusMessage()
//         => $"{Data.NumLevequestAllowances} Allowances Remaining";
// }
