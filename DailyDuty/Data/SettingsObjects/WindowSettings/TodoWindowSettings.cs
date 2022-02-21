using System.Numerics;
using DailyDuty.Utilities;

namespace DailyDuty.Data.SettingsObjects.WindowSettings;

public class TodoWindowSettings : GenericWindowSettings
{
    public bool ShowDaily = true;
    public bool ShowWeekly = true;
    public bool HideWhenTasksComplete = false;
    public bool ShowTasksWhenComplete = false;

    public Vector4 HeaderColor = Colors.White;
    public Vector4 IncompleteColor = Colors.Red;
    public Vector4 CompleteColor = Colors.Green;
}