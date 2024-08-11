﻿using System.Collections.Generic;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using KamiLib.Extensions;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Modules;

public class RaidsNormal : RaidsBase {
	public override ModuleName ModuleName => ModuleName.RaidsNormal;

	protected override List<ContentFinderCondition> RaidDuties { get; set; } = Service.DataManager.GetLimitedNormalRaidDuties().ToList();
	
	protected override void UpdateTaskLists() {
		CheckForDutyListUpdate(RaidDuties);
	}

	public override PayloadId ClickableLinkPayloadId => PayloadId.OpenDutyFinderRaid;
    
	protected override StatusMessage GetStatusMessage() {
		var message = $"{IncompleteTaskCount} {Strings.RaidsAvailable}";

		return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.OpenDutyFinderRaid);
	}
}