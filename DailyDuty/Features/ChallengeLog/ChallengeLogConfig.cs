using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.ChallengeLog;

public class ChallengeLogConfig : ModuleConfig<ChallengeLogConfig> {
    protected override string FileName => "ChallengeLog";
	
	public bool EnableContentFinderWarning = true;
	public bool EnableWarningSound = true;
    
	public HashSet<uint> WarningEntries = [];
    public HashSet<uint> TrackedEntries = [];
}
