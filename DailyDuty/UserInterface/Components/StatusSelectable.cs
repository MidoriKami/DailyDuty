using DailyDuty.Interfaces;
using ImGuiNET;
using System;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using KamiLib.Drawing;
using KamiLib.Interfaces;

namespace DailyDuty.UserInterface.Components;

internal class StatusSelectable : ISelectable, IDrawable
{
    public IDrawable Contents => this;
    
    private readonly IStatusComponent statusComponent;
    private ModuleName OwnerModuleName => statusComponent.ParentModule.Name;
    public IModule ParentModule => statusComponent.ParentModule;
    public string ID => OwnerModuleName.GetTranslatedString();

    private readonly Func<ModuleStatus> getStatus;

    public StatusSelectable(IStatusComponent parentModule)
    {
        statusComponent = parentModule;
        getStatus = parentModule.ParentModule.LogicComponent.GetModuleStatus;
    }

    public void Draw() => statusComponent.DrawStatus();

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

        var color = getStatus.Invoke().GetStatusColor();
        var text = getStatus.Invoke().GetTranslatedString();

        // Override Status if Module is Disabled
        if (!ParentModule.GenericSettings.Enabled)
        {
            text = Strings.Common_Disabled;
            color = Colors.Grey;
        }

        var textSize = ImGui.CalcTextSize(text);

        ImGui.SameLine(region.X - textSize.X + 3.0f);
        ImGui.TextColored(color, text);
    }
}