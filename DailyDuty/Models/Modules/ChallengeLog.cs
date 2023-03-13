using DailyDuty.Abstracts;
using DailyDuty.Interfaces;
using DailyDuty.Models.Enums;
using Dalamud.Game.Text;

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

    public override ModuleStatus GetModuleStatus()
    {
        return ModuleStatus.Unknown;
    }
    
    public override IStatusMessage GetStatusMessage() => new LinkedStatusMessage
    {
        Message = "Click Me!",
        MessageChannel = XivChatType.Party,
        SourceModule = ModuleName,
        Payload = PayloadId.OpenWondrousTailsBook,
    };
}