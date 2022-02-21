using System.Numerics;

namespace DailyDuty.Data.SettingsObjects
{
    public class DutyFinderOverlaySettings
    {
        public bool Enabled = false;
        public bool WondrousTailsOverlayEnabled = false;
        public bool DutyRouletteOverlayEnabled = false;
        public Vector4 DutyRouletteCompleteColor = new Vector4(33 / 255f, 193 / 255f, 0, 1.0f);
        public Vector4 DutyRouletteIncompleteColor = new Vector4(210/255f, 42/255f, 43/255f, 1.0f);
    }
}
