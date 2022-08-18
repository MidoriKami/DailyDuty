using DailyDuty.Interfaces;
using DailyDuty.Modules.Enums;
using DailyDuty.System.Localization;
using DailyDuty.Utilities;
using ImGuiNET;
using System;
using DailyDuty.Configuration.Common;

namespace DailyDuty.UserInterface.Components;

internal class ConfigurationSelectable : ISelectable
{
    public ModuleName OwnerModuleName { get; }
    public IDrawable Contents { get; }

    private readonly Setting<bool> enabled;

    public ConfigurationSelectable(ModuleName ownerModuleName, IDrawable contents, Setting<bool> enabled)
    {
        this.OwnerModuleName = ownerModuleName;
        Contents = contents;
        this.enabled = enabled;
    }

    public void DrawLabel()
    {
        DrawModuleLabel();
        DrawModuleStatus();
    }

    private void DrawModuleLabel()
    {
        ImGui.Text(OwnerModuleName.ToString()[..Math.Min(OwnerModuleName.ToString().Length, 20)]);
    }

    private void DrawModuleStatus()
    {
        var region = ImGui.GetContentRegionAvail();

        var text = enabled.Value ? Strings.Common.Enabled : Strings.Common.Disabled;
        var color = enabled.Value ? Colors.Green : Colors.Red;

        var textSize = ImGui.CalcTextSize(text);

        ImGui.SameLine(region.X - textSize.X + 3.0f);
        ImGui.TextColored(color, text);
    }
}