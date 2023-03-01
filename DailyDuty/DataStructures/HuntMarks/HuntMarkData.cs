using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace DailyDuty.DataStructures.HuntMarks;

public unsafe class HuntMarkData
{
    private MobHunt* HuntStruct => MobHunt.Instance();
    
    private static HuntMarkData? _instance;
    public static HuntMarkData Instance => _instance ??= new HuntMarkData();

    private HuntMarkData()
    {
        SignatureHelper.Initialise(this);
    }

    public HuntData GetHuntData(HuntMarkType type)
    {
        return this[type];
    }
    
    private HuntData this[HuntMarkType type] => new()
    {
        HuntID = HuntStruct->MarkID[(int)type],
        HuntType = type,
        Obtained = HuntStruct->IsMarkBillObtained((int)type),
        KillCounts = HuntStruct->CurrentKillsSpan[(int)type],
    };
}


