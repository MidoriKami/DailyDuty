using DailyDuty.Data.Components;

namespace DailyDuty.Interfaces
{
    internal interface ILoginNotification
    {
        public GenericSettings GenericSettings { get; }

        public void TrySendNotification()
        {
            if (GenericSettings.LoginReminder && GenericSettings.Enabled)
            {
                SendNotification();
            }
        }

        public void SendNotification();
    }
}