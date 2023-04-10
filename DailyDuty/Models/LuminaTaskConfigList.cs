using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using DailyDuty.Interfaces;
using DailyDuty.Models.Attributes;
using DailyDuty.System.Localization;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Caching;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using Action = System.Action;

namespace DailyDuty.Models;

public class LuminaTaskConfigList<T> : IConfigDrawable, ICollection<LuminaTaskConfig<T>> where T : ExcelRow
{
    public List<LuminaTaskConfig<T>> ConfigList = new();

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
    
    public void Draw(Action saveAction)
    {
        ImGui.Text(Strings.TaskSelection);
        ImGui.Separator();

        ImGuiHelpers.ScaledIndent(15.0f);
        
        switch (this)
        {
            case LuminaTaskConfigList<ContentsNote>:
                DrawContentsNoteConfig(saveAction);
                break;
            
            case LuminaTaskConfigList<ContentFinderCondition>:
                DrawContentFinderConditionConfig(saveAction);
                break;

            case LuminaTaskConfigList<ContentRoulette>:
            case LuminaTaskConfigList<ClassJob>:
            case LuminaTaskConfigList<MobHuntOrderType>:
            case LuminaTaskConfigList<Addon>:
                DrawStandardConfigList(saveAction);
                break;

            default:
                ImGui.TableNextColumn();
                ImGui.Text("Invalid Config Data Type");
                break;
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }

    private void DrawStandardConfigList(Action saveAction)
    {
        foreach (var configEntry in ConfigList)
        {
            var entryLabel = this switch
            {
                LuminaTaskConfigList<ContentRoulette> => LuminaCache<ContentRoulette>.Instance.GetRow(configEntry.RowId)!.Name.ToString(),
                LuminaTaskConfigList<ClassJob> => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(LuminaCache<ClassJob>.Instance.GetRow(configEntry.RowId)!.Name.ToString()),
                LuminaTaskConfigList<MobHuntOrderType> => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(LuminaCache<MobHuntOrderType>.Instance.GetRow(configEntry.RowId)!.EventItem.Value?.Singular.ToString() ?? "Unable to Read Event Item Name"),
                LuminaTaskConfigList<Addon> => LuminaCache<Addon>.Instance.GetRow(configEntry.RowId)!.Text.ToString(),
                _ => throw new Exception($"Data Type Not Registered")
            };
            
            var enabled = configEntry.Enabled;
            if (ImGui.Checkbox($"{entryLabel}##{configEntry.RowId}", ref enabled))
            {
                configEntry.Enabled = enabled;
                saveAction.Invoke();
            }
        }
    }

    private void DrawContentsNoteConfig(Action saveAction)
    {
        foreach (var category in LuminaCache<ContentsNoteCategory>.Instance.Where(category => category.CategoryName.ToString() != string.Empty))
        {
            if (ImGui.CollapsingHeader(category.CategoryName.ToString()))
            {
                foreach (var option in ConfigList)
                {
                    var luminaData = LuminaCache<ContentsNote>.Instance.GetRow(option.RowId)!;
                    if (luminaData.ContentType != category.RowId) continue;

                    var enabled = option.Enabled;
                    if (ImGui.Checkbox(luminaData.Name.ToString(), ref enabled))
                    {
                        option.Enabled = enabled;
                        saveAction.Invoke();
                    }
                }
            }
        }
    }

    private void DrawContentFinderConditionConfig(Action saveAction)
    {
        if (ImGui.BeginTable("##RaidTrackerTable", 2, ImGuiTableFlags.SizingStretchSame))
        {
            ImGui.TableNextColumn();
            ImGui.TextColored(KnownColor.Gray.AsVector4(), Strings.DutyName);

            ImGui.TableNextColumn();
            ImGui.TextColored(KnownColor.Gray.AsVector4(), Strings.NumDrops);
            ImGuiComponents.HelpMarker(Strings.RaidsModuleHelp);
                            
            if (ConfigList.Count > 0)
            {
                foreach (var data in ConfigList)
                {
                    var luminaData = LuminaCache<ContentFinderCondition>.Instance.GetRow(data.RowId)!;

                    ImGui.TableNextColumn();
                    var enabled = data.Enabled;
                    if (ImGui.Checkbox(luminaData.Name.ToString(), ref enabled))
                    {
                        data.Enabled = enabled;
                        saveAction.Invoke();
                    }
                            
                    ImGui.TableNextColumn();
                    var count = data.TargetCount;
                    ImGui.InputInt("##TrackedItemCount", ref count, 0, 0);
                    if (ImGui.IsItemDeactivatedAfterEdit())
                    {
                        data.TargetCount = count;
                        saveAction.Invoke();
                    }
                }
            }
            else
            {
                ImGui.TableNextColumn();
                ImGui.TextColored(KnownColor.Orange.AsVector4(), Strings.NothingToTrack);
            }
                            
            ImGui.EndTable();
        }
    }
}
