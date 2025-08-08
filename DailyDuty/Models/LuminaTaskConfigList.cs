using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Lumina.Excel;
using DailyDuty.Localization;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Lumina.Excel.Sheets;

namespace DailyDuty.Models;

public class LuminaTaskConfigList<T> : ICollection<LuminaTaskConfig<T>> where T : struct, IExcelRow<T> {
	public List<LuminaTaskConfig<T>> ConfigList = [];

	// Implement ICollection
	public IEnumerator<LuminaTaskConfig<T>> GetEnumerator() => ConfigList.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	public void Add(LuminaTaskConfig<T> item) => ConfigList.Add(item);
	public void Clear() => ConfigList.Clear();
	public bool Contains(LuminaTaskConfig<T> item) => ConfigList.Contains(item);
	public void CopyTo(LuminaTaskConfig<T>[] array, int arrayIndex) => ConfigList.CopyTo(array, arrayIndex);
	public bool Remove(LuminaTaskConfig<T> item) => ConfigList.Remove(item);
	public int Count => ConfigList.Count;
	public bool IsReadOnly => false;
	// End ICollection
    
	public bool Draw() {
		switch (this) {
			case LuminaTaskConfigList<ContentsNote>:
				return DrawContentsNoteConfig();
            
			case LuminaTaskConfigList<ContentFinderCondition>:
				return DrawContentFinderConditionConfig();

			case LuminaTaskConfigList<ContentRoulette>:
			case LuminaTaskConfigList<ClassJob>:
			case LuminaTaskConfigList<MobHuntOrderType>:
			case LuminaTaskConfigList<Addon>:
				return DrawStandardConfigList();

			default:
				ImGui.TableNextColumn();
				ImGui.Text("Invalid ModuleConfig Data Type");
				break;
		}

		return false;
	}

	public void Sort() => ConfigList = ConfigList.OrderBy(e => e.RowId).ToList();
    
	private bool DrawStandardConfigList() {
		var result = false;
		
		foreach (var configEntry in ConfigList) {
			var entryLabel = configEntry.Label();
            
			var enabled = configEntry.Enabled;
			if (ImGui.Checkbox($"{entryLabel}##{configEntry.RowId}", ref enabled)) {
				configEntry.Enabled = enabled;
				result = true;
			}
		}

		return result;
	}

	private bool DrawContentsNoteConfig() {
		var result = false;
		
		foreach (var category in Service.DataManager.GetExcelSheet<ContentsNoteCategory>().Where(category => category.Unknown0.ToString() != string.Empty)) {
			if (ImGui.CollapsingHeader(category.Unknown0.ToString())) {
				using var indent = ImRaii.PushIndent();
				
				foreach (var option in ConfigList) {
					var luminaData = Service.DataManager.GetExcelSheet<ContentsNote>().GetRow(option.RowId);
					if (luminaData.ContentType.RowId != category.RowId) continue;

					var enabled = option.Enabled;
					if (ImGui.Checkbox(luminaData.Name.ToString(), ref enabled)) {
						option.Enabled = enabled;
						result = true;
					}
				}
			}
		}

		return result;
	}

	private bool DrawContentFinderConditionConfig() {
		var result = false;
		
		using var table = ImRaii.Table("raid_tracker_table", 2, ImGuiTableFlags.SizingStretchSame);
		if (!table) return false;
		
		ImGui.TableNextColumn();
		ImGui.TextColored(KnownColor.Gray.Vector(), Strings.DutyName);

		ImGui.TableNextColumn();
		ImGui.TextColored(KnownColor.Gray.Vector(), Strings.NumDrops);
		ImGuiComponents.HelpMarker(Strings.RaidsModuleHelp);
                        
		if (ConfigList.Count > 0) {
			foreach (var data in ConfigList) {
				var luminaData = Service.DataManager.GetExcelSheet<ContentFinderCondition>().GetRow(data.RowId);

				ImGui.TableNextColumn();
				var enabled = data.Enabled;
				if (ImGui.Checkbox(luminaData.Name.ToString(), ref enabled)) {
					data.Enabled = enabled;
					result = true;
				}
                        
				ImGui.TableNextColumn();
				var count = data.TargetCount;
				ImGui.InputInt($"##TrackedItemCount{luminaData.Name}", ref count);
				if (ImGui.IsItemDeactivatedAfterEdit()) {
					data.TargetCount = count;
					result = true;
				}
			}
		}
		else {
			ImGui.TableNextColumn();
			ImGui.TextColored(KnownColor.Orange.Vector(), Strings.NothingToTrack);
		}

		return result;
	}
}