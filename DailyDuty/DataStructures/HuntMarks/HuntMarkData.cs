using Dalamud.Utility.Signatures;

namespace DailyDuty.DataStructures.HuntMarks;

public unsafe class HuntMarkData
{
    [Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
    private readonly MobHuntStruct* huntStruct = null;
    
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
        HuntID = huntStruct->MarkID[(int)type],
        HuntType = type,
        Obtained = huntStruct->Obtained[(int)type],
        KillCounts = huntStruct->KillCounts[(int)type],
    };
}


