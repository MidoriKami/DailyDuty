using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using Dalamud.Logging;
using ImGuiNET;
using KamiLib.InfoBoxSystem;
using KamiLib.Utilities;

namespace DailyDuty.Utilities;

public static class InfoBoxExtensions
{
    public static void DrawGenericSettings(this InfoBox instance, IConfigurationComponent component)
    {
        instance
            .AddTitle(Strings.Configuration.Options)
            .AddConfigCheckbox(Strings.Common.Enabled, component.ParentModule.GenericSettings.Enabled)
            .Draw();
    }

    public static void DrawGenericStatus(this InfoBox instance, IStatusComponent component)
    {
        var logicComponent = component.ParentModule.LogicComponent;
        var moduleStatus = logicComponent.Status();

        instance
            .AddTitle(Strings.Status.Label)
            .BeginTable()
            .BeginRow()
            .AddString(Strings.Status.ModuleStatus)
            .AddString(moduleStatus.GetTranslatedString(), moduleStatus.GetStatusColor())
            .EndRow()
            .EndTable()
            .Draw();
    }

    public static void DrawNotificationOptions(this InfoBox instance, IConfigurationComponent component)
    {
        instance
            .AddTitle(Strings.Configuration.NotificationOptions)
            .AddConfigCheckbox(Strings.Configuration.OnLogin, component.ParentModule.GenericSettings.NotifyOnLogin)
            .AddConfigCheckbox(Strings.Configuration.OnZoneChange, component.ParentModule.GenericSettings.NotifyOnZoneChange, Strings.Common.MessageTimeout)
            .Draw();
    }

    public static void DrawSuppressionOption(this InfoBox instance, IStatusComponent component)
    {
        instance
            .AddTitle(Strings.Status.Suppress, out var innerWidth)
            .AddStringCentered(Strings.Status.SuppressInfo, Colors.Orange)
            .AddDummy(20.0f)
            .AddStringCentered(Strings.Module.HuntMarks.NoUndo, Colors.Orange)
            .AddDisabledButton(Strings.Status.Snooze, () =>
            {
                PluginLog.Debug($"Suppressing, {component.ParentModule.Name.GetTranslatedString()}");
                component.ParentModule.GenericSettings.Suppressed.Value = true;
                Service.ConfigurationManager.Save();
            }, !(ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl), Strings.Module.Raids.RegenerateTooltip, innerWidth)
            .Draw();
    }
}