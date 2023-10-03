using System.Linq;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.Game;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Abstracts;


public abstract unsafe class HuntMarksBase : Module.SpecialTaskModule<MobHuntOrderType>
{
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
    
    protected override ModuleStatus GetModuleStatus() => GetIncompleteCount(Config.TaskConfig, Data.TaskData) == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = $"{GetIncompleteCount(Config.TaskConfig, Data.TaskData)} {Strings.HuntsRemaining}",
    };
}