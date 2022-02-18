using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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