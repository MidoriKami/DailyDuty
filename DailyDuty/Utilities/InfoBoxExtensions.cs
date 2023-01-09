using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using Dalamud.Logging;
using ImGuiNET;
using KamiLib.Drawing;

namespace DailyDuty.Utilities;

public static class InfoBoxExtensions
{
    public static void DrawGenericSettings(this InfoBox instance, IConfigurationComponent component)
    {
        instance
            .AddTitle(Strings.Config_Options)
            .AddConfigCheckbox(Strings.Common_Enabled, component.ParentModule.GenericSettings.Enabled)
            .Draw();
    }

    public static void DrawGenericStatus(this InfoBox instance, IStatusComponent component)
    {
        var logicComponent = component.ParentModule.LogicComponent;
        var moduleStatus = logicComponent.Status();

        instance
            .AddTitle(Strings.Status_Label)
            .BeginTable()
            .BeginRow()
            .AddString(Strings.Status_ModuleStatus)
            .AddString(moduleStatus.GetTranslatedString(), moduleStatus.GetStatusColor())
            .EndRow()
            .EndTable()
            .Draw();
    }

    public static void DrawNotificationOptions(this InfoBox instance, IConfigurationComponent component)
    {
        instance
            .AddTitle(Strings.Config_NotificationOptions)
            .AddConfigCheckbox(Strings.Config_LoginNotification, component.ParentModule.GenericSettings.NotifyOnLogin)
            .AddConfigCheckbox(Strings.Config_ZoneChageNotification, component.ParentModule.GenericSettings.NotifyOnZoneChange, Strings.Common_MessageTimeout_Info)
            .Draw();
    }

    public static void DrawSuppressionOption(this InfoBox instance, IStatusComponent component)
    {
        instance
            .AddTitle(Strings.Status_Suppression, out var innerWidth)
            .AddStringCentered(Strings.Status_Suppression_Info, innerWidth, Colors.Orange)
            .AddDummy(10.0f)
            .AddDisabledButton(component.ParentModule.GenericSettings.Suppressed ? Strings.Status_Unsnooze : Strings.Status_Snooze, () =>
            {
                if (!component.ParentModule.GenericSettings.Suppressed)
                {
                    PluginLog.Debug($"Snoozing, {component.ParentModule.Name.GetTranslatedString()}");
                    component.ParentModule.GenericSettings.Suppressed.Value = true;
                }
                else
                {
                    PluginLog.Debug($"UnSnoozing, {component.ParentModule.Name.GetTranslatedString()}");
                    component.ParentModule.GenericSettings.Suppressed.Value = false;
                }
                Service.ConfigurationManager.Save();
            }, !(ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl), Strings.DisabledButton_Hover, innerWidth)
            .Draw();
    }
}