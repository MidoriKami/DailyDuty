using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using KamiLib.Utilities;

namespace DailyDuty.System;

public class RaidsNormal : RaidsBase
{
    public override ModuleName ModuleName => ModuleName.RaidsNormal;

    protected override void UpdateTaskLists() => CheckForDutyListUpdate(DutyLists.Instance.LimitedSavage);

    public override bool HasClickableLink => true;
    public override PayloadId ClickableLinkPayloadId => PayloadId.OpenDutyFinderRaid;
    
    protected override StatusMessage GetStatusMessage()
    {
        var message = $"{GetIncompleteCount(Config.TaskConfig, Data.TaskData)} {Strings.RaidsAvailable}";

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.OpenDutyFinderRaid);
    }
}