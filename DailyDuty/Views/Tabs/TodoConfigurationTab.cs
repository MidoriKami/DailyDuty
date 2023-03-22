using DailyDuty.System.Localization;
using KamiLib.Interfaces;

namespace DailyDuty.Views.Tabs;

public class TodoConfigurationTab : ITabItem
{
    public string TabName => Strings.TodoConfiguration;
    public bool Enabled => true;
    public void Draw()
    {
        DailyDutyPlugin.System.TodoController.DrawConfig();
    }
}