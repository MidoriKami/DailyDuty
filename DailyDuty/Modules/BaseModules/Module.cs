using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using Dalamud.Game.Text;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.Configuration;
using KamiLib.Extensions;

namespace DailyDuty.Modules.BaseModules;

public abstract class Module : IDisposable {
    public abstract ModuleName ModuleName { get; }
    
    public abstract ModuleType ModuleType { get; }

    public abstract ModuleConfig GetConfig();

    public abstract ModuleData GetData();
    
    public virtual void Dispose() { }

    public virtual bool HasTooltip => false;

    public virtual string TooltipText { get; protected set; } = string.Empty;
    
    public virtual bool HasClickableLink => false;

    public virtual PayloadId ClickableLinkPayloadId => PayloadId.Unknown;

    public virtual ModuleStatus ModuleStatus => ModuleStatus.Unknown;
    
    public virtual bool IsEnabled => false;

    public abstract void DrawConfig();

    public abstract void DrawData();

    public abstract void Update();

    public abstract void Load();

    public abstract void Unload();

    public abstract void Reset();

    public abstract void ZoneChange();

    public abstract void SaveConfig();

    public abstract void SaveData();
    
    public abstract DateTime GetNextReset();

    public abstract TimeSpan GetModulePeriod();
    
    public TimeSpan GetTimeRemaining() => GetNextReset() - DateTime.UtcNow;
    
    protected abstract ModuleStatus GetModuleStatus();
    
    protected abstract StatusMessage GetStatusMessage();

    protected virtual void UpdateTaskLists() { }

    public abstract TimerConfig GetTimerConfig();
    
    public virtual bool ShouldReset() => DateTime.UtcNow >= GetData().NextReset;
}

public abstract class Module<T, TU> : Module where T : ModuleData, new() where TU : ModuleConfig, new() {
    protected T Data { get; private set; } = new();

    protected TU Config { get; private set; } = new();

    public override TimerConfig GetTimerConfig() => Config.TimerConfig;

    public override bool IsEnabled => Config.ModuleEnabled;

    public override ModuleStatus ModuleStatus => Config.Suppressed ? ModuleStatus.Suppressed : GetModuleStatus();
    
    protected XivChatType GetChatChannel() => Config.UseCustomChannel ? Config.MessageChatChannel : Service.PluginInterface.GeneralChatType;

    public override ModuleConfig GetConfig() => Config;

    public override ModuleData GetData() => Data;

    private readonly Stopwatch statusMessageLockout = new();

    protected virtual void UpdateTaskData() { }
        
    protected bool DataChanged;
    protected bool ConfigChanged;

    public override void DrawConfig() {
        if (Config.DrawConfigUi()) {
            ConfigChanged = true;
            SaveConfig();
        }
    }

