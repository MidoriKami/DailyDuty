using DailyDuty.Interfaces;
using DailyDuty.Modules.Enums;
using DailyDuty.Utilities;
using ImGuiNET;
using System;
using DailyDuty.Configuration.Components;
using DailyDuty.System.Localization;

namespace DailyDuty.UserInterface.Components;

internal class StatusSelectable : ISelectable
{
    public ModuleName OwnerModuleName { get; }
    public IDrawable Contents { get; }

    private readonly Func<ModuleStatus> status;
    private readonly Setting<bool> enabled;

    public StatusSelectable(ModuleName ownerModuleName, IDrawable contents, Setting<bool> enabled, Func<ModuleStatus> status)
    {
        this.OwnerModuleName = ownerModuleName;
        Contents = contents;
        this.enabled = enabled;
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
        if (!enabled.Value)
        {
            text = Strings.Common.Disabled;
            color = Colors.Grey;
        }

        var textSize = ImGui.CalcTextSize(text);

        ImGui.SameLine(region.X - textSize.X + 3.0f);
        ImGui.TextColored(color, text);
    }
}