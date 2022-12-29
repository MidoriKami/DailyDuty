using System;
using System.Linq;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using Dalamud.Utility;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Utilities;

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
        return NextDayOfWeek(DayOfWeek.Tuesday, 8);
    }

    public static DateTime NextFashionReportReset()
    {
        return NextWeeklyReset().AddDays(-4);
    }

    public static DateTime NextGrandCompanyReset()
    {
        var now = DateTime.UtcNow;
        var targetHour = 20;    
        
        if( now.Hour < targetHour )
        {
            return now.Date.AddHours(targetHour);
        }
        else
        {
            return now.AddDays(1).Date.AddHours(targetHour);
        }
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
            return now.Date.AddDays(1);
        }
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

    public static DateTime NextJumboCactpotReset()
    {
        var region = LookupDatacenterRegion(Service.ClientState.LocalPlayer?.HomeWorld.GameData?.DataCenter.Row);

        return region switch
        {
            // Japan
            1 => NextDayOfWeek(DayOfWeek.Saturday, 12),

            // North America
            2 => NextDayOfWeek(DayOfWeek.Sunday, 2),

            // Europe
            3 => NextDayOfWeek(DayOfWeek.Saturday, 19),

            // Australia
            4 => NextDayOfWeek(DayOfWeek.Saturday, 9),

            // Unknown Region
            _ => DateTime.MinValue
        };
    }

    private static byte LookupDatacenterRegion(uint? playerDatacenterID)
    {
        if (playerDatacenterID == null) return 0;

        return LuminaCache<WorldDCGroupType>.Instance.GetAll()
            .Where(world => world.RowId == playerDatacenterID.Value)
            .Select(dc => dc.Region)
            .FirstOrDefault();
    }
    
    public static string FormatTimespan(TimeSpan span, TimerStyle style)
    {
        return style switch
        {
            // Human Style just shows the highest order nonzero field.
            TimerStyle.Human when span.Days > 1 => Strings.UserInterface.Timers.NumDays.Format(span.Days),
            TimerStyle.Human when span.Days == 1 => Strings.UserInterface.Timers.DayPlusHours.Format(span.Days, span.Hours),
            TimerStyle.Human when span.Hours > 1 => Strings.UserInterface.Timers.NumHours.Format(span.Hours),
            TimerStyle.Human when span.Minutes >= 1 => Strings.UserInterface.Timers.NumMins.Format(span.Minutes),
            TimerStyle.Human => Strings.UserInterface.Timers.NumSecs.Format(span.Seconds),

            TimerStyle.Full => $"{(span.Days >= 1 ? $"{span.Days}." : "")}{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}",
            TimerStyle.NoSeconds => $"{(span.Days >= 1 ? $"{span.Days}." : "")}{span.Hours:D2}:{span.Minutes:D2}",
            _ => string.Empty
        };
    }
}