using System.Numerics;
using DailyDuty.Classes.Timers;
using DailyDuty.Localization;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.Configuration;
using KamiToolKit.Nodes.NodeStyles;

namespace DailyDuty.Models;

public class TimersConfig {
    public int Version = 2;

    public bool Enabled = false;
	
    public bool HideInDuties = true;
    public bool HideInQuestEvents = true;

    public TimerConfig WeeklyTimerConfig = new() {
        Style = new TimerNodeStyle {
            Size = new Vector2(400.0f, 32.0f),
            Scale = new Vector2(0.80f, 0.80f),
            Position = new Vector2(400.0f, 400.0f),
            BaseDisable = BaseStyleDisable.Visible | BaseStyleDisable.Size | BaseStyleDisable.NodeFlags | BaseStyleDisable.Color | BaseStyleDisable.Margin,
        },
    };
    
    public TimerConfig DailyTimerConfig = new() {
        Style = new TimerNodeStyle {
            Size = new Vector2(400.0f, 32.0f),
            Scale = new Vector2(0.80f, 0.80f),
            Position = new Vector2(400.0f, 475.0f),
            BaseDisable = BaseStyleDisable.Visible | BaseStyleDisable.Size | BaseStyleDisable.NodeFlags | BaseStyleDisable.Color | BaseStyleDisable.Margin,
        },
    };
	
    public static TimersConfig Load() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "Timers.config.json", () => new TimersConfig());

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "Timers.config.json", this);
}

public class TimerConfig {
    public bool TimerEnabled;
    public bool HideSeconds;
    public bool UseCustomLabel;
    public string CustomLabel = string.Empty;

    public TimerNodeStyle Style = new();

    public bool Draw() {
        var configChanged = false;
		
        configChanged |= ImGui.Checkbox("Enable Timer Display", ref TimerEnabled);
        ImGuiHelpers.ScaledDummy(5.0f);

        configChanged |= ImGui.Checkbox(Strings.UseCustomLabel, ref UseCustomLabel);
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        configChanged |= ImGui.InputTextWithHint("##CustomTimerLabel", "Custom Timer Label...", ref CustomLabel, 1024);
                       
        ImGuiHelpers.ScaledDummy(5.0f);
        configChanged |= ImGui.Checkbox("Hide Seconds", ref HideSeconds);
        
        ImGuiHelpers.ScaledDummy(5.0f);
        configChanged |= Style.DrawSettings();

        return configChanged;
    }
}