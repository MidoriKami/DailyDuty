using DailyDuty.Interfaces;
using ImGuiNET;
using System;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.UserInterface.Components;

internal class ConfigurationSelectable : ISelectable
{
    private ModuleName OwnerModuleName { get; }
    public IDrawable Contents { get; }
    private IModule ParentModule { get; }
    public string ID => OwnerModuleName.GetTranslatedString();

    public ConfigurationSelectable(IModule parentModule, IDrawable contents)
    {
        OwnerModuleName = parentModule.Name;
        ParentModule = parentModule;
        Contents = contents;
    }

    public void DrawLabel()
    {
        DrawModuleLabel();
        DrawModuleStatus();
    }

    private void DrawModuleLabel()
    {
        ImGui.Text(OwnerModuleName.GetTranslatedString()[..Math.Min(OwnerModuleName.GetTranslatedString().Length, 28)]);
    }

    private void DrawModuleStatus()
    {
        var region = ImGui.GetContentRegionAvail();

        var text = ParentModule.GenericSettings.Enabled ? Strings.Common_Enabled : Strings.Common_Disabled;
        var color = ParentModule.GenericSettings.Enabled ? Colors.Green : Colors.Red;

        var textSize = ImGui.CalcTextSize(text);

        ImGui.SameLine(region.X - textSize.X + 3.0f);
        ImGui.TextColored(color, text);
    }
}