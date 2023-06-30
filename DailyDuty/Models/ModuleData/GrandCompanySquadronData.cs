using System;
using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.ModuleData;

[Category("ModuleData", 1)]
public interface IGrandCompanySquadronModuleData
{
    [BoolDisplay("MissionCompleted")]
    public bool MissionCompleted { get; set; }

    [BoolDisplay("MissionStarted")]
    public bool MissionStarted { get; set; }
    
    [LocalDateTimeDisplay("MissionCompleteTime")]
    public DateTime MissionCompleteTime { get; set; }
    
    [TimeSpanDisplay("TimeUntilMissionComplete")]
    public TimeSpan TimeUntilMissionComplete { get; set; }
}

public class GrandCompanySquadronData : IModuleDataBase, IGrandCompanySquadronModuleData
{
    public DateTime NextReset { get; set; } = DateTime.MinValue;
    
    public bool MissionCompleted { get; set; }
    public bool MissionStarted { get; set; }
    public DateTime MissionCompleteTime { get; set; } = DateTime.MinValue;
    public TimeSpan TimeUntilMissionComplete { get; set; } = TimeSpan.MinValue;
}