using System;
using DailyDuty.Models.Enums;
using KamiLib.Misc;

namespace DailyDuty.Abstracts;

public static class Module
{
    public abstract class DailyModule : BaseModule
    {
        public override ModuleType ModuleType => ModuleType.Daily;
        public override TimeSpan GetResetPeriod() => TimeSpan.FromDays(1);
        public override DateTime GetNextReset() => Time.NextDailyReset();
    }

    public abstract class WeeklyModule : BaseModule
    {
        public override ModuleType ModuleType => ModuleType.Weekly;
        public override TimeSpan GetResetPeriod() => TimeSpan.FromDays(1);
        public override DateTime GetNextReset() => Time.NextWeeklyReset();
    }

    public abstract class SpecialModule : BaseModule
    {
        public override ModuleType ModuleType => ModuleType.Special;
    }
}

