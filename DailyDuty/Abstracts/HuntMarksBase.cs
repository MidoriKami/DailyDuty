using System.Globalization;
using System.Linq;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Abstracts;

public class HuntMarksConfig : ModuleConfigBase
{
    [SelectableTasks]
    public LuminaTaskConfigList<MobHuntOrderType> TaskConfig = new();
}

public class HuntMarksData : ModuleDataBase
{
    [SelectableTasks] 
    public LuminaTaskDataList<MobHuntOrderType> TaskData = new();
}

public abstract unsafe class HuntMarksBase : Module.SpecialModule
{
    public override ModuleDataBase ModuleData { get; protected set; } = new HuntMarksData();
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new HuntMarksConfig();
    protected HuntMarksConfig Config => ModuleConfig as HuntMarksConfig ?? new HuntMarksConfig();
    protected HuntMarksData Data => ModuleData as HuntMarksData ?? new HuntMarksData();

    private static MobHunt* HuntData => MobHunt.Instance();
    
    public override void Update()
    {
        foreach (var task in Data.TaskData)
        {
            // If we have the active mark bill
            if (HuntData->AvailableMarkId[task.RowId] == HuntData->ObtainedMarkId[task.RowId] && !task.Complete)
            {
                var orderData = LuminaCache<MobHuntOrderType>.Instance.GetRow(task.RowId)!;
                var targetRow = orderData.OrderStart.Row + HuntData->ObtainedMarkId[task.RowId] - 1;
                
                // Elite
                if (orderData.Type is 2 && IsEliteMarkComplete(targetRow, task.RowId))
                {
                    task.Complete = true;
                    DataChanged = true;
                }
                // Regular Hunt
                else if (orderData.Type is 1 && IsNormalMarkComplete(targetRow, task.RowId))
                {
                    task.Complete = true;
                    DataChanged = true;
                }
            }
        }
        
        base.Update();
    }

    private bool IsEliteMarkComplete(uint targetRow, uint markId)
    {
        var eliteTargetInfo = LuminaCache<MobHuntOrder>.Instance.GetRow(targetRow, 0)!;

        return HuntData->CurrentKillsSpan[(int) markId][0] == eliteTargetInfo.NeededKills;
    }

    private bool IsNormalMarkComplete(uint targetRow, uint markId)
    {
        var allTargetsKilled = Enumerable.Range(0, 5).All(index => 
        {
            var huntData = LuminaCache<MobHuntOrder>.Instance.GetRow(targetRow, (uint) index)!;

            return HuntData->CurrentKillsSpan[(int) markId][index] == huntData.NeededKills;
        });

        return allTargetsKilled;
    }

    public override void Reset()
    {
        Data.TaskData.Reset();
        
        base.Reset();
    }
    
    public override bool HasTooltip { get; protected set; } = true;
    public override string GetTooltip() => GetTaskListTooltip(Config.TaskConfig, Data.TaskData, row =>
    {
        var mobHuntOrderType = LuminaCache<MobHuntOrderType>.Instance.GetRow(row)!;
        var eventItemName = mobHuntOrderType.EventItem.Value?.Name.ToString();
        if (eventItemName == string.Empty) eventItemName = mobHuntOrderType.EventItem.Value?.Singular.ToString();

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(eventItemName ?? "Unable to Read Event Item");
    });
    
    protected override ModuleStatus GetModuleStatus() => GetIncompleteCount(Config.TaskConfig, Data.TaskData) == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = $"{GetIncompleteCount(Config.TaskConfig, Data.TaskData)} {Strings.HuntsRemaining}",
    };
}