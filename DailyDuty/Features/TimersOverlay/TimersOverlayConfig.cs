using System.Collections.Generic;
using System.Text.Json.Serialization;
using DailyDuty.Classes;

namespace DailyDuty.Features.TimersOverlay;

public class TimersOverlayConfig : ConfigBase {
    public bool HideInDuties = true;
    public bool HideInQuestEvents = true;
    public bool HideTimerSeconds = false;
    public float Scale = 1.0f;
    public bool ShowLabel = true;
    public bool ShowCountdownText = true;
    
    [JsonIgnore] public bool EnableMovingTimers = false;

    public List<string> EnabledTimers = [];
    public Dictionary<string, TimersOverlayTimerData> TimerData = [];
}
