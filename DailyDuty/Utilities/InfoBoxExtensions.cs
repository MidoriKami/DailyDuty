using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using KamiLib.InfoBoxSystem;

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
        var moduleStatus = logicComponent.GetModuleStatus();

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
}