namespace DailyDuty.Configuration.Components;

public record Setting<T>(T Value) where T : struct
{
    public T Value = Value;

    public override string ToString()
    {
        return Value.ToString() ?? "Null";
    }
}
