using KamiLib.Interfaces;

namespace DailyDuty.Views.Tabs;

public class TodoConfigurationTab : ITabItem
{
    public string TabName => "Todo Configuration";
    public bool Enabled => true;
    public void Draw()
    {
        DailyDutyPlugin.System.TodoController.DrawConfig();
    }
}