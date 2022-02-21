using System.Numerics;

namespace DailyDuty.Interfaces;

internal interface ICountdownTimer
{
    public bool Enabled { get; }
    public int ElementWidth { get; }

    public Vector4 Color { get; }

    public void Draw()
    {
        if (Enabled)
        {
            DrawContents();
        }
    }

    protected void DrawContents();

}