using System.Drawing;
using System.Text.Json.Serialization;
using DailyDuty.CustomNodes;
using DailyDuty.Localization;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using KamiLib.Classes;

namespace DailyDuty.Modules.BaseModules;

public abstract class ModuleConfig {
    public bool ModuleEnabled;
	    
    public bool OnLoginMessage = true;
    public bool OnZoneChangeMessage = true;
    public bool ResetMessage;
	    
    public bool TodoEnabled = true;

    public bool UseCustomChannel;
    public XivChatType MessageChatChannel = Service.PluginInterface.GeneralChatType;
    public bool UseCustomStatusMessage;
    public string CustomStatusMessage = string.Empty;
    public bool UseCustomResetMessage;
    public string CustomResetMessage = string.Empty;

    public bool Suppressed;

    [JsonIgnore] public bool ConfigChanged;
	    
    protected virtual void DrawModuleConfig() {
        ImGui.TextColored(KnownColor.Orange.Vector(), "No additional options for this module");
    }
    
    public void DrawConfigUi(Module module) {
        using var tabBar = ImRaii.TabBar("config_tabs");
        if (!tabBar) return;
        
        DrawModuleTab();
        DrawNotificationTab();
        DrawTodoTab(module);
    }

    private void DrawModuleTab() {
        using var moduleTab = ImRaii.TabItem("Module");
        if (!moduleTab) return;
        
        using var tabChild = ImRaii.Child("tab_child", ImGui.GetContentRegionAvail());
        if (!tabChild) return;

        ImGuiTweaks.Header(Strings.ModuleEnable);
        using (ImRaii.PushIndent()) {
            ConfigChanged |= ImGui.Checkbox(Strings.Enable, ref ModuleEnabled);
        }

        ImGuiTweaks.Header(Strings.ModuleConfiguration);
        using (ImRaii.PushIndent()) {
            DrawModuleConfig();
        }
    }

    private void DrawNotificationTab() {
        using var notificationTab = ImRaii.TabItem("Notifications");
        if (!notificationTab) return;
                
        using var tabChild = ImRaii.Child("tab_child", ImGui.GetContentRegionAvail());
        if (!tabChild) return;
        
        ImGuiTweaks.Header(Strings.NotificationOptions);
        using (ImRaii.PushIndent()) {
            ConfigChanged |= ImGuiTweaks.Checkbox(Strings.SendStatusOnLogin, ref OnLoginMessage, Strings.SendStatusOnLoginHelp);
            ConfigChanged |= ImGuiTweaks.Checkbox(Strings.SendStatusOnZoneChange, ref OnZoneChangeMessage, Strings.SendStatusOnZoneChangeHelp);
            ConfigChanged |= ImGuiTweaks.Checkbox(Strings.SendMessageOnReset, ref ResetMessage, Strings.SendMessageOnResetHelp);
        }

        ImGuiTweaks.Header(Strings.NotificationCustomization);
        using (ImRaii.PushIndent()) {
            ConfigChanged |= ImGui.Checkbox(Strings.EnableCustomChannel, ref UseCustomChannel);

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ConfigChanged |= ImGuiTweaks.EnumCombo("##ChannelSelect", ref MessageChatChannel);

            ImGuiHelpers.ScaledDummy(3.0f);
            ConfigChanged |= ImGui.Checkbox(Strings.EnableCustomStatusMessage, ref UseCustomStatusMessage);
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.InputTextWithHint("##CustomStatusMessage", Strings.StatusMessage, ref CustomStatusMessage, 1024);
            if (ImGui.IsItemDeactivatedAfterEdit()) {
                ConfigChanged = true;
            }

            ImGuiHelpers.ScaledDummy(3.0f);
            ConfigChanged |= ImGui.Checkbox(Strings.EnableCustomResetMessage, ref UseCustomResetMessage);
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.InputTextWithHint("##CustomResetMessage", Strings.ResetMessage, ref CustomResetMessage, 1024);
            if (ImGui.IsItemDeactivatedAfterEdit()) {
                ConfigChanged = true;
            }
        }
    }

    private void DrawTodoTab(Module module) {
        using var todoTab = ImRaii.TabItem("Todo");
        if (!todoTab) return;

        using var tabChild = ImRaii.Child("tab_child", ImGui.GetContentRegionAvail());
        if (!tabChild) return;

        ImGuiTweaks.Header(Strings.TodoConfiguration);
        using (ImRaii.PushIndent()) {
            ConfigChanged |= ImGui.Checkbox(Strings.TodoEnable, ref TodoEnabled);
        }

        ImGuiTweaks.Header("Style Configuration");
        DrawNodeConfig(module);

        System.TodoListController.Refresh();
    }

    private void DrawNodeConfig(Module module) {
        using var modeTabBar = ImRaii.TabBar("modeSelect");
        if (!modeTabBar) return;
        
        DrawSimpleConfigTab(module);
        DrawAdvancedConfigTab(module);
    }

    private static void DrawAdvancedConfigTab(Module module) {
        using var advancedModeTab = ImRaii.TabItem("Advanced Mode");
        if (!advancedModeTab) return;
        
        using var child = ImRaii.Child("tab_child", ImGui.GetContentRegionAvail());
        if (!child) return;

        module.TodoTaskNode?.DrawConfig();
    }

    private void DrawSimpleConfigTab(Module module) {
        using var simpleModeTab = ImRaii.TabItem("Simple Mode");
        if (!simpleModeTab) return;

        DrawSimpleModeConfig(module.TodoTaskNode);
    }

    private void DrawSimpleModeConfig(TodoTaskNode? node) {
        if (node is null) return;

        using var table = ImRaii.Table("simple_mode_table", 2);
        if (!table) return;
        
        ImGui.TableSetupColumn("##label", ImGuiTableColumnFlags.WidthStretch, 1.0f);
        ImGui.TableSetupColumn("##config", ImGuiTableColumnFlags.WidthStretch, 2.0f);
        
        ImGui.TableNextRow();

        ImGui.TableNextColumn();
        ImGui.Text("Text Color");

        ImGui.TableNextColumn();
        var textColor = node.TextColor;
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.ColorEdit4("##TextColor", ref textColor, ImGuiColorEditFlags.AlphaPreviewHalf)) {
            node.TextColor = textColor;
        }

        // Maybe not include this here? Gonna include a set-all option in TodoList config
        // ImGui.TableNextColumn();
        // ImGui.Text("Font Size");
        //
        // ImGui.TableNextColumn();
        // var fontSize = (int) node.FontSize;
        // ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        // if (ImGui.InputInt("##FontSize", ref fontSize)) {
        //     node.FontSize = (uint) fontSize;
        //     node.Text = node.Text;
        // }
        
        ImGui.TableNextColumn();
        ImGui.Text("Label");
        
        ImGui.TableNextColumn();
        var label = node.String;
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.InputText("##Label", ref label, 100)) {
            node.String = label;
        }
    }
}