using DailyDuty.Data.Components;

namespace DailyDuty.Interfaces
{
    internal interface IZoneChangeAlwaysNotification
    {
        public GenericSettings GenericSettings { get; }

        public void TrySendNotification(ushort newTerritory)
        {
            if (GenericSettings.ZoneChangeReminder && GenericSettings.Enabled)
            {
                SendNotification(newTerritory);
            }
        }

        public void SendNotification(ushort newTerritory);
    }
}