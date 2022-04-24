using System.Numerics;
using DailyDuty.Utilities;

namespace DailyDuty.Data.Components
{
    public class MainWindowSettings : GenericWindowSettings
    {
        public Vector4 TimerProgressBarColor = Colors.ForestGreen;
        public bool HideSeconds = false;
    }
}