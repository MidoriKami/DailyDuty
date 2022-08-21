using DailyDuty.Interfaces;
using DailyDuty.Modules.Enums;
using DailyDuty.Utilities;
using ImGuiNET;
using System;
using DailyDuty.System.Localization;

namespace DailyDuty.UserInterface.Components;

internal class StatusSelectable : ISelectable
{
    public ModuleName OwnerModuleName { get; }
    public IDrawable Contents { get; }
    public IModule ParentModule { get; }

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
        ImGui.Text(OwnerModuleName.GetLocalizedString()[..Math.Min(OwnerModuleName.GetLocalizedString().Length, 20)]);
    }

    private void DrawModuleStatus()
    {
        var region = ImGui.GetContentRegionAvail();
        
        var color = status.Invoke() switch
        {
            ModuleStatus.Unknown => Colors.Grey,
            ModuleStatus.Incomplete => Colors.Red,
            ModuleStatus.Unavailable => Colors.Orange,
            ModuleStatus.Complete => Colors.Green,
            _ => throw new ArgumentOutOfRangeException()
        };

        var text = status.Invoke().GetLocalizedString();

        // Override Status if Module is Disabled
        if (!ParentModule.GenericSettings.Enabled.Value)
        {
            text = Strings.Common.Disabled;
            color = Colors.Grey;
        }

        var textSize = ImGui.CalcTextSize(text);

        ImGui.SameLine(region.X - textSize.X + 3.0f);
        ImGui.TextColored(color, text);
    }
}