    public override void DrawData() {
        ImGui.TextUnformatted(Strings.CurrentStatus);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        using (var _ = ImRaii.PushIndent()) {
           DrawModuleCurrentStatusUi();
        } 

        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted(Strings.ModuleData);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        using (var _ = ImRaii.PushIndent()) {
            DrawModuleResetDataUi();
        } 
    
        Data.DrawDataUi();
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted(Strings.ModuleSuppression);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        ImGuiHelpers.CenteredText(Strings.ModuleSuppressionHelp);

        using (ImRaii.Disabled(!(ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl))) {
            if (ImGui.Button(Strings.Snooze, new Vector2(ImGui.GetContentRegionAvail().X, 23.0f * ImGuiHelpers.GlobalScale))) {
                Config.Suppressed = true;
                ConfigChanged = true;
            }

            using (ImRaii.PushStyle(ImGuiStyleVar.Alpha, 1.0f)) {
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) {
                    ImGui.SetTooltip("Hold Shift + Control while clicking activate button");
                }
            }
        }
    }

    private void DrawModuleCurrentStatusUi() {
        using var table = ImRaii.Table("module_data_table", 2, ImGuiTableFlags.SizingStretchSame);
        if (!table) return;

        ImGui.TableNextColumn();
        ImGui.TextUnformatted(Strings.CurrentStatus);

        ImGui.TableNextColumn();
        var message = ModuleStatus switch {
            ModuleStatus.Suppressed => Strings.Suppressed,
            ModuleStatus.Incomplete => Strings.Incomplete,
            ModuleStatus.InProgress => Strings.InProgress,
            ModuleStatus.Unavailable => Strings.Unavailable,
            ModuleStatus.Complete => Strings.Complete,
            ModuleStatus.Unknown => Strings.Unknown,
            _ => "ERROR, Report this bug.",
        };
        
        ImGui.TextColored(ModuleStatus.GetColor(), message);
    }
    
    private void DrawModuleResetDataUi() {
        using var table = ImRaii.Table("module_data_table", 2, ImGuiTableFlags.SizingStretchSame);
        if (!table) return;

        ImGui.TableNextColumn();
        ImGui.TextUnformatted(Strings.NextReset);

        ImGui.TableNextColumn();
        ImGui.TextUnformatted(GetNextReset().ToLocalTime().ToString(CultureInfo.CurrentCulture));

        var timeRemaining = GetTimeRemaining();
        if (timeRemaining > TimeSpan.Zero) {
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(Strings.TimeRemaining);

            ImGui.TableNextColumn();
            ImGui.TextUnformatted($"{timeRemaining.Days}.{timeRemaining.Hours:00}:{timeRemaining.Minutes:00}:{timeRemaining.Seconds:00}");
        }
    }
    
    public override void Update() {
        if (DataChanged || ConfigChanged) {
            UpdateTaskLists();
            UpdateTaskData();
            UpdateOverlays();
        }
        
        if (DataChanged) SaveData();
        if (ConfigChanged) SaveConfig();
        
        DataChanged = false;
        ConfigChanged = false;
    }

    public override void Load() {
        Service.Log.Debug($"[{ModuleName}] Loading Module");
        Data = LoadData();
        Config = LoadConfig();

        if (DateTime.UtcNow > Data.NextReset) {
            Reset();
        }
        
        UpdateTaskLists();
        UpdateTaskData();
        UpdateOverlays();
        
        Update();
        
        if (Config is { OnLoginMessage: true, ModuleEnabled: true, Suppressed: false }) {
            SendStatusMessage();
        }
    }

    public override void Unload() {
        Service.Log.Debug($"[{ModuleName}] Unloading Module");
        
        statusMessageLockout.Stop();
        statusMessageLockout.Reset();
    }

    public override void Reset() {
        Service.Log.Debug($"[{ModuleName}] Resetting Module, Next Reset: {GetNextReset().ToLocalTime()}");

        SendResetMessage();
        
        Data.NextReset = GetNextReset();
        DataChanged = true;
        SaveData();
        
        Config.Suppressed = false;
        ConfigChanged = true;
        SaveConfig();
    }

    public override void ZoneChange() {
        if (Config is { OnZoneChangeMessage: true, ModuleEnabled: true, Suppressed: false }) {
            SendStatusMessage();
        }
    }

    private T LoadData()
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, $"{ModuleName}.data.json", () => new T());
    
    private TU LoadConfig()
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, $"{ModuleName}.config.json", () => new TU());
    
    public override void SaveConfig() 
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, $"{ModuleName}.config.json", Config);
    
    public override void SaveData() 
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, $"{ModuleName}.data.json", Data);

    private void UpdateOverlays() {
        System.TodoListController.Refresh();
        System.TimersController.Refresh();
    }

    private void SendStatusMessage() {
        if (GetModuleStatus() is not (ModuleStatus.Incomplete or ModuleStatus.Unknown)) return;
        if (Service.Condition.IsBoundByDuty()) return;
        if (statusMessageLockout.Elapsed < TimeSpan.FromMinutes(5) && statusMessageLockout.IsRunning) {
            Service.Log.Debug($"[{ModuleName}] Suppressing Status Message: {TimeSpan.FromMinutes(5) - statusMessageLockout.Elapsed}");
            return;
        }
        
        Service.Log.Debug($"[{ModuleName}] Sending Status Message");
        
        var statusMessage = GetStatusMessage();
        if (Config.UseCustomStatusMessage && GetModuleStatus() != ModuleStatus.Unknown) {
            statusMessage.Message = Config.CustomStatusMessage;
        }
        statusMessage.SourceModule = ModuleName;
        statusMessage.MessageChannel = GetChatChannel();
        statusMessage.PrintMessage();
        
        statusMessageLockout.Restart();
    }
    
    private void SendResetMessage() {
        if (Config is not { ResetMessage: true, ModuleEnabled: true, Suppressed: false }) return;
        if (DateTime.UtcNow - Data.NextReset >= TimeSpan.FromMinutes(5)) return;
        
        var statusMessage = GetStatusMessage();
        statusMessage.Message = Config.UseCustomResetMessage ? Config.CustomResetMessage : Strings.ModuleReset;
        statusMessage.SourceModule = ModuleName;
        statusMessage.MessageChannel = GetChatChannel();
        statusMessage.PrintMessage();
    }

    protected TV TryUpdateData<TV>(TV value, TV newValue) where TV : IEquatable<TV> {
        if (!value.Equals(newValue)) {
            DataChanged = true;
            return newValue;
        }

        return value;
    }
}