using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Localization;
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

public class ConfigurationWindow : TabbedSelectionWindow<ModuleBase> {

    protected override string SelectionListTabName => "Modules";
    
    protected override List<ITabItem> Tabs { get; } = [
        new TodoConfigTab(),
    ];
    
    protected override List<ModuleBase> Options => System.ModuleController.Modules;
    
    protected override float SelectionListWidth { get; set; } = 200.0f;
    
    protected override float SelectionItemHeight => ImGui.CalcTextSize("Butts").Y;

    protected override bool ShowListButton => true;

    protected override bool FilterOptions(ModuleBase option)
        => !System.SystemConfig.HideDisabledModules || option.IsEnabled;

    public ConfigurationWindow() : base("DailyDuty - Configuration Window", new Vector2(1000.0f, 400.0f)) {
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

    protected override void DrawListOption(ModuleBase option) {
        ImGui.Text(option.ModuleName.GetDescription(Strings.ResourceManager));
        
        ImGui.SameLine(ImGui.GetContentRegionAvail().X- 10.0f * ImGuiHelpers.GlobalScale);
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

    protected override void DrawSelectedOption(ModuleBase option) {
        using var table = ImRaii.Table($"module_table", 2, ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.Resizable, ImGui.GetContentRegionAvail());
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
        
        using var table = ImRaii.Table("todo_config_table", 4, ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.Resizable);
        if (!table) return;

        ImGui.TableNextColumn();
        using (ImRaii.Child("main_child", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding)) {
            configChanged |= DrawMainConfig();
        }

        ImGui.TableNextColumn();
        using (ImRaii.Child("daily_child", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding)) {
            configChanged |= DrawCategory(ModuleType.Daily);
        }
        
        ImGui.TableNextColumn();
        using (ImRaii.Child("weekly_child", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding)) {
            configChanged |= DrawCategory(ModuleType.Weekly);
        }
        
        ImGui.TableNextColumn();
        using (ImRaii.Child("special_child", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding)) {
            configChanged |= DrawCategory(ModuleType.Special);
        }
        
        if (configChanged) {
            System.TodoConfig.Save();
            System.TodoListController.Refresh();
        }
    }

    private bool DrawMainConfig() {
        using var id = ImRaii.PushId("main_config");
        var configChanged = false;
  
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted("Todo List Config");
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        using (ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.Enable, ref System.TodoConfig.Enabled);
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted("Display Options");
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        using (ImRaii.PushIndent()) {
            ImGui.Text("Position");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            configChanged |= ImGui.DragFloat2(Strings.Position, ref System.TodoConfig.Position, 5.0f);
            
            ImGuiHelpers.ScaledDummy(5.0f);
            
            ImGui.Text("Size");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            configChanged |= ImGui.DragFloat2("Size", ref System.TodoConfig.Size, 5.0f);
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted("Style Options");
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        using (ImRaii.PushIndent()) {
            
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
            configChanged |= ImGuiTweaks.EnumCombo("Anchor Corner", ref System.TodoConfig.Anchor, Strings.ResourceManager);
            
            ImGuiHelpers.ScaledDummy(5.0f);
            configChanged |= ImGui.Checkbox("Single Line", ref System.TodoConfig.SingleLine);
            configChanged |= ImGui.Checkbox("Show Background", ref System.TodoConfig.ShowListBackground);
            
            ImGuiHelpers.ScaledDummy(5.0f);
            configChanged |= ImGuiTweaks.ColorEditWithDefault("Background Color", ref System.TodoConfig.ListBackgroundColor, KnownColor.Aqua.Vector() with { W = 0.40f });
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted($"Functional Options");
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
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

        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted($"{type.GetDescription(Strings.ResourceManager)} Config");
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        using (ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.Enable, ref config.Enabled);
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted($"Style Options");
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        using (ImRaii.PushIndent()) {
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
            configChanged |= ImGuiTweaks.EnumCombo("Anchor Corner", ref config.LayoutAnchor, Strings.ResourceManager);
            ImGuiHelpers.ScaledDummy(5.0f);

            configChanged |= ImGui.Checkbox(Strings.EnableOutline, ref config.Edge);
            configChanged |= ImGui.Checkbox(Strings.EnableGlowingOutline, ref config.Glare);
            
            ImGuiHelpers.ScaledDummy(5.0f);
            ImGui.Text("Category Spacing");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            configChanged |= ImGui.DragFloat4("Category Spacing", ref config.CategoryMargin, 0.05f);
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted($"Header Options");
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        using (ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox("Show Header", ref config.ShowHeader);
            configChanged |= ImGui.Checkbox(Strings.HeaderItalic, ref config.HeaderItalic);

            ImGuiHelpers.ScaledDummy(5.0f);
            ImGui.Text("Header Font Size");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            configChanged |= ImGuiTweaks.SliderUint(Strings.HeaderSize, ref config.HeaderFontSize, 1, 64);
            
            ImGuiHelpers.ScaledDummy(5.0f);
            configChanged |= ImGuiTweaks.ColorEditWithDefault(Strings.HeaderColor, ref config.HeaderTextColor, KnownColor.White.Vector());
            configChanged |= ImGuiTweaks.ColorEditWithDefault(Strings.HeaderOutlineColor, ref config.HeaderTextOutline, KnownColor.Orange.Vector());
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted("Module Options");
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        using (ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.ModuleItalic, ref config.ModuleItalic);

            ImGuiHelpers.ScaledDummy(5.0f);
            ImGui.Text("Module Font Size");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            configChanged |= ImGuiTweaks.SliderUint(Strings.FontSize, ref config.ModuleFontSize, 1, 64);
            
            ImGuiHelpers.ScaledDummy(5.0f);
            ImGui.Text("Module Spacing");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            configChanged |= ImGui.DragFloat4("Module Spacing", ref config.ModuleMargin, 0.05f);
            
            ImGuiHelpers.ScaledDummy(5.0f);
            configChanged |= ImGuiTweaks.ColorEditWithDefault(Strings.ModuleTextColor, ref config.ModuleTextColor, KnownColor.White.Vector());
            configChanged |= ImGuiTweaks.ColorEditWithDefault(Strings.ModuleOutlineColor, ref config.ModuleOutlineColor, KnownColor.Orange.Vector());
        }
        
        return configChanged;
    }
}