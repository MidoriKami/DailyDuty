using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.UserInterface.Windows;
using DailyDuty.Utilities;
using Dalamud.Interface.Windowing;

namespace DailyDuty.System;

internal class WindowManager : IDisposable
{
    private readonly WindowSystem windowSystem = new("DailyDuty");

    private readonly List<Window> windows = new()
    {
        new ConfigurationWindow(),
        new StatusWindow(),
    };

    public WindowManager()
    {
        Log.Verbose("Constructing WindowManager");

        Log.Verbose("Adding Windows to WindowManager");
        foreach (var window in windows)
        {
            windowSystem.AddWindow(window);
        }

        Log.Verbose("Adding Draw Delegates");
        Service.PluginInterface.UiBuilder.Draw += DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    private void DrawUI() => windowSystem.Draw();

    private void DrawConfigUI() => windows[0].IsOpen = true;

    public T? GetWindowOfType<T>()
    {
        return windows.OfType<T>().FirstOrDefault();
    }

    public void Dispose()
    {
        Service.PluginInterface.UiBuilder.Draw -= DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

        foreach (var window in windows.OfType<IDisposable>())
        {
            window.Dispose();
        }

        windowSystem.RemoveAllWindows();
    }
}