// using System.Collections.Generic;
// using System.Linq;
// using DailyDuty.Classes;
// using DailyDuty.Models;
// using DailyDuty.Modules.BaseModules;
// using KamiLib.Extensions;
// using Lumina.Excel.Sheets;
//
// namespace DailyDuty.Modules;
//
// public class RaidsAlliance : RaidsBase {
// 	public override ModuleName ModuleName => ModuleName.RaidsAlliance;
//
// 	protected override List<ContentFinderCondition> RaidDuties { get; set; } = Services.DataManager.GetLimitedAllianceRaidDuties().ToList();
//
// 	public override PayloadId ClickableLinkPayloadId => PayloadId.OpenDutyFinderAllianceRaid;
//
// 	protected override StatusMessage GetStatusMessage() => new LinkedStatusMessage {
// 		LinkEnabled = Config.ClickableLink,
// 		Message = $"{IncompleteTaskCount} Raids Available",
// 		Payload = PayloadId.OpenDutyFinderAllianceRaid,
// 	};
// }
