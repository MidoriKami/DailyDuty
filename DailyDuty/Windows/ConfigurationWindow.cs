using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules;
using DailyDuty.Modules.BaseModules;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.CommandManager;
using KamiLib.Configuration;
using KamiLib.Extensions;
using KamiLib.Window;

namespace DailyDuty.Windows;

public class ConfigurationWindow : TabbedSelectionWindow<Module> {

    protected override string SelectionListTabName => "Modules";
    
    protected override List<ITabItem> Tabs { get; } = [
        new TodoConfigTab(),
        new TimersConfigTab(),
    ];
    
    protected override List<Module> Options => System.ModuleController.Modules;
    
    protected override float SelectionListWidth { get; set; } = 200.0f;
    
    protected override float SelectionItemHeight => ImGui.GetTextLineHeight() / ImGuiHelpers.GlobalScale;

    protected override bool ShowListButton => true;

    protected override bool FilterOptions(Module option)
        => !System.SystemConfig.HideDisabledModules || option.IsEnabled;

    public ConfigurationWindow() : base("DailyDuty - Configuration Window", new Vector2(1000.0f, 500.0f)) {
        TitleBarButtons.Add(new TitleBarButton {
            Click = _ => System.WindowManager.AddWindow(new ConfigurationManagerWindow(), WindowFlags.OpenImmediately),
            Icon = FontAwesomeIcon.Cog,
            ShowTooltip = () => ImGui.SetTooltip("Open Configuration Manager"),
            IconOffset = new Vector2(2.0f, 1.0f),
        });
        
        System.CommandManager.RegisterCommand(new CommandHandler {
            Delegate = _ => UnCollapseOrToggle(),
            ActivationPath = "/",
        });
    }

    protected override void DrawListOption(Module option) {
        ImGui.Text(option.ModuleName.GetDescription());
        
        ImGui.SameLine(ImGui.GetContentRegionAvail().X - 13.0f * ImGuiHelpers.GlobalScale);
        using var _ = Service.PluginInterface.UiBuilder.IconFontFixedWidthHandle.Push();

        var status = option.ModuleStatus;

        switch (status) {
            case ModuleStatus.Suppressed when option.IsEnabled:
                ImGui.TextColored(KnownColor.MediumPurple.Vector(), FontAwesomeIcon.History.ToIconString());
                break;
            
            case ModuleStatus.Unknown when option.IsEnabled:
                ImGui.TextColored(KnownColor.Orange.Vector(), FontAwesomeIcon.Question.ToIconString());
                break;
            
            default:
                var color = option.IsEnabled ? KnownColor.Green.Vector() : KnownColor.Red.Vector();
                ImGui.TextColored(color, FontAwesomeIcon.Circle.ToIconString());
                break;
        }
    }

    protected override void DrawSelectedOption(Module option) {
        using var table = ImRaii.Table("module_table", 2, ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.Resizable, ImGui.GetContentRegionAvail());
        if (!table) return;

        ImGui.TableNextColumn();
        using (var _ = ImRaii.Child($"config_child_{option.ModuleName}", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding)) {
            option.DrawConfig();
        }

        ImGui.TableNextColumn();
        using (var _ = ImRaii.Child($"data_child_{option.ModuleName}", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding)) {
            option.DrawData();
        }
    }

    protected override void DrawExtraButton() {
        var label = System.SystemConfig.HideDisabledModules ? Strings.ShowDisabled : Strings.HideDisabled;

        if (ImGui.Button(label, ImGui.GetContentRegionAvail())) {
            System.SystemConfig.HideDisabledModules = !System.SystemConfig.HideDisabledModules;
            System.SystemConfig.Save();
        }
    }
}

public class TodoConfigTab : ITabItem {
    public string Name => "Todo List";
    
    public bool Disabled => false;
    
    public void Draw() {
        var configChanged = false;

        using (var tabBar = ImRaii.TabBar("todo_tab_bar")) {
            if (tabBar) {
                using (var masterTab = ImRaii.TabItem("General")) {
                    if (masterTab) {
                        using var tabChild = ImRaii.Child("tab_child");
                        if (tabChild) {
                            configChanged |= DrawMainConfig();
                        }
                    }
                }
                
                using (var dailyTab = ImRaii.TabItem("Daily")) {
                    if (dailyTab) {
                        using var tabChild = ImRaii.Child("tab_child");
                        if (tabChild) {
                            configChanged |= DrawCategory(ModuleType.Daily);
                        }
                    }
                }
                
                using (var weeklyTab = ImRaii.TabItem("Weekly")) {
                    if (weeklyTab) {
                        using var tabChild = ImRaii.Child("tab_child");
                        if (tabChild) {
                            configChanged |= DrawCategory(ModuleType.Weekly);
                        }
                    }
                }
                
                using (var specialTab = ImRaii.TabItem("Special")) {
                    if (specialTab) {
                        using var tabChild = ImRaii.Child("tab_child");
                        if (tabChild) {
                            configChanged |= DrawCategory(ModuleType.Special);
                        }
                    }
                }
            }
        }
        
        if (configChanged) {
            System.TodoConfig.Save();
            System.TodoListController.Refresh();
        }
    }

