using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.SettingsObjects;

namespace DailyDuty.Interfaces
{
    internal interface ILoginNotification
    {
        public GenericSettings GenericSettings { get; }

        public void TrySendNotification()
        {
            if (GenericSettings.LoginReminder == true)
            {
                SendNotification();
            }
        }

        public void SendNotification();
    }
}
