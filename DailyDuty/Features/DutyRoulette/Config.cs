using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyDuty.Classes;
using Dalamud.Interface;

namespace DailyDuty.Features.DutyRoulette;

public class Config : ConfigBase {
    public bool CompleteWhenCapped;
    public bool ColorContentFinder = true;
    public Vector4 CompleteColor = KnownColor.LimeGreen.Vector();
    public Vector4 IncompleteColor = KnownColor.OrangeRed.Vector();
    // public bool ShowOpenDailyDutyButton = true; // todo: move these into Duty Finder Improvements
    // public bool ShowResetTimer = true;
    // public Vector4 TimerColor = KnownColor.Black.Vector();
    public List<uint> TrackedRoulettes = [];
}
