using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("DisplayStyle", 5)]
public interface ITodoDisplayStyle
{
    [IntConfig("FontSize", 5, 48)]
    public int FontSize { get; set; }

    [IntConfig("HeaderSize", 5, 48)] 
    public int HeaderFontSize { get; set; }

    [IntConfig("CategorySpacing",  0, 100)]
    public int CategorySpacing { get; set; }

    [IntConfig("HeaderSpacing",  0, 100)]
    public int HeaderSpacing { get; set; }
    
    [IntConfig("ModuleSpacing",  0, 100)]
    public int ModuleSpacing { get; set; }
}