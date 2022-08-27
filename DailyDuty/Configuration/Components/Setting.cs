namespace DailyDuty.Configuration.Components;

public record Setting<T>(T Value)
{
    public T Value = Value;

    public override string ToString()
    {
        return Value?.ToString() ?? "Null";
    }
}
