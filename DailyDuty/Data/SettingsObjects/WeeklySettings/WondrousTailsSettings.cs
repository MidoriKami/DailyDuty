using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Data.SettingsObjects.WeeklySettings;

public class WondrousTailsSettings : GenericSettings
{
    public int NumPlacedStickers = 0;
    public bool InstanceStartNotification = false;
    public bool InstanceEndNotification = false;
    public bool RerollNotificationStickers = false;
    public bool RerollNotificationTasks = false;
    public bool NewBookNotification = false;
    public DateTime CompletionDate = new();
    public bool BookCompleteNotification = false;
}