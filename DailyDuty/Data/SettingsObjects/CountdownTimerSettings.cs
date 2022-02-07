using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Utilities;
using Lumina.Data.Parsing.Layer;

namespace DailyDuty.Data.SettingsObjects;

[Serializable]
public class CountdownTimerSettings
{
    public bool DailyCountdownEnabled = true;
    public Vector4 DailyCountdownColor = Colors.Blue;

    public bool WeeklyCountdownEnabled = true;
    public Vector4 WeeklyCountdownColor = Colors.Purple;

    public bool FashionReportCountdownEnabled = false;
    public Vector4 FashionReportCountdownColor = Colors.Green;
    public int TimerWidth = 200;
}