using System.Collections.Generic;
using System.Linq;
using DailyDuty.Enums;
using Dalamud.Game;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;

namespace DailyDuty.Extensions;

public static class DataManagerExtensions {
	extension(IDataManager dataManager) {
		public IEnumerable<ContentFinderCondition> SavageDuties
			=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English).Where(cfc => GetDutyType(cfc) is DutyType.Savage);

		public IEnumerable<ContentFinderCondition> UltimateDuties
			=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English).Where(cfc => GetDutyType(cfc) is DutyType.Ultimate);

		public IEnumerable<ContentFinderCondition> ExtremeDuties
			=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English).Where(cfc => GetDutyType(cfc) is DutyType.Extreme);

		public IEnumerable<ContentFinderCondition> UnrealDuties
			=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English).Where(cfc => GetDutyType(cfc) is DutyType.Unreal);

		public IEnumerable<ContentFinderCondition> CriterionDuties
			=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English).Where(cfc => GetDutyType(cfc) is DutyType.Criterion);

		public IEnumerable<ContentFinderCondition> AllianceDuties
			=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English).Where(cfc => GetDutyType(cfc) is DutyType.Alliance);

		/// <summary> Warning, expensive operation, as this has to cross-reference multiple data sets. </summary>
		public IEnumerable<ContentFinderCondition> LimitedAllianceRaidDuties
			=> dataManager.LimitedDuties.Where(cfc => GetDutyType(cfc) is DutyType.Alliance);

		/// <summary> Warning, expensive operation, as this has to cross-reference multiple data sets. </summary>
		public IEnumerable<ContentFinderCondition> LimitedSavageRaidDuties
			=> dataManager.LimitedDuties.Where(cfc => GetDutyType(cfc) is DutyType.Savage);

		/// <summary> Warning, expensive operation, as this has to cross-reference multiple data sets. </summary>
		public IEnumerable<ContentFinderCondition> LimitedNormalRaidDuties
			=> dataManager.LimitedDuties.Where(cfc => GetDutyType(cfc) is DutyType.NormalRaid);

		/// <summary> Warning, expensive operation, as this has to cross-reference multiple data sets. </summary>
		private IEnumerable<ContentFinderCondition> LimitedDuties
			=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)
			              .Where(cfc => dataManager.GetExcelSheet<InstanceContent>()
			                                       .Where(instanceContent => instanceContent is { WeekRestriction: 1 })
			                                       .Select(instanceContent => instanceContent.RowId)
			                                       .Contains(cfc.Content.RowId));

		public DutyType GetDutyType(ContentFinderCondition cfc)
			=> GetDutyType(dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English).GetRow(cfc.RowId));

		public unsafe DutyType CurrentDutyType => dataManager
			.GetDutyType(dataManager.GetExcelSheet<ContentFinderCondition>().GetRow(GameMain.Instance()->CurrentContentFinderConditionId));
	}
	
	private static DutyType GetDutyType(ContentFinderCondition cfc)
		=> cfc switch {
			{ ContentType.RowId: 5, ContentMemberType.RowId: 4 } => DutyType.Alliance,
			{ ContentType.RowId: 37 } => DutyType.ChaoticAlliance,
			{ ContentType.RowId: 5 } when cfc.Name.ToString().Contains("Savage") => DutyType.Savage,
			{ ContentType.RowId: 5 } when !cfc.Name.ToString().Contains("Savage") => DutyType.NormalRaid,
			{ ContentType.RowId: 28 } => DutyType.Ultimate,
			{ ContentType.RowId: 4 } when cfc.Name.ToString().Contains("Extreme") || cfc.Name.ToString().Contains("Minstrel") => DutyType.Extreme,
			{ ContentType.RowId: 4 } => DutyType.Unreal,
			{ ContentType.RowId: 30, AllowUndersized: false } => DutyType.Criterion,
			_ => DutyType.Unknown,
		};
}
