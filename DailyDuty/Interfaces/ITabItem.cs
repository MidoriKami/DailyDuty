using DailyDuty.Enums;

namespace DailyDuty.Interfaces
{
    internal interface ITabItem
    {
        ModuleType ModuleType { get; }

        void DrawTabItem();

        void DrawConfigurationPane();
    }
}
