using System;
using System.Diagnostics;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace DailyDuty.Utilities;

internal static class Time
{
    public static DateTime NextDailyReset()
    {
        var now = Now();
            
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
        var today = Now();
            
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

    public static DateTime NextDayOfWeek(DayOfWeek weekday)
    {
        var now = Now();

        while (now.DayOfWeek != weekday)
        {
            now = now.AddDays(1);
        }

        return now.Date;
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

    public static DateTime Now()
    {
        unsafe
        {
            var framework = Framework.Instance();

            var time = new TimeStamp(framework->ServerTime * 1000).DateTime;

            return time;
        }
    }
}