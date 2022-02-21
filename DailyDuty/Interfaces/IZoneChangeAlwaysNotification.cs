using DailyDuty.Data.SettingsObjects;

namespace DailyDuty.Interfaces;

internal interface IZoneChangeAlwaysNotification
{
    public GenericSettings GenericSettings { get; }

    public void TrySendNotification()
    {
        if (GenericSettings.ZoneChangeReminder == true && GenericSettings.Enabled == true)
        {
            SendNotification();
        }
    }

    public void SendNotification();
}