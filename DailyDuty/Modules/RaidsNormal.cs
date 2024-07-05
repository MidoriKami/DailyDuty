using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using KamiLib.Extensions;

namespace DailyDuty.Modules;

public class RaidsNormal : RaidsBase {
	public override ModuleName ModuleName => ModuleName.RaidsNormal;

	protected override void UpdateTaskLists() {
		CheckForDutyListUpdate(Service.DataManager.GetLimitedSavageDuties().ToList());
	}

	public override PayloadId ClickableLinkPayloadId => PayloadId.OpenDutyFinderRaid;
    
	protected override StatusMessage GetStatusMessage() {
		var message = $"{IncompleteTaskCount} {Strings.RaidsAvailable}";

		return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.OpenDutyFinderRaid);
	}
}