using DailyDuty.Data.Components;

namespace DailyDuty.Interfaces
{
    internal interface IZoneChangeAlwaysNotification
    {
        public GenericSettings GenericSettings { get; }

        public void TrySendNotification()
        {
            if (GenericSettings.ZoneChangeReminder && GenericSettings.Enabled)
            {
                SendNotification();
            }
        }

        public void SendNotification();
    }
}