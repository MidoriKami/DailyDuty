using System.Numerics;
using DailyDuty.Utilities;

namespace DailyDuty.Configuration.Components;

public class TaskColors
{
    public Setting<Vector4> HeaderColor = new(Colors.White);
    public Setting<Vector4> IncompleteColor = new(Colors.Red);
    public Setting<Vector4> UnavailableColor = new(Colors.Orange);
    public Setting<Vector4> CompleteColor = new(Colors.Green);
}