using System;
using System.Diagnostics;

namespace DailyDuty.Utilities
{
    internal static class Time
    {
        public static DateTime NextDailyReset()
        {
            var now = DateTime.UtcNow;
            
            if( now.Hour < 15 )
            {
                return now.Date.AddHours(15);   
            }
            else
            {
                return now.AddDays(1).Date.AddHours(15);
            }
        }

        public static DateTime NextWeeklyReset()
        {
            var today = DateTime.UtcNow;
            
            if(today.Hour < 8 && today.DayOfWeek == DayOfWeek.Tuesday)
            {
                return today.Date.AddHours(8);
            }
            else
            {
                var nextReset = today.AddDays(1);

                while (nextReset.DayOfWeek != DayOfWeek.Tuesday)
                {
                    nextReset = nextReset.AddDays(1);
                }
                
                return nextReset.Date.AddHours(8);
            }
        }

        public static DateTime NextFashionReportReset()
        {
            return NextWeeklyReset().AddDays(-4);
        }

        public static DateTime NextLeveAllowanceReset()
        {
            var now = DateTime.UtcNow;
            
            if( now.Hour < 12 )
            {
                return now.Date.AddHours(12);   
            }
            else
            {
                return now.AddDays(1).Date.AddHours(12);
            }
        }

        public static DateTime NextDayOfWeek(DayOfWeek weekday)
        {
            var now = DateTime.UtcNow;

            do
            {
                now = now.AddDays(1);

            } while (now.DayOfWeek != weekday);

            return now.Date;
        }

        public static DateTime NextDayOfWeek(DayOfWeek weekday, int hour)
        {
            var today = DateTime.UtcNow;
            
            if(today.Hour < hour && today.DayOfWeek == weekday)
            {
                return today.Date.AddHours(hour);
            }
            else
            {
                var nextReset = today.AddDays(1);

                while (nextReset.DayOfWeek != weekday)
                {
                    nextReset = nextReset.AddDays(1);
                }
                
                return nextReset.Date.AddHours(hour);
            }
        }

        public static void UpdateDelayed(Stopwatch stopwatch, TimeSpan delayTime, Action function)
        {
            if (stopwatch.IsRunning && stopwatch.Elapsed >= delayTime)
            {
                stopwatch.Stop();
                stopwatch.Reset();
            }

            if (stopwatch.IsRunning == false)
            {
                stopwatch.Start();
                function();
            }
        }
    }
}