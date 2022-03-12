using System.Numerics;
using DailyDuty.Utilities;

namespace DailyDuty.Data.Graphical
{
    public class TimerStyle
    {
        public TimerOptions Options = new();
        public Vector4 BackgroundColor = Colors.Black;
        public Vector4 ForegroundColor = Colors.Purple;
        public Vector4 TextColor = Colors.White;
        public int Padding = 5;
        public int Size = 200;
        public bool StretchToFit = false;
    }
}
