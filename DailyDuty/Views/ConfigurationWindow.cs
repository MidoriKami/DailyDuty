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
using KamiLib.CommandManager;
using KamiLib.Components;
using KamiLib.Configuration;
using KamiLib.Extensions;
using KamiLib.Window;

namespace DailyDuty.Views;

public class ConfigurationWindow : TabbedSelectionWindow<BaseModule> {

    protected override string SelectionListTabName => "Modules";
    
    protected override List<ITabItem> Tabs { get; } = [
        new SystemConfigTab(),
    ];
    
    protected override List<BaseModule> Options => System.ModuleController.Modules;
    
    protected override float SelectionListWidth { get; set; } = 200.0f;
    
    protected override float SelectionItemHeight => ImGui.CalcTextSize("Butts").Y;

    protected override bool ShowListButton => true;

    protected override bool FilterOptions(BaseModule option)
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

    protected override void DrawListOption(BaseModule option) {
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

    protected override void DrawSelectedOption(BaseModule option) {
        using var table = ImRaii.Table($"module_table_{option.ModuleName}", 2, ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.BordersInnerV, ImGui.GetContentRegionAvail());
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

public class SystemConfigTab : ITabItem {
    public string Name => "System Config";
    public bool Disabled => false;
    public void Draw() {
        var configChanged = false;
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.TextUnformatted("System Options");
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);

        using var _ = ImRaii.PushIndent();
        configChanged |= ImGui.Checkbox(Strings.HideDisabled, ref System.SystemConfig.HideDisabledModules);

        if (configChanged) {
            System.SystemConfig.Save();
        }
    }
}