using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Utilities;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DailyDuty.DataStructures;

internal unsafe class WondrousTailsBook
{
    private delegate int WondrousTailsGetDeadlineDelegate(int* deadlineIndex);

    [Signature("88 05 ?? ?? ?? ?? 8B 43 18", ScanType = ScanType.StaticAddress)]
    private readonly WondrousTailsStruct* wondrousTails = null;

    [Signature("8B 81 ?? ?? ?? ?? C1 E8 04 25")]
    private readonly WondrousTailsGetDeadlineDelegate wondrousTailsGetDeadline = null!;

    [Signature("48 8D 0D ?? ?? ?? ?? 48 89 BD ?? ?? ?? ?? E8 ?? ?? ?? ?? 44 8B C0", ScanType = ScanType.StaticAddress)]
    private readonly int* wondrousTailsDeadlineIndex = null;

    private const int WondrousTailsBookItemID = 2002023;

    private static WondrousTailsBook? _instance;
    public static WondrousTailsBook Instance => _instance ??= new WondrousTailsBook();

    private WondrousTailsBook()
    {
        SignatureHelper.Initialise(this);
    }

    public int Stickers => wondrousTails->Stickers;
    public int SecondChance => wondrousTails->SecondChance;
    public static bool PlayerHasBook => InventoryManager.Instance()->GetInventoryItemCount(WondrousTailsBookItemID) > 0;
    public bool NewBookAvailable =>  DateTime.Now > Deadline - TimeSpan.FromDays(7);
    public bool Complete => wondrousTails->Stickers == 9;
    public bool NeedsNewBook => NewBookAvailable && Complete;
    private DateTime Deadline => DateTimeOffset.FromUnixTimeSeconds(wondrousTailsGetDeadline(wondrousTailsDeadlineIndex)).ToLocalTime().DateTime;
    
    public WondrousTailsTask? GetTaskForDuty(uint instanceID)
    {
        return GetAllTaskData().FirstOrDefault(task => task.DutyList.Contains(instanceID));
    }

    public IEnumerable<WondrousTailsTask> GetAllTaskData()
    {
        var result = new List<WondrousTailsTask>();

        for (var i = 0; i < 16; ++i)
        {
            var taskButtonState = wondrousTails->TaskStatus(i);
            var instances = TaskLookup.GetInstanceListFromID(wondrousTails->Tasks[i]);

            result.Add(new WondrousTailsTask(taskButtonState, instances));
        }

        return result;
    }
}