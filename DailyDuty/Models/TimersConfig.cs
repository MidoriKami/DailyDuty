using System.Drawing;
using System.Numerics;
using DailyDuty.Localization;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.Configuration;

namespace DailyDuty.Models;

public class TimersConfig {

    public bool Enabled = false;
	
    public bool HideInDuties = true;
    public bool HideInQuestEvents = true;

    public TimerConfig WeeklyTimerConfig = new();
    public TimerConfig DailyTimerConfig = new();
	
    public static TimersConfig Load() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "Timers.config.json", () => new TimersConfig());

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "Timers.config.json", this);
}

public class TimerConfig {
    public bool TimerEnabled = false;
    public Vector2 Position = new(400.0f, 400.0f);
    public Vector2 Size = new(400.0f, 32.0f);
    public Vector4 BarColor = KnownColor.Aqua.Vector();
    public Vector4 BarBackgroundColor = KnownColor.Black.Vector();
    public bool HideWhenComplete = true;
    public bool HideName;
    public bool HideTime;
    public bool HideSeconds;
    public bool UseCustomLabel;
    public string CustomLabel = string.Empty;
    public float Scale = 0.80f;

    public bool Draw(bool disableHideWhenComplete = false) {
        var configChanged = false;
		
        configChanged |= ImGui.Checkbox("Enable Timer Display", ref TimerEnabled);
        ImGuiHelpers.ScaledDummy(5.0f);

        ImGui.Text("Position");
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        configChanged |= ImGui.DragFloat2("##position", ref Position, 5.0f);

        ImGuiHelpers.ScaledDummy(5.0f);
        ImGui.Text("Size");
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        configChanged |= ImGui.DragFloat2("##Size", ref Size, 5.0f);

        ImGuiHelpers.ScaledDummy(5.0f);
        ImGui.Text("Scale");
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        configChanged |= ImGui.DragFloat("##Scale", ref Scale, 0.005f);
                        
        ImGuiHelpers.ScaledDummy(5.0f);
        configChanged |= ImGui.Checkbox(Strings.UseCustomLabel, ref UseCustomLabel);
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        configChanged |= ImGui.InputTextWithHint("##CustomTimerLabel", "Custom Timer Label...", ref CustomLabel, 1024);

        if (!disableHideWhenComplete) {
            ImGuiHelpers.ScaledDummy(5.0f);
            configChanged |= ImGui.Checkbox("Hide when complete", ref HideWhenComplete);
        }
                       
        ImGuiHelpers.ScaledDummy(5.0f);
        configChanged |= ImGui.Checkbox("Hide Label", ref HideName);
        configChanged |= ImGui.Checkbox("Hide Time", ref HideTime);
        configChanged |= ImGui.Checkbox("Hide Seconds", ref HideSeconds);

        ImGuiHelpers.ScaledDummy(5.0f);
        configChanged |= ImGuiTweaks.ColorEditWithDefault("Progress Color", ref BarColor, KnownColor.Aqua.Vector());
        configChanged |= ImGuiTweaks.ColorEditWithDefault("Background Color", ref BarBackgroundColor, KnownColor.Black.Vector());

        return configChanged;
    }
}