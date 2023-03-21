using DailyDuty.Models;
using DailyDuty.Models.Enums;
using KamiLib.Misc;

namespace DailyDuty.System;

public class RaidsNormal : RaidsBase
{
    public override ModuleName ModuleName => ModuleName.RaidsNormal;

    public override void Load()
    {
        base.Load();
        
        CheckForDutyListUpdate(DutyLists.Instance.LimitedSavage);
    }

    protected override StatusMessage GetStatusMessage()
    {
        var message = $"{GetIncompleteCount(Config.Tasks, Data.Tasks)} Raids Available";

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.OpenDutyFinderRaid);
    }
}