    private bool DrawMainConfig() {
        using var id = ImRaii.PushId("main_config");
        var configChanged = false;
  
        ImGuiTweaks.Header("Todo List Config");
        using (ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.Enable, ref System.TodoConfig.Enabled);
        }

        ImGuiTweaks.Header("Todo List Style");
        using (ImRaii.PushIndent()) {
            configChanged |= System.TodoConfig.ListStyle.DrawSettings();
        }
        
        ImGuiTweaks.Header("Functional Options");
        using (ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.HideInQuestEvent, ref System.TodoConfig.HideDuringQuests);
            configChanged |= ImGui.Checkbox(Strings.HideInDuties, ref System.TodoConfig.HideInDuties);
        }

        return configChanged;
    }
    
    private bool DrawCategory(ModuleType type) {
        using var id = ImRaii.PushId(type.ToString());
        
        var config = System.TodoConfig.CategoryConfigs[(uint)type];
        var configChanged = false;

        ImGuiTweaks.Header($"{type.GetDescription()} Config");
        using (ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.Enable, ref config.Enabled);
        }
        
        ImGuiTweaks.Header("Custom Label");
        using (ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox("Use Custom Label", ref config.UseCustomLabel);
            
            ImGuiHelpers.ScaledDummy(5.0f);
            configChanged |= ImGui.InputText("Custom Label", ref config.CustomLabel, 1024, ImGuiInputTextFlags.AutoSelectAll);
        }
        
        ImGuiTweaks.Header("Category Style");
        using (ImRaii.PushIndent()) {
            using (ImRaii.TabBar("todo_node_options")) {
                using (var tabItem = ImRaii.TabItem("Category Container")) {
                    if (tabItem) {
                        configChanged |= config.ListNodeStyle.DrawSettings();
                    }
                }
            
                using (var tabItem = ImRaii.TabItem("Header Text")) {
                    if (tabItem) {
                        configChanged |= config.HeaderStyle.DrawSettings();
                    }
                }
            
                using (var tabItem = ImRaii.TabItem("Task Text")) {
                    if (tabItem) {
                        configChanged |= config.ModuleStyle.DrawSettings();
                    }
                }
            }
        }
        
        return configChanged;
    }
}

public class TimersConfigTab : ITabItem {
    
    public string Name => "Timers";
    public bool Disabled => false;
    public void Draw() {
        var configChanged = false;

        ImGuiTweaks.Header("Timers Config");
        using (ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.Enable, ref System.TimersConfig.Enabled);
            
            ImGuiHelpers.ScaledDummy(5.0f);

            configChanged |= ImGui.Checkbox(Strings.HideInDuties, ref System.TimersConfig.HideInDuties);
            configChanged |= ImGui.Checkbox(Strings.HideInQuestEvent, ref System.TimersConfig.HideInQuestEvents);
            configChanged |= ImGui.Checkbox(Strings.HideTimerTooltip, ref System.TimersConfig.HideTimerTooltip);
        }

        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);

        using (var table = ImRaii.Table("special_timers_config", 2)) {
            if (table) {
                ImGui.TableNextColumn();
                using (ImRaii.PushId("Weekly")) {
                    ImGui.TextUnformatted("Weekly Timer");
                    ImGuiHelpers.ScaledDummy(5.0f);
                    using (ImRaii.PushIndent()) {
                        configChanged |= System.TimersConfig.WeeklyTimerConfig.Draw();
                    }
                }
                
                ImGui.TableNextColumn();
                using (ImRaii.PushId("Daily")) {
                    ImGui.TextUnformatted("Daily Timer");
                    ImGuiHelpers.ScaledDummy(2.0f);
                    using (ImRaii.PushIndent()) {
                        configChanged |= System.TimersConfig.DailyTimerConfig.Draw();
                    }
                }
            }
        }
        
        if (configChanged) {
            System.TimersConfig.Save();
            System.TimersController.Refresh();
        }
    }
}