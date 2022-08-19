using System.Numerics;
using DailyDuty.Utilities;

namespace DailyDuty.Configuration.Components;

public class TaskColors
{
    public readonly Setting<Vector4> HeaderColor = new(Colors.White);
    public readonly Setting<Vector4> IncompleteColor = new(Colors.Red);
    public readonly Setting<Vector4> UnavailableColor = new(Colors.Orange);
    public readonly Setting<Vector4> CompleteColor = new(Colors.Green);
}