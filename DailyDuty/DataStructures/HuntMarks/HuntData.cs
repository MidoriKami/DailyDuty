using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace DailyDuty.DataStructures.HuntMarks;

public unsafe class HuntData
{
    public byte HuntID { get; init; }
    public HuntMarkType HuntType { get; init; }
    public MobHunt.KillCounts* KillCounts { get; init; }
    public HuntInfo TargetInfo => new(HuntType, HuntID, IsElite);
    public bool Obtained { get; init; }
    public bool IsElite => HuntType is
        HuntMarkType.EndwalkerElite or
        HuntMarkType.ShadowbringersElite or
        HuntMarkType.StormbloodElite or
        HuntMarkType.HeavenswardElite or
        HuntMarkType.RealmRebornElite;
    public bool IsCompleted => Enumerable.Range(0, IsElite ? 1 : 5).All(index => (TargetInfo[index]?.NeededKills ?? 0) == (*KillCounts)[index]);
}