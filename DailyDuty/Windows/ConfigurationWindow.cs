using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
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
        using var _ = Service.PluginInterface.PushIconFont();

        switch (option.ModuleStatus) {
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

        using var id = ImRaii.PushId("main_config");
  
        ImGuiTweaks.Header("Todo List Config");
        using (ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.Enable, ref System.TodoConfig.Enabled);
        }
        
        ImGuiTweaks.Header("Functional Options");
        using (ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.HideInQuestEvent, ref System.TodoConfig.HideDuringQuests);
            configChanged |= ImGui.Checkbox(Strings.HideInDuties, ref System.TodoConfig.HideInDuties);
        }
        
        ImGuiTweaks.Header("Todo List Style");
        using (var child = ImRaii.Child("TodoListStyleConfig", ImGui.GetContentRegionAvail() - ImGuiHelpers.ScaledVector2(0.0f, 33.0f))) {
            if (child) {
                System.TodoListController.DrawConfig();
            }
        }
        
        ImGui.Separator();
        
        if (ImGui.Button("Save", ImGuiHelpers.ScaledVector2(100.0f, 23.0f))) {
            System.TodoListController.Save();
            System.TodoListController.Refresh();
            StatusMessage.PrintTaggedMessage("Saved configuration options for Todo List", "Todo List Config");
        }
        
        ImGui.SameLine(ImGui.GetContentRegionMax().X / 2.0f - 75.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.Button("Refresh Layout", ImGuiHelpers.ScaledVector2(150.0f, 23.0f))) {
            System.TodoListController.Refresh();
        }
        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip("Triggers a refresh of the UI element to recalculate dynamic element size/positions");
        }
        
        ImGui.SameLine(ImGui.GetContentRegionMax().X - 100.0f * ImGuiHelpers.GlobalScale);
        ImGuiTweaks.DisabledButton("Reset", () => {
            System.TodoListController.Load();
            System.TodoListController.Refresh();
            StatusMessage.PrintTaggedMessage("Loaded last saved configuration options for Todo List", "Todo List Config");
        });
        
        if (configChanged) {
            System.TodoConfig.Save();
            System.TodoListController.Refresh();
        }
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
            
            ImGuiHelpers.ScaledDummy(5.0f);
            configChanged |= ImGui.Checkbox("Hide Seconds", ref System.TimersConfig.HideTimerSeconds);

        }

        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);

        using (var child = ImRaii.Child("timers_child", ImGui.GetContentRegionAvail() - ImGuiHelpers.ScaledVector2(0.0f, 33.0f))) {
            if (child) {
                using var table = ImRaii.Table("special_timers_config", 2);
                if (table) {
                    ImGui.TableNextColumn();
                    using (var weeklyChild = ImRaii.Child("weekly_child", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding)) {
                        if (weeklyChild) {
                            using (ImRaii.PushId("Weekly")) {
                                ImGui.TextUnformatted("Weekly Timer");
                                ImGuiHelpers.ScaledDummy(5.0f);
                                System.TimersController.WeeklyTimerNode?.DrawConfig();
                            }
                        }
                    }
                
                    ImGui.TableNextColumn();
                    using (var weeklyChild = ImRaii.Child("daily_child", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding)) {
                        if (weeklyChild) {
                            using (ImRaii.PushId("Daily")) {
                                ImGui.TextUnformatted("Daily Timer");
                                ImGuiHelpers.ScaledDummy(5.0f);
                                System.TimersController.DailyTimerNode?.DrawConfig();
                            }
                        }
                    }
                }
            }
        }
        
        ImGui.Separator();
        
        if (ImGui.Button("Save", ImGuiHelpers.ScaledVector2(100.0f, 23.0f))) {
            System.TimersController.WeeklyTimerNode?.Save(System.TimersController.WeeklyTimerSavePath);
            System.TimersController.DailyTimerNode?.Save(System.TimersController.DailyTimerSavePath);
            StatusMessage.PrintTaggedMessage("Saved configuration options for Timers", "Timers Config");
        }
        
        ImGui.SameLine(ImGui.GetContentRegionMax().X - 100.0f * ImGuiHelpers.GlobalScale);
        ImGuiTweaks.DisabledButton("Reset", () => {
            System.TimersController.WeeklyTimerNode?.Load(System.TimersController.WeeklyTimerSavePath);
            System.TimersController.DailyTimerNode?.Load(System.TimersController.DailyTimerSavePath);
            StatusMessage.PrintTaggedMessage("Loaded last saved configuration options for Timers", "Timers Config");
        });
        
        if (configChanged) {
            System.TimersConfig.Save();
        }
    }
}