using System;
using System.Drawing;
using DailyDuty.Localization;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.Classes;

namespace DailyDuty.Modules.BaseModules;

public abstract class ModuleData {
	public DateTime NextReset;

	protected virtual void DrawModuleData() {
		ImGui.TextColored(KnownColor.Orange.Vector(), "No additional data for this module");
	}
    
	public void DrawDataUi() {
		ImGuiHelpers.ScaledDummy(10.0f);
		ImGui.TextUnformatted(Strings.ModuleData);
		ImGui.Separator();
		ImGuiHelpers.ScaledDummy(5.0f);

		using var _ = ImRaii.PushIndent();
		DrawModuleData();
	}

	protected void DrawDataTable(params (string Label, string Data)[] tableData) {
		using var table = ImRaii.Table("module_data_table", 2, ImGuiTableFlags.SizingStretchSame);
		if (!table) return;

		foreach (var (label, data) in tableData) {
			ImGuiTweaks.TableRow(label, data);
		}
	}
}