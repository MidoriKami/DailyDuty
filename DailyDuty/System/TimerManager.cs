using System;
using System.Collections.Generic;
using DailyDuty.Data.Components;
using DailyDuty.Localization;
using DailyDuty.Timers;
using DailyDuty.Utilities;

namespace DailyDuty.System
{
    public class TimerManager : IDisposable
    {
        private static TimersSettings Settings => Service.SystemConfiguration.Timers;

        public readonly List<CountdownTimer> Timers = new()
        {
            new CountdownTimer()
            {
                Label = Strings.Timers.DailyResetLabel,
                ShortLabel = Strings.Timers.DailyResetShortLabel,
                Period = TimeSpan.FromDays(1),
                UpdateNextReset = Time.NextDailyReset,
                TimerSettings = Settings.Daily
            },
            new CountdownTimer()
            {
                Label = Strings.Timers.FashionReportLabel,
                ShortLabel = Strings.Timers.FashionReportShortLabel,
                Period = TimeSpan.FromDays(7),
                UpdateNextReset = () =>
                {
                    var now = DateTime.UtcNow;

                    var fashionReportOpen = Time.NextFashionReportReset();
                    var fashionReportClose = Time.NextWeeklyReset();

                    if (now > fashionReportOpen && now < fashionReportClose)
                    {
                        return Time.NextWeeklyReset().AddDays(3);
                    }
                    else
                    {
                        return Time.NextFashionReportReset();
                    }
                },
                TimerSettings = Settings.FashionReport
            },
            new CountdownTimer()
            {
                Label = Strings.Timers.JumboCactpotLabel,
                ShortLabel = Strings.Timers.JumboCactpotShortLabel,
                Period = TimeSpan.FromDays(7),
                UpdateNextReset = Time.NextJumboCactpotReset,
                TimerSettings = Settings.JumboCactpot,
            },
            new CountdownTimer()
            {
                Label = Strings.Timers.LeveAllowanceLabel,
                ShortLabel = Strings.Timers.LeveAllowanceShortLabel,
                Period = TimeSpan.FromDays(1),
                UpdateNextReset = Time.NextLeveAllowanceReset,
                TimerSettings = Settings.LeveAllowance,
            },
            new CountdownTimer()
            {
                Label = Strings.Timers.TreasureMapLabel,
                ShortLabel = Strings.Timers.TreasureMapShortLabel,
                Period = TimeSpan.FromHours(18),
                UpdateNextReset = () =>
                {
                    var harvestTime = Service.CharacterConfiguration.TreasureMap.LastMapGathered;
                    var nextAvailableTime = harvestTime.AddHours(18);

                    var remainingTime = nextAvailableTime - DateTime.UtcNow;

                    if (remainingTime < TimeSpan.Zero)
                    {
                        return DateTime.UtcNow;
                    }
                    else
                    {
                        return nextAvailableTime;
                    }
                },
                TimerSettings = Settings.TreasureMap,
            },
            new CountdownTimer()
            {
                Label = Strings.Timers.WeeklyResetLabel,
                ShortLabel = Strings.Timers.WeeklyResetShortLabel,
                Period = TimeSpan.FromDays(7),
                UpdateNextReset = Time.NextWeeklyReset,
                TimerSettings = Settings.Weekly
            }
        };

        public void Dispose()
        {
        }
    }
}
