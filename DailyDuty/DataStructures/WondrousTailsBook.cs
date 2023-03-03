using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace DailyDuty.DataStructures;

public record WondrousTailsTask(PlayerState.WeeklyBingoTaskStatus TaskState, List<uint> DutyList);

public unsafe class WondrousTailsBook
{
    private static WondrousTailsBook? _instance;
    public static WondrousTailsBook Instance => _instance ??= new WondrousTailsBook();
    
    public int Stickers => PlayerState.Instance()->WeeklyBingoNumPlacedStickers;
    public uint SecondChance => PlayerState.Instance()->WeeklyBingoNumSecondChancePoints;
    public static bool PlayerHasBook => PlayerState.Instance()->HasWeeklyBingoJournal;
    public bool NewBookAvailable => DateTime.Now > Deadline - TimeSpan.FromDays(7);
    public bool IsComplete => Stickers == 9;
    public bool NeedsNewBook => NewBookAvailable && IsComplete;
    private DateTime Deadline => DateTimeOffset.FromUnixTimeSeconds(PlayerState.Instance()->GetWeeklyBingoExpireUnixTimestamp()).ToLocalTime().DateTime; 
    
    public static WondrousTailsTask? GetTaskForDuty(uint instanceID) => GetAllTaskData().FirstOrDefault(task => task.DutyList.Contains(instanceID));

    public static IEnumerable<WondrousTailsTask> GetAllTaskData() => 
        (from index in Enumerable.Range(0, 16) 
            let taskButtonState = PlayerState.Instance()->GetWeeklyBingoTaskStatus(index)
            let instances = TaskLookup.GetInstanceListFromID(PlayerState.Instance()->WeeklyBingoOrderData[index]) 
            select new WondrousTailsTask(taskButtonState, instances))
        .ToList();
}