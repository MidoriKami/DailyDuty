using System;
using System.Collections.Generic;
using DailyDuty.Localization;
using DailyDuty.Timers;
using DailyDuty.Utilities;

namespace DailyDuty.System
{
    public class TimerManager : IDisposable
    {
        public readonly List<CountdownTimer> Timers = new()
        {
            new CountdownTimer()
            {
                Label = Strings.Timers.DailyResetLabel,
                Period = TimeSpan.FromDays(1),
                UpdateNextReset = Time.NextDailyReset
            },
            new CountdownTimer()
            {
                Label = Strings.Timers.FashionReportLabel,
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
                }
            },
            new CountdownTimer()
            {
                Label = Strings.Timers.JumboCactpotLabel,
                Period = TimeSpan.FromDays(7),
                UpdateNextReset = Time.NextJumboCactpotReset
            },
            new CountdownTimer()
            {
                Label = Strings.Timers.LeveAllowanceLabel,
                Period = TimeSpan.FromDays(1),
                UpdateNextReset = Time.NextLeveAllowanceReset
            },
            new CountdownTimer()
            {
                Label = Strings.Timers.TreasureMapLabel,
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

                }
            },
            new CountdownTimer()
            {
                Label = Strings.Timers.WeeklyResetLabel,
                Period = TimeSpan.FromDays(7),
                UpdateNextReset = Time.NextWeeklyReset
            }
        };

        public void Dispose()
        {
        }
    }
}
