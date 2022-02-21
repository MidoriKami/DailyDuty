namespace DailyDuty.Data.ModuleData.HuntMarks
{
    public class TrackedHunt
    {
        public readonly ExpansionType Expansion;
        public bool Tracked;
        public bool Obtained;

        public TrackedHunt(ExpansionType expansion, bool tracked)
        {
            Expansion = expansion;
            Tracked = tracked;
            Obtained = false;
        }
    }
}
