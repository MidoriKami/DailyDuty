using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
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
    }
}
