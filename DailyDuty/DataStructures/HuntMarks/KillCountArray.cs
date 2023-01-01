using System.Linq;

namespace DailyDuty.DataStructures.HuntMarks;

public unsafe class KillCountArray
{
    private readonly int* rawData;
    
    public KillCountArray(int* start)
    {
        rawData = start;
    }

    public KillCounts this[int index] => new(rawData + index * 5);

}

public unsafe class KillCounts
{
    private readonly int[] rawCounts = new int[5];

    public KillCounts(int* start)
    {
        foreach (var index in Enumerable.Range(0, 5))
        {
            rawCounts[index] = start[index];
        }
    }
        
    public int this[int i] => rawCounts[i];
}