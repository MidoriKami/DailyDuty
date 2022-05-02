using DailyDuty.Data.Components;

namespace DailyDuty.Interfaces
{
    internal interface IZoneChangeThrottledNotification
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