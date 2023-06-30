using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("TextStyle", 3)]
public interface ITodoTextStyle
{
    [BoolConfig("HeaderItalic")]
    public bool HeaderItalic { get; set; }
    
    [BoolConfig("ModuleItalic")]
    public bool ModuleItalic { get; set; }

    [BoolConfig("EnableOutline")]
    public bool Edge { get; set; }

    [BoolConfig("EnableGlowingOutline")]
    public bool Glare { get; set; }
}