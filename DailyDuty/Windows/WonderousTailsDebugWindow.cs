using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.CommandManager;
using KamiLib.Window;
using Lumina.Excel.Sheets;

namespace DailyDuty.Windows;

public unsafe class WonderousTailsDebugWindow : Window {

	public WonderousTailsDebugWindow() : base("WondrousTails Debug", new Vector2(400.0f, 400.0f)) {
		System.CommandManager.RegisterCommand(new CommandHandler {
			Delegate = _ => UnCollapseOrShow(),
			ActivationPath = "/wt",
			Hidden = true,
		});
	}

	protected override void DrawContents() {
		foreach (var index in Enumerable.Range(0, 16)) {
			var taskId = PlayerState.Instance()->WeeklyBingoOrderData[index];
			
			var dutyListForSlot = WondrousTailsTaskResolver.GetTerritoriesFromOrderId(Service.DataManager, taskId);
			var bingoOrderData = Service.DataManager.GetExcelSheet<WeeklyBingoOrderData>().GetRow(taskId);

			if (ImGui.CollapsingHeader($"{taskId}: {bingoOrderData.Text.Value.Description}")) {
				foreach (var duty in dutyListForSlot) {
					var territoryType = Service.DataManager.GetExcelSheet<TerritoryType>().GetRow(duty);
					var cfc = territoryType.ContentFinderCondition.Value;
					ImGui.Text($"{territoryType.RowId}: {cfc.Name} - {cfc.ClassJobLevelRequired}");
				}
			}
		}
	}
}