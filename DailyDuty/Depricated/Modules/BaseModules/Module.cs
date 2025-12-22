// using System;
// using System.Diagnostics;
// using System.Globalization;
// using DailyDuty.Classes;
// using DailyDuty.CustomNodes;
// using DailyDuty.Models;
// using Dalamud.Bindings.ImGui;
// using Dalamud.Game.Text;
// using Dalamud.Interface.Utility;
// using Dalamud.Interface.Utility.Raii;
// using KamiLib.Classes;
// using KamiLib.Configuration;
// using KamiLib.Extensions;
//
// namespace DailyDuty.Modules.BaseModules;
//
// public abstract class Module : IDisposable {
//     public abstract ModuleName ModuleName { get; }
//     
//     public abstract ModuleType ModuleType { get; }
//
//     public abstract ModuleConfig GetConfig();
//
//     public abstract ModuleData GetData();
//     
//     public virtual void Dispose() { }
//
//     public virtual bool HasTooltip => false;
//
//     public virtual string TooltipText { get; protected set; } = string.Empty;
//     
//     public virtual bool HasClickableLink => false;
//
//     public virtual PayloadId ClickableLinkPayloadId => PayloadId.Unknown;
//
//     public virtual ModuleStatus ModuleStatus => ModuleStatus.Unknown;
//     
//     public virtual bool IsEnabled => false;
//     public TodoTaskNode? TodoTaskNode;
//
//     public abstract void DrawConfig();
//
//     public abstract void DrawData();
//
//     public abstract void Update();
//
//     public abstract void Load();
//
//     public abstract void Unload();
//
//     public abstract void Reset();
//
//     public abstract void ZoneChange();
//
//     public abstract void SaveConfig();
//
//     public abstract void SaveData();
//     
//     public abstract DateTime GetNextReset();
//
//     public TimeSpan GetTimeRemaining() => GetData().NextReset - DateTime.UtcNow;
//     
//     protected abstract ModuleStatus GetModuleStatus();
//     
//     protected abstract StatusMessage GetStatusMessage();
//
//     protected virtual void UpdateTaskLists() { }
//
//     public virtual bool ShouldReset() => DateTime.UtcNow >= GetData().NextReset;
// }
//
// public abstract class Module<T, TU> : Module where T : ModuleData, new() where TU : ModuleConfig, new() {
//     protected T Data { get; private set; } = new();
//
//     protected TU Config { get; private set; } = new();
//
//     public override bool IsEnabled => Config.ModuleEnabled;
//
//     public override ModuleStatus ModuleStatus => Config.Suppressed ? ModuleStatus.Suppressed : GetModuleStatus();
//     
//     protected XivChatType GetChatChannel() => Config.UseCustomChannel ? Config.MessageChatChannel : Services.PluginInterface.GeneralChatType;
//
//     public override ModuleConfig GetConfig() => Config;
//
//     public override ModuleData GetData() => Data;
//
//     private readonly Stopwatch statusMessageLockout = new();
//
//     protected virtual void UpdateTaskData() { }
//         
//     protected bool DataChanged;
//     protected bool ConfigChanged;
//
//     public override void DrawConfig() {
//         Config.DrawConfigUi(this);
//
//         ConfigChanged |= Config.ConfigChanged;
//     }
//
//     public override void DrawData() {
//         ImGuiTweaks.Header("Current Status");
//         using (var _ = ImRaii.PushIndent()) {
//            DrawModuleCurrentStatusUi();
//         }
//
//         if (Data.NextReset != DateTime.MaxValue) {
//             ImGuiTweaks.Header("Module Data");
//             using var _ = ImRaii.PushIndent();
//             
//             DrawModuleResetDataUi();
//         }
//     
//         Data.DrawDataUi();
//         
//         ImGuiTweaks.Header("Module Suppression");
//         ImGuiHelpers.CenteredText("Silence notification until the next module reset");
//
//         ImGuiTweaks.DisabledButton(!Config.Suppressed ? "Snooze" : "Unsnooze", () => {
//             Config.Suppressed = !Config.Suppressed;
//             ConfigChanged = true;
//         });
//     }
//
//     private void DrawModuleCurrentStatusUi() {
//         using var table = ImRaii.Table("module_data_table", 2, ImGuiTableFlags.SizingStretchSame);
//         if (!table) return;
//
//         ImGui.TableNextColumn();
//         ImGui.TextUnformatted("Current Status");
//
//         ImGui.TableNextColumn();
//         var message = ModuleStatus switch {
//             ModuleStatus.Suppressed => "Suppressed",
//             ModuleStatus.Incomplete => "Incomplete",
//             ModuleStatus.InProgress => "In-Progress",
//             ModuleStatus.Unavailable => "Unavailable",
//             ModuleStatus.Complete => "Complete",
//             ModuleStatus.Unknown => "Unknown",
//             _ => "ERROR, Report this bug.",
//         };
//         
//         ImGui.TextColored(ModuleStatus.GetColor(), message);
//     }
//     
//     private void DrawModuleResetDataUi() {
//         using var table = ImRaii.Table("module_data_table", 2, ImGuiTableFlags.SizingStretchSame);
//         if (!table) return;
//
//         ImGui.TableNextColumn();
//         ImGui.TextUnformatted("Next Reset");
//
//         ImGui.TableNextColumn();
//         ImGui.TextUnformatted(GetData().NextReset.ToLocalTime().ToString(CultureInfo.CurrentCulture));
//
//         var timeRemaining = GetTimeRemaining();
//         if (timeRemaining > TimeSpan.Zero) {
//             ImGui.TableNextColumn();
//             ImGui.TextUnformatted("Time Remaining");
//
//             ImGui.TableNextColumn();
//             ImGui.TextUnformatted($"{timeRemaining.Days}.{timeRemaining.Hours:00}:{timeRemaining.Minutes:00}:{timeRemaining.Seconds:00}");
//         }
//     }
//     
//     public override void Update() {
//         if (DataChanged || ConfigChanged) {
//             UpdateTaskLists();
//             UpdateTaskData();
//             UpdateOverlays();
//         }
//         
//         if (DataChanged) SaveData();
//         if (ConfigChanged) SaveConfig();
//         
//         DataChanged = false;
//         ConfigChanged = false;
//     }
//
//     public override void Load() {
//         Services.Log.Debug($"[{ModuleName}] Loading Module");
//         Data = LoadData();
//         Config = LoadConfig();
//
//         if (DateTime.UtcNow > Data.NextReset) {
//             Reset();
//         }
//         
//         UpdateTaskLists();
//         UpdateTaskData();
//         UpdateOverlays();
//         
//         Update();
//         
//         if (Config is { OnLoginMessage: true, ModuleEnabled: true, Suppressed: false }) {
//             SendStatusMessage();
//         }
//     }
//
//     public override void Unload() {
//         Services.Log.Debug($"[{ModuleName}] Unloading Module");
//         
//         statusMessageLockout.Stop();
//         statusMessageLockout.Reset();
//     }
//
//     public override void Reset() {
//         Services.Log.Debug($"[{ModuleName}] Resetting Module, Next Reset: {GetNextReset().ToLocalTime()}");
//
//         SendResetMessage();
//         
//         Data.NextReset = GetNextReset();
//         DataChanged = true;
//         SaveData();
//         
//         Config.Suppressed = false;
//         ConfigChanged = true;
//         SaveConfig();
//     }
//
//     public override void ZoneChange() {
//         if (Config is { OnZoneChangeMessage: true, ModuleEnabled: true, Suppressed: false }) {
//             SendStatusMessage();
//         }
//     }
//
//     private T LoadData()
//         => Services.PluginInterface.LoadCharacterFile<T>(Services.PlayerState.ContentId, $"{ModuleName}.data.json");
//     
//     private TU LoadConfig()
//         => Services.PluginInterface.LoadCharacterFile<TU>(Services.PlayerState.ContentId, $"{ModuleName}.config.json");
//     
//     public override void SaveConfig() 
//         => Services.PluginInterface.SaveCharacterFile(Services.PlayerState.ContentId, $"{ModuleName}.config.json", Config);
//     
//     public override void SaveData() 
//         => Services.PluginInterface.SaveCharacterFile(Services.PlayerState.ContentId, $"{ModuleName}.data.json", Data);
//
//     private void UpdateOverlays() {
//         System.TodoListController.Refresh();
//     }
//
//     private void SendStatusMessage() {
//         if (GetModuleStatus() is not (ModuleStatus.Incomplete or ModuleStatus.Unknown)) return;
//         if (Services.Condition.IsBoundByDuty()) return;
//         if (statusMessageLockout.Elapsed < TimeSpan.FromMinutes(5) && statusMessageLockout.IsRunning) {
//             Services.Log.Debug($"[{ModuleName}] Suppressing Status Message: {TimeSpan.FromMinutes(5) - statusMessageLockout.Elapsed}");
//             return;
//         }
//         
//         Services.Log.Debug($"[{ModuleName}] Sending Status Message");
//         
//         var statusMessage = GetStatusMessage();
//         if (Config.UseCustomStatusMessage && GetModuleStatus() != ModuleStatus.Unknown) {
//             statusMessage.Message = Config.CustomStatusMessage;
//         }
//         statusMessage.SourceModule = ModuleName;
//         statusMessage.MessageChannel = GetChatChannel();
//         statusMessage.PrintMessage();
//         
//         statusMessageLockout.Restart();
//     }
//     
//     private void SendResetMessage() {
//         if (Config is not { ResetMessage: true, ModuleEnabled: true, Suppressed: false }) return;
//         if (DateTime.UtcNow - Data.NextReset >= TimeSpan.FromMinutes(5)) return;
//         
//         var statusMessage = GetStatusMessage();
//         statusMessage.Message = Config.UseCustomResetMessage ? Config.CustomResetMessage : "Module Reset";
//         statusMessage.SourceModule = ModuleName;
//         statusMessage.MessageChannel = GetChatChannel();
//         statusMessage.PrintMessage();
//     }
//
//     protected TV TryUpdateData<TV>(TV value, TV newValue) where TV : IEquatable<TV> {
//         if (!value.Equals(newValue)) {
//             DataChanged = true;
//             return newValue;
//         }
//
//         return value;
//     }
// }
