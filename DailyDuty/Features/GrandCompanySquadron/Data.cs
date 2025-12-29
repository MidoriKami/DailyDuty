using System;
using DailyDuty.Classes;

namespace DailyDuty.Features.GrandCompanySquadron;

public class Data : DataBase {
    	public bool MissionCompleted;
    	public bool MissionStarted;
    	public DateTime MissionCompleteTime = DateTime.MinValue;
    	public TimeSpan TimeUntilMissionComplete = TimeSpan.MinValue;
}
