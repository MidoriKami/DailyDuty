using System;
using Dalamud.Interface.GameFonts;

namespace DailyDuty.System;

public class FontController : IDisposable
{
    public GameFontHandle Axis12 { get; }

    public FontController()
    {
        Axis12 = Service.PluginInterface.UiBuilder.GetGameFontHandle( new GameFontStyle(GameFontFamilyAndSize.Axis12) );
    }

    public void Dispose()
    {
        Axis12.Dispose();
    }
}