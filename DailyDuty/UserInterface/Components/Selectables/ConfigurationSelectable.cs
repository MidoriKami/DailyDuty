using DailyDuty.Interfaces;
using ImGuiNET;
using System;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using KamiLib.Drawing;
using KamiLib.Interfaces;

namespace DailyDuty.UserInterface.Components;

internal class ConfigurationSelectable : ISelectable, IDrawable
{
    public IDrawable Contents => this;
    
    private readonly IConfigurationComponent configurationComponent;
    private ModuleName OwnerModuleName => configurationComponent.ParentModule.Name;
    private IModule ParentModule => configurationComponent.ParentModule;
    public string ID => OwnerModuleName.GetTranslatedString();

    public ConfigurationSelectable(IConfigurationComponent parentModule)
    {
        configurationComponent = parentModule;
    }
    
    public void Draw() => configurationComponent.DrawConfiguration();

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