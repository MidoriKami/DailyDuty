using System.Drawing;
using System.Numerics;
using DailyDuty.Localization;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.Classes;

namespace DailyDuty.Modules.BaseModules;

public abstract class ModuleConfig {
	public bool ModuleEnabled;
    
	public bool OnLoginMessage = true;
	public bool OnZoneChangeMessage = true;
	public bool ResetMessage;
    
	public bool TodoEnabled = true;
	public bool UseCustomTodoLabel;
	public string CustomTodoLabel = string.Empty;
	public bool OverrideTextColor;
	public Vector4 TodoTextColor = KnownColor.White.Vector();
	public Vector4 TodoTextOutline = KnownColor.Black.Vector();

	public bool UseCustomChannel;
	public XivChatType MessageChatChannel = Service.PluginInterface.GeneralChatType;
	public bool UseCustomStatusMessage;
	public string CustomStatusMessage = string.Empty;
	public bool UseCustomResetMessage;
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
        
		using (ImRaii.PushIndent()) {
			configChanged |= ImGui.Checkbox(Strings.Enable, ref ModuleEnabled);
		}
        
		ImGuiHelpers.ScaledDummy(10.0f);
		ImGui.TextUnformatted(Strings.ModuleConfiguration);
		ImGui.Separator();
		ImGuiHelpers.ScaledDummy(5.0f);
		using (ImRaii.PushIndent()) {
			configChanged |= DrawModuleConfig();
		}
        
		ImGuiHelpers.ScaledDummy(10.0f);
		ImGui.TextUnformatted(Strings.NotificationOptions);
		ImGui.Separator();
		ImGuiHelpers.ScaledDummy(5.0f);
		using (ImRaii.PushIndent()) {
			configChanged |= ImGuiTweaks.Checkbox(Strings.SendStatusOnLogin, ref OnLoginMessage, Strings.SendStatusOnLoginHelp);
			configChanged |= ImGuiTweaks.Checkbox(Strings.SendStatusOnZoneChange, ref OnZoneChangeMessage, Strings.SendStatusOnZoneChangeHelp);
			configChanged |= ImGuiTweaks.Checkbox(Strings.SendMessageOnReset, ref ResetMessage, Strings.SendMessageOnResetHelp);
		}
        
		ImGuiHelpers.ScaledDummy(10.0f);
		ImGui.TextUnformatted(Strings.NotificationCustomization);
		ImGui.Separator();
		ImGuiHelpers.ScaledDummy(5.0f);
		using (ImRaii.PushIndent()) {
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
        
		ImGuiHelpers.ScaledDummy(10.0f);
		ImGui.TextUnformatted(Strings.TodoConfiguration);
		ImGui.Separator();
		ImGuiHelpers.ScaledDummy(5.0f);
		using (ImRaii.PushIndent()) {
			configChanged |= ImGui.Checkbox(Strings.TodoEnable, ref TodoEnabled);

			ImGuiHelpers.ScaledDummy(5.0f);

			configChanged |= ImGui.Checkbox(Strings.UseCustomLabel, ref UseCustomTodoLabel);
			ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
			ImGui.InputTextWithHint("##CustomTodoLabel", "Custom Todo Label...", ref CustomTodoLabel, 1024);

			ImGuiHelpers.ScaledDummy(5.0f);
            
			configChanged |= ImGui.Checkbox(Strings.OverrideTodoListColor, ref OverrideTextColor);
			configChanged |= ImGuiTweaks.ColorEditWithDefault(Strings.TextColor, ref TodoTextColor, KnownColor.White.Vector());
			configChanged |= ImGuiTweaks.ColorEditWithDefault(Strings.TextOutlineColor, ref TodoTextOutline, KnownColor.Black.Vector());
		}
        
		return configChanged;
	}
}