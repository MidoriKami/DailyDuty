using System;
using DailyDuty.Classes;

namespace DailyDuty.Features.GrandCompanySquadron;

public class GrandCompanySquadronData : DataBase {
    public bool MissionCompleted;
    public bool MissionStarted;
    public DateTime MissionCompleteTime = DateTime.MinValue;
    public TimeSpan TimeUntilMissionComplete => MissionCompleteTime != DateTime.MinValue ? MissionCompleteTime - DateTime.UtcNow : TimeSpan.MinValue;
}
