using System.Drawing;
using System.Numerics;
using DailyDuty.Classes;
using Dalamud.Interface;

namespace DailyDuty.Features.WondrousTails;

public class WondrousTailsConfig : ConfigBase {
    public bool InstanceNotifications = true;
    public bool StickerAvailableNotice = true;
    public bool UnclaimedBookWarning = true;
    public bool ShuffleAvailableNotice;
    public bool CloverIndicator = true;
    public bool ColorDutyFinderText;
    public Vector4 DutyFinderColor = KnownColor.Yellow.Vector();
}
