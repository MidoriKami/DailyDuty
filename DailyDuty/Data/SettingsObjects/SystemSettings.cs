using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Data.SettingsObjects;

[Serializable]
public class SystemSettings
{
    public int ZoneChangeDelayRate = 1;
    public bool ShowSaveDebugInfo = false;
    public bool ClickableLinks = false;
    public bool EnableDebugOutput = false;
}