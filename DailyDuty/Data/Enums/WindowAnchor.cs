namespace DailyDuty.Data.Enums
{
    public enum WindowAnchor
    {
        Bottom = 1,
        Right = 2,
        
        TopLeft = 0,
        TopRight = Right,
        BottomLeft = Bottom,
        BottomRight = Bottom | Right
    }
}
