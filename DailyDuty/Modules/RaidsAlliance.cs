using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using KamiLib.Extensions;

namespace DailyDuty.Modules;

public class RaidsAlliance : RaidsBase {
	public override ModuleName ModuleName => ModuleName.RaidsAlliance;
    
	protected override void UpdateTaskLists() {
		CheckForDutyListUpdate(Service.DataManager.GetLimitedAllianceDuties().ToList());
	}
	
	public override PayloadId ClickableLinkPayloadId => PayloadId.OpenDutyFinderAllianceRaid;

	protected override StatusMessage GetStatusMessage() {
		var message = $"{IncompleteTaskCount} {Strings.RaidsAvailable}";

		return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.OpenDutyFinderAllianceRaid);
	}
}