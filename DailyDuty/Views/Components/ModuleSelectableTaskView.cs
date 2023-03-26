using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.System.Localization;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;
using Action = System.Action;

namespace DailyDuty.Views.Components;

public static class ModuleSelectableTaskView
{
    public static void DrawConfig(FieldInfo? field, ModuleConfigBase moduleConfig, Action saveAction)
    {
        if (field is null) return;
        
        ImGui.Text(Strings.TaskSelection);
        ImGui.Separator();

        ImGuiHelpers.ScaledIndent(15.0f);

        if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
        {
            // If the type inside the list is generic
            if (field.FieldType.GetGenericArguments()[0] is { IsGenericType: true } listType)
            {
                // If the list contains a LuminaTaskConfig
                if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(LuminaTaskConfig<>))
                {
                    var configType = listType.GetGenericArguments()[0];

                    // If the contained type is ContentsNote
                    if (configType == typeof(ContentsNote))
                    {
                        var list = (List<LuminaTaskConfig<ContentsNote>>) field.GetValue(moduleConfig)!;

                        foreach (var category in LuminaCache<ContentsNoteCategory>.Instance.Where(category => category.CategoryName.ToString() != string.Empty))
                        {
                            if (ImGui.CollapsingHeader(category.CategoryName.ToString()))
                            {
                                foreach (var option in list)
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
                    else if (configType == typeof(ContentRoulette))
                    {
                        var list = (List<LuminaTaskConfig<ContentRoulette>>) field.GetValue(moduleConfig)!;

                        var dataRows = LuminaCache<ContentRoulette>.Instance
                            .Where(cr => cr.DutyType.ToString() != string.Empty)
                            .OrderBy(cr => cr.SortKey);

                        foreach (var data in dataRows)
                        {
                            var taskInfo = list.First(task => task.RowId == data.RowId);
                            
                            var enabled = taskInfo.Enabled;
                            if (ImGui.Checkbox(data.Name.ToString(), ref enabled))
                            {
                                taskInfo.Enabled = enabled;
                                saveAction.Invoke();
                            }
                        }
                    }
                    else if (configType == typeof(ClassJob))
                    {
                        var list = (List<LuminaTaskConfig<ClassJob>>) field.GetValue(moduleConfig)!;
                        
                        foreach (var data in list)
                        {
                            var luminaData = LuminaCache<ClassJob>.Instance.GetRow(data.RowId)!;
                            var label = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(luminaData.Name.ToString());
                            
                            var enabled = data.Enabled;
                            if (ImGui.Checkbox(label, ref enabled))
                            {
                                data.Enabled = enabled;
                                saveAction.Invoke();
                            }
                        }
                    }
                    else if (configType == typeof(MobHuntOrderType))
                    {
                        var list = (List<LuminaTaskConfig<MobHuntOrderType>>) field.GetValue(moduleConfig)!;
                        
                        foreach (var data in list)
                        {
                            var luminaData = LuminaCache<MobHuntOrderType>.Instance.GetRow(data.RowId)!;
                            var label = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(luminaData.EventItem.Value?.Name.ToString() ?? "Unable to Read Event Item");
                            
                            var enabled = data.Enabled;
                            if (ImGui.Checkbox(label, ref enabled))
                            {
                                data.Enabled = enabled;
                                saveAction.Invoke();
                            }
                        }
                    }
                    else if (configType == typeof(Addon))
                    {
                        var list = (List<LuminaTaskConfig<Addon>>) field.GetValue(moduleConfig)!;
                        
                        foreach (var data in list)
                        {
                            var luminaData = LuminaCache<Addon>.Instance.GetRow(data.RowId)!;
                            
                            var enabled = data.Enabled;
                            if (ImGui.Checkbox(luminaData.Text.ToString(), ref enabled))
                            {
                                data.Enabled = enabled;
                                saveAction.Invoke();
                            }
                        }
                    }
                    else if (configType == typeof(ContentFinderCondition))
                    {
                        var list = (List<LuminaTaskConfig<ContentFinderCondition>>) field.GetValue(moduleConfig)!;
                        
                        if (ImGui.BeginTable("##RaidTrackerTable", 2, ImGuiTableFlags.SizingStretchSame))
                        {
                            ImGui.TableNextColumn();
                            ImGui.TextColored(KnownColor.Gray.AsVector4(), Strings.DutyName);

                            ImGui.TableNextColumn();
                            ImGui.TextColored(KnownColor.Gray.AsVector4(), Strings.NumDrops);
                            ImGuiComponents.HelpMarker(Strings.RaidsModuleHelp);
                            
                            if (list.Count > 0)
                            {
                                foreach (var data in list)
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
            }

            ImGuiHelpers.ScaledDummy(10.0f);
            ImGuiHelpers.ScaledIndent(-15.0f);
        }
    }
    
    public static void DrawData(FieldInfo? field, ModuleDataBase moduleData)
    {
        if (field is null) return;
        
        ImGui.Text(Strings.TaskData);
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);

        if (ImGui.BeginTable("##TaskDataTable", 2, ImGuiTableFlags.SizingStretchSame))
        {
            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (field.FieldType.GetGenericArguments()[0] is { IsGenericType: true } listType)
                {
                    // If the list contains a LuminaTaskConfig
                    if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(LuminaTaskData<>))
                    {
                        var configType = listType.GetGenericArguments()[0];

                        // If the contained type is ContentsNote
                        if (configType == typeof(ContentsNote))
                        {
                            var list = (List<LuminaTaskData<ContentsNote>>) field.GetValue(moduleData)!;

                            foreach (var data in list)
                            {
                                var luminaData = LuminaCache<ContentsNote>.Instance.GetRow(data.RowId)!;

                                ImGui.TableNextColumn();
                                ImGui.Text(luminaData.Name.ToString());

                                ImGui.TableNextColumn();
                                var color = data.Complete ? KnownColor.Green.AsVector4() : KnownColor.Orange.AsVector4();
                                var text = data.Complete ? Strings.Complete : Strings.Incomplete;
                                ImGui.TextColored(color, text);
                            }
                        }
                        else if (configType == typeof(ContentRoulette))
                        {
                            var list = (List<LuminaTaskData<ContentRoulette>>) field.GetValue(moduleData)!;

                            foreach (var data in list)
                            {
                                ImGui.TableNextColumn();
                                ImGui.Text(LuminaCache<ContentRoulette>.Instance.GetRow(data.RowId)!.Name.ToString());

                                ImGui.TableNextColumn();
                                var color = data.Complete ? KnownColor.Green.AsVector4() : KnownColor.Orange.AsVector4();
                                var text = data.Complete ? Strings.Complete : Strings.Incomplete;
                                ImGui.TextColored(color, text);
                            }
                        }
                        else if (configType == typeof(ClassJob))
                        {
                            var list = (List<LuminaTaskData<ClassJob>>) field.GetValue(moduleData)!;

                            foreach (var data in list)
                            {
                                ImGui.TableNextColumn();
                                var luminaData = LuminaCache<ClassJob>.Instance.GetRow(data.RowId)!;
                                var label = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(luminaData.Name.ToString());
                                ImGui.Text(label);

                                ImGui.TableNextColumn();
                                var color = data.Complete ? KnownColor.Green.AsVector4() : KnownColor.Orange.AsVector4();
                                var text = data.Complete ? Strings.Complete : Strings.Incomplete;
                                ImGui.TextColored(color, text);
                            }
                        }
                        else if (configType == typeof(MobHuntOrderType))
                        {
                            var list = (List<LuminaTaskData<MobHuntOrderType>>) field.GetValue(moduleData)!;

                            foreach (var data in list)
                            {
                                ImGui.TableNextColumn();
                                var luminaData = LuminaCache<MobHuntOrderType>.Instance.GetRow(data.RowId)!;
                                var label = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(luminaData.EventItem.Value?.Name.ToString() ?? "Unable to Read Event Item");
                                ImGui.Text(label);

                                ImGui.TableNextColumn();
                                var color = data.Complete ? KnownColor.Green.AsVector4() : KnownColor.Orange.AsVector4();
                                var text = data.Complete ? Strings.Complete : Strings.Incomplete;
                                ImGui.TextColored(color, text);
                            }
                        }
                        else if (configType == typeof(Addon))
                        {
                            var list = (List<LuminaTaskData<Addon>>) field.GetValue(moduleData)!;

                            foreach (var data in list)
                            {
                                ImGui.TableNextColumn();
                                var luminaData = LuminaCache<Addon>.Instance.GetRow(data.RowId)!;
                                ImGui.Text(luminaData.Text.ToString());

                                ImGui.TableNextColumn();
                                var color = data.Complete ? KnownColor.Green.AsVector4() : KnownColor.Orange.AsVector4();
                                var text = data.Complete ? Strings.Complete : Strings.Incomplete;
                                ImGui.TextColored(color, text);
                            }
                        }
                        else if (configType == typeof(ContentFinderCondition))
                        {
                            var list = (List<LuminaTaskData<ContentFinderCondition>>) field.GetValue(moduleData)!;

                            ImGui.TableNextColumn();
                            ImGui.TextColored(KnownColor.Gray.AsVector4(), Strings.DutyName);

                            ImGui.TableNextColumn();
                            ImGui.TextColored(KnownColor.Gray.AsVector4(), Strings.CurrentNumDrops);
                            
                            if (list.Count > 0)
                            {
                                foreach (var data in list)
                                {
                                    ImGui.TableNextColumn();
                                    var luminaData = LuminaCache<ContentFinderCondition>.Instance.GetRow(data.RowId)!;
                                    ImGui.Text(luminaData.Name.ToString());

                                    ImGui.TableNextColumn();
                                    ImGui.Text(data.CurrentCount.ToString());
                                }
                            }
                            else
                            {
                                ImGui.TableNextColumn();
                                ImGui.TextColored(KnownColor.Orange.AsVector4(), Strings.NothingToTrack);
                            }
                        }
                    }
                }
            }
            ImGui.EndTable();
        }

        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}
