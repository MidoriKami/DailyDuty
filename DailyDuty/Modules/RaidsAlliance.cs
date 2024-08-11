using System.Collections.Generic;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using KamiLib.Extensions;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Modules;

public class RaidsAlliance : RaidsBase {
	public override ModuleName ModuleName => ModuleName.RaidsAlliance;

	protected override List<ContentFinderCondition> RaidDuties { get; set; } = Service.DataManager.GetLimitedAllianceRaidDuties().ToList();

	protected override void UpdateTaskLists() {
		CheckForDutyListUpdate(RaidDuties);
	}
	
	public override PayloadId ClickableLinkPayloadId => PayloadId.OpenDutyFinderAllianceRaid;

	protected override StatusMessage GetStatusMessage() {
		var message = $"{IncompleteTaskCount} {Strings.RaidsAvailable}";

		return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.OpenDutyFinderAllianceRaid);
	}
}