using DailyDuty.Interfaces;
using ImGuiNET;
using System;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.UserInterface.Components;

internal class StatusSelectable : ISelectable
{
    private ModuleName OwnerModuleName { get; }
    public IDrawable Contents { get; }
    public IModule ParentModule { get; }
    public string ID => OwnerModuleName.GetTranslatedString();

    private readonly Func<ModuleStatus> status;

    public StatusSelectable(IModule parentModule, IDrawable contents, Func<ModuleStatus> status)
    {
        OwnerModuleName = parentModule.Name;
        ParentModule = parentModule;
        Contents = contents;
        this.status = status;
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

        var color = status.Invoke().GetStatusColor();
        var text = status.Invoke().GetTranslatedString();

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