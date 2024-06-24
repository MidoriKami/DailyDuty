using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.Components;
using KamiLib.Configuration;
using KamiLib.Extensions;

namespace DailyDuty.Modules.BaseModules;

public abstract class ModuleDataBase {
    public DateTime NextReset;

    protected virtual void DrawModuleData() {
        ImGui.TextColored(KnownColor.Orange.Vector(), "No additional data for this module");
    }
    
    public void DrawDataUi() {
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted(Strings.ModuleData);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);

        using var _ = ImRaii.PushIndent();
        DrawModuleData();
    }

    protected void DrawDataTable(params (string Label, string Data)[] tableData) {
        using var table = ImRaii.Table("module_data_table", 2, ImGuiTableFlags.SizingStretchSame);
        if (!table) return;

        foreach (var (label, data) in tableData) {
            ImGuiTweaks.TableRow(label, data);
        }
    }
}

public abstract class ModuleConfigBase {
    public bool ModuleEnabled = false;
    
    public bool OnLoginMessage = true;
    public bool OnZoneChangeMessage = true;
    public bool ResetMessage = false;
    
    // todo: do todo stuff
    // public bool TodoEnabled = true;
    // public bool UseCustomTodoLabel= false;
    // public string CustomTodoLabel = string.Empty;
    // public bool OverrideTextColor = false;
    // public Vector4 TodoTextColor = new(1.0f, 1.0f, 1.0f, 1.0f);
    // public Vector4 TodoTextOutline = new(0.0f, 0.0f, 0.0f, 1.0f);
    // public bool StyleChanged = true;

    public bool UseCustomChannel = false;
    public XivChatType MessageChatChannel = Service.PluginInterface.GeneralChatType;
    public bool UseCustomStatusMessage = false;
    public string CustomStatusMessage = string.Empty;
    public bool UseCustomResetMessage  = false;
    public string CustomResetMessage = string.Empty;

    public bool Suppressed;
    
    protected virtual bool DrawModuleConfig() {
        ImGui.TextColored(KnownColor.Orange.Vector(), "No additional options for this module");
        return false;
    }
    
    public virtual bool DrawConfigUi() {
        var configChanged = false;
        
        ImGui.TextUnformatted(Strings.ModuleEnable);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        
        using (var indent = ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.Enable, ref ModuleEnabled);
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted(Strings.ModuleConfiguration);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        using (var indent = ImRaii.PushIndent()) {
            configChanged |= DrawModuleConfig();
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted(Strings.NotificationOptions);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        using (var indent = ImRaii.PushIndent()) {
            configChanged |= ImGuiTweaks.Checkbox(Strings.SendStatusOnLogin, ref OnLoginMessage, Strings.SendStatusOnLoginHelp);
            configChanged |= ImGuiTweaks.Checkbox(Strings.SendStatusOnZoneChange, ref OnZoneChangeMessage, Strings.SendStatusOnZoneChangeHelp);
            configChanged |= ImGuiTweaks.Checkbox(Strings.SendMessageOnReset, ref ResetMessage, Strings.SendMessageOnResetHelp);
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted(Strings.NotificationCustomization);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        using (var indent = ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.EnableCustomChannel, ref UseCustomChannel);
            
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            configChanged |= ImGuiTweaks.EnumCombo("##ChannelSelect", ref MessageChatChannel);

            ImGuiHelpers.ScaledDummy(3.0f);
            configChanged |= ImGui.Checkbox(Strings.EnableCustomStatusMessage, ref UseCustomStatusMessage);
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.InputTextWithHint("##CustomStatusMessage", Strings.StatusMessage, ref CustomStatusMessage, 1024);
            if (ImGui.IsItemDeactivatedAfterEdit()) {
                configChanged = true;
            }

            ImGuiHelpers.ScaledDummy(3.0f);
            configChanged |= ImGui.Checkbox(Strings.EnableCustomResetMessage, ref UseCustomResetMessage);
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.InputTextWithHint("##CustomResetMessage", Strings.ResetMessage, ref CustomResetMessage, 1024);
            if (ImGui.IsItemDeactivatedAfterEdit()) {
                configChanged = true;
            }
        }
        
        return configChanged;
    }
}

public abstract class BaseModule : IDisposable {
    public abstract ModuleName ModuleName { get; }
    
    public abstract ModuleType ModuleType { get; }
    
    public virtual void Dispose() { }

    public virtual bool HasTooltip { get; protected set; } = false;
    
    public virtual string TooltipText { get; protected set; } = string.Empty;
    
    public virtual bool HasClickableLink { get; protected set; } = false;
    
    public virtual PayloadId ClickableLinkPayloadId { get; protected set; } = PayloadId.Unknown;

    public virtual ModuleStatus ModuleStatus => ModuleStatus.Unknown;
    
    public virtual bool IsEnabled => false;

    public abstract void DrawConfig();

    public abstract void DrawData();

    public abstract void Update();

    public abstract void Load();

    public abstract void Unload();

    public abstract void Reset();

    public abstract void ZoneChange(uint newZone);

    public abstract void SaveConfig();

    public abstract void SaveData();
    
    public abstract DateTime GetNextReset();

    protected abstract ModuleStatus GetModuleStatus();
    
    protected abstract StatusMessage GetStatusMessage();

    protected virtual void UpdateTaskLists() { }
}

public abstract class BaseModule<T, TU> : BaseModule where T : ModuleDataBase, new() where TU : ModuleConfigBase, new() {
    protected T Data { get; set; } = new();

    protected TU Config { get; set; } = new();

    public override bool IsEnabled => Config.ModuleEnabled;

    public override ModuleStatus ModuleStatus => Config.Suppressed ? ModuleStatus.Suppressed : GetModuleStatus();
    
    protected XivChatType GetChatChannel() => Config.UseCustomChannel ? Config.MessageChatChannel : Service.PluginInterface.GeneralChatType;
    
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

        using (var disabled = ImRaii.Disabled(!(ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl))) {
            if (ImGui.Button(Strings.Snooze, new Vector2(ImGui.GetContentRegionAvail().X, 23.0f * ImGuiHelpers.GlobalScale))) {
                Config.Suppressed = true;
            }

            using (var alphaStyle = ImRaii.PushStyle(ImGuiStyleVar.Alpha, 1.0f)) {
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

        ImGui.TableNextColumn();
        ImGui.TextUnformatted(Strings.TimeRemaining);

        ImGui.TableNextColumn();
        var timeRemaining = GetNextReset() - DateTime.UtcNow;
        ImGui.TextUnformatted($"{timeRemaining.Days}.{timeRemaining.Hours:00}:{timeRemaining.Minutes:00}:{timeRemaining.Seconds:00}");
    }
    
    public override void Update() {
        if (DataChanged || ConfigChanged) UpdateTaskData();
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
        SaveData();
        
        Config.Suppressed = false;
        SaveConfig();
    }

    public override void ZoneChange(uint newZone) {
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