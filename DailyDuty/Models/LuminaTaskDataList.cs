using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using DailyDuty.System.Localization;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Interfaces;
using KamiLib.Utilities;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Models;

public class LuminaTaskDataList<T> : IDrawable, ICollection<LuminaTaskData<T>> where T : ExcelRow
{
    private readonly List<LuminaTaskData<T>> DataList = new();

    // Implement ICollection
    public IEnumerator<LuminaTaskData<T>> GetEnumerator() => DataList.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public void Add(LuminaTaskData<T> item) => DataList.Add(item);
    public void Clear() => DataList.Clear();
    public bool Contains(LuminaTaskData<T> item) => DataList.Contains(item);
    public void CopyTo(LuminaTaskData<T>[] array, int arrayIndex) => DataList.CopyTo(array, arrayIndex);
    public bool Remove(LuminaTaskData<T> item) => DataList.Remove(item);
    public int Count => DataList.Count;
    public bool IsReadOnly => false;
    // End ICollection
    
    public void Draw()
    {
        if (ImGui.BeginTable("##TaskDataTable", 2, ImGuiTableFlags.SizingStretchSame))
        {
            switch (this)
            {
                case LuminaTaskDataList<ContentsNote>:
                case LuminaTaskDataList<ContentRoulette>:
                case LuminaTaskDataList<ClassJob>:
                case LuminaTaskDataList<MobHuntOrderType>:
                case LuminaTaskDataList<Addon>:
                    DrawStandardDataList();
                    break;
                
                case LuminaTaskDataList<ContentFinderCondition>:
                    DrawContentFinderCondition();
                    break;
                
                default:
                    ImGui.TableNextColumn();
                    ImGui.Text("Invalid Data Type");
                    break;
            }
            
            ImGui.EndTable();
        }
    }
    
    private void DrawStandardDataList()
    {
        foreach (var dataEntry in DataList)
        {
            ImGui.TableNextColumn();
            switch (this)
            {
                case LuminaTaskDataList<ContentsNote>:
                    ImGui.Text(LuminaCache<ContentsNote>.Instance.GetRow(dataEntry.RowId)!.Name.ToString());
                    break;
                
                case LuminaTaskDataList<ContentRoulette>:
                    ImGui.Text(LuminaCache<ContentRoulette>.Instance.GetRow(dataEntry.RowId)!.Name.ToString());
                    break;
                
                case LuminaTaskDataList<ClassJob>:
                    var classJob = LuminaCache<ClassJob>.Instance.GetRow(dataEntry.RowId)!;
                    var classJobLabel = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(classJob.Name.ToString());
                    ImGui.Text(classJobLabel);
                    break;
                
                case LuminaTaskDataList<MobHuntOrderType>:
                    var mobHuntOrderType = LuminaCache<MobHuntOrderType>.Instance.GetRow(dataEntry.RowId)!;
                    var eventItemName = mobHuntOrderType.EventItem.Value?.Name.ToString();
                    if (eventItemName == string.Empty) eventItemName = mobHuntOrderType.EventItem.Value?.Singular.ToString();

                    var mobHuntLabel = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(eventItemName ?? "Unable to Read Event Item");
                    ImGui.Text(mobHuntLabel);
                    break;
                
                case LuminaTaskDataList<Addon>:
                    ImGui.Text(LuminaCache<Addon>.Instance.GetRow(dataEntry.RowId)!.Text.ToString());
                    break;
            }
            
            ImGui.TableNextColumn();
            var color = dataEntry.Complete ? KnownColor.Green.AsVector4() : KnownColor.Orange.AsVector4();
            var text = dataEntry.Complete ? Strings.Complete : Strings.Incomplete;
            ImGui.TextColored(color, text);
        }
    }
    
    private void DrawContentFinderCondition()
    {
        ImGui.TableNextColumn();
        ImGui.TextColored(KnownColor.Gray.AsVector4(), Strings.DutyName);

        ImGui.TableNextColumn();
        ImGui.TextColored(KnownColor.Gray.AsVector4(), Strings.CurrentNumDrops);
                            
        if (DataList.Count > 0)
        {
            foreach (var data in DataList)
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

    public void Update(ref bool dataChanged, Func<uint, bool> getTaskStatusFunction)
    {
        foreach (var task in DataList)
        {
            var status = getTaskStatusFunction(task.RowId);

            if (task.Complete != status)
            {
                task.Complete = status;
                dataChanged = true;
            }
        }
    }

    public void Reset()
    {
        foreach (var task in DataList)
        {
            task.Complete = false;
            task.CurrentCount = 0;
        }
    }
}