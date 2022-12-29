using System.Numerics;
using DailyDuty.Configuration;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.UserInterface.Components;

internal class HideDisabledCheckbox : IDrawable
{
    private CharacterConfiguration Config => Service.ConfigurationManager.CharacterConfiguration;

    public void Draw()
    {
        var region = ImGui.GetContentRegionAvail();

        var label = Config.HideDisabledModulesInSelectWindow ? Strings.Configuration.ShowDisabled : Strings.Configuration.HideDisabled;

        if (ImGui.Button(label, region with { Y = 23.0f * ImGuiHelpers.GlobalScale }))
        {
            Config.HideDisabledModulesInSelectWindow = !Config.HideDisabledModulesInSelectWindow;
            Service.ConfigurationManager.Save();
        }
    }
}