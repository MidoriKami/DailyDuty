using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Data.SettingsObjects.WindowSettings;

internal class TodoWindowSettings : GenericWindowSettings
{
    public bool ShowDaily = true;
    public bool ShowWeekly = true;
    public bool HideWhenTasksComplete = false;
}