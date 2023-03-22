using System.Collections.Generic;
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
    public List<LuminaTaskConfig<MobHuntOrderType>> Tasks = new();
}

public class HuntMarksData : ModuleDataBase
{
    [SelectableTasks] 
    public List<LuminaTaskData<MobHuntOrderType>> Tasks = new();
}

public abstract unsafe class HuntMarksBase : Module.SpecialModule
{
    public override ModuleDataBase ModuleData { get; protected set; } = new HuntMarksData();
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new HuntMarksConfig();
    protected HuntMarksConfig Config => ModuleConfig as HuntMarksConfig ?? new HuntMarksConfig();
    protected HuntMarksData Data => ModuleData as HuntMarksData ?? new HuntMarksData();

    private readonly Dictionary<uint, bool> lastObtainedStatus = new();

    private static MobHunt* HuntData => MobHunt.Instance();
    
    public override void Update()
    {
        foreach (var task in Data.Tasks)
        {
            var huntObtained = HuntData->IsMarkBillObtained((int)task.RowId);

            if (!lastObtainedStatus.ContainsKey(task.RowId))
            {
                lastObtainedStatus.Add(task.RowId, huntObtained);
            }

            // If the status changed from obtained to not obtained
            if (lastObtainedStatus[task.RowId] && huntObtained == false)
            {
                var orderData = LuminaCache<MobHuntOrderType>.Instance.GetRow(task.RowId)!;
                var targetRow = orderData.OrderStart.Row + task.RowId - 1;
                
                // Elite
                if (orderData.Type is 2)
                {
                    var eliteTargetInfo = LuminaCache<MobHuntOrder>.Instance.GetRow(targetRow, 0)!;

                    if (HuntData->CurrentKillsSpan[(int) task.RowId][0] == eliteTargetInfo.NeededKills)
                    {
                        task.Complete = true;
                        DataChanged = true;
                    }
                }
                // Regular Hunt
                else if (orderData.Type is 1)
                {
                    var allTargetsKilled = Enumerable.Range(0, 5)
                        .All(index =>
                        {
                            var huntData = LuminaCache<MobHuntOrder>.Instance.GetRow(targetRow, (uint) index)!;

                            return HuntData->CurrentKillsSpan[(int) task.RowId][index] == huntData.NeededKills;
                        });

                    if (task.Complete != allTargetsKilled)
                    {
                        task.Complete = allTargetsKilled;
                        DataChanged = true;
                    }
                }
            }

            lastObtainedStatus[task.RowId] = huntObtained;
        }
        
        base.Update();
    }

    public override void Reset()
    {
        foreach (var data in Data.Tasks)
        {
            data.Complete = false;
        }
        
        base.Reset();
    }
    
    protected override ModuleStatus GetModuleStatus() => GetIncompleteCount(Config.Tasks, Data.Tasks) == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = $"{GetIncompleteCount(Config.Tasks, Data.Tasks)} {Strings.HuntsRemaining}",
    };
}