using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.ChallengeLog;

public class Config : ConfigBase {
	public bool EnableContentFinderWarning = true;
	public bool EnableWarningSound = true;
    
	public HashSet<uint> WarningEntries = [];
    public HashSet<uint> TrackedEntries = [];
}
