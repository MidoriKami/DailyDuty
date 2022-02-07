using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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