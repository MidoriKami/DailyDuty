using System.Collections.Generic;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using KamiLib.Extensions;
using Lumina.Excel.Sheets;

namespace DailyDuty.Modules;

public class RaidsNormal : RaidsBase {
	public override ModuleName ModuleName => ModuleName.RaidsNormal;

	protected override List<ContentFinderCondition> RaidDuties { get; set; } = Service.DataManager.GetLimitedNormalRaidDuties().ToList();
	
	public override PayloadId ClickableLinkPayloadId => PayloadId.OpenDutyFinderRaid;

	protected override StatusMessage GetStatusMessage() => new LinkedStatusMessage {
			Message = $"{IncompleteTaskCount} {Strings.RaidsAvailable}", 
			LinkEnabled = Config.ClickableLink, 
			Payload = PayloadId.OpenDutyFinderRaid,
		};
}