using DailyDuty.Data.SettingsObjects;

namespace DailyDuty.Interfaces
{
    internal interface ILoginNotification
    {
        public GenericSettings GenericSettings { get; }

        public void TrySendNotification()
        {
            if (GenericSettings.LoginReminder == true && GenericSettings.Enabled == true)
            {
                SendNotification();
            }
        }

        public void SendNotification();
    }
}