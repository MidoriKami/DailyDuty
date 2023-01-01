using System.Linq;

namespace DailyDuty.DataStructures.HuntMarks;

public class HuntData
{
    public byte HuntID { get; init; }
    public HuntMarkType HuntType { get; init; }
    public KillCounts KillCounts { get; init; } = null!;
    public HuntInfo TargetInfo => new(HuntType, HuntID, IsElite);
    public bool Obtained { get; init; }
    public bool IsElite => HuntType is
        HuntMarkType.EndwalkerElite or
        HuntMarkType.ShadowbringersElite or
        HuntMarkType.StormbloodElite or
        HuntMarkType.HeavenswardElite or
        HuntMarkType.RealmRebornElite;

    public bool IsCompleted
    {
        get
        {
            if (IsElite)
            {
                return TargetInfo[0]?.NeededKills == KillCounts[0];
            }
            else
            {
                return Enumerable.Range(0, 5)
                    .All(index => TargetInfo[index]?.NeededKills == KillCounts[index]);
            }
        }
    }
}