using System;
using System.Numerics;
using DailyDuty.Utilities;

namespace DailyDuty.Data.SettingsObjects
{
    public class CountdownTimerSettings
    {
        public int TimerWidth = 200;

        public bool DailyCountdownEnabled = true;
        public Vector4 DailyCountdownColor = Colors.Blue;
        public Vector4 DailyCountdownBgColor = Colors.Black;

        public bool WeeklyCountdownEnabled = true;
        public Vector4 WeeklyCountdownColor = Colors.Purple;
        public Vector4 WeeklyCountdownBgColor = Colors.Black;

        public bool FashionReportCountdownEnabled = false;
        public Vector4 FashionReportCountdownColor = Colors.ForestGreen;
        public Vector4 FashionReportCountdownBgColor = Colors.Black;

        public bool TreasureMapCountdownEnabled = false;
        public Vector4 TreasureMapCountdownColor = Colors.Blue;
        public Vector4 TreasureMapCountdownBgColor = Colors.Black;

        public bool JumboCactpotCountdownEnabled = false;
        public Vector4 JumboCactpotCountdownColor = Colors.Purple;
        public Vector4 JumboCactpotCountdownBgColor = Colors.Black;
    }
}