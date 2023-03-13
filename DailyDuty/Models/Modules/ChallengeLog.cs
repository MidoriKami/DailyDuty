using DailyDuty.Abstracts;
using DailyDuty.Models.Enums;

namespace DailyDuty.Models.Modules;

public class ChallengeLogConfig : ModuleConfigBase
{
    
}

public class ChallengeLogData : ModuleDataBase
{
    
}

public class ChallengeLog : Module.WeeklyModule
{
    public override ModuleDataBase ModuleData { get; protected set; } = new ChallengeLogData();
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new ChallengeLogConfig();
    public override ModuleName ModuleName => ModuleName.ChallengeLog;

    protected override ModuleStatus GetModuleStatus()
    {
        return ModuleStatus.Unavailable;
    }
    
    public override StatusMessage GetStatusMessage() => new LinkedStatusMessage
    {
        Message = "Click Me!",
        Payload = PayloadId.OpenWondrousTailsBook,
    };

    public override void ExtraConfig()
    {
        
    }
}