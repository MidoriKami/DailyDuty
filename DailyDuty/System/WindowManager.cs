using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
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
        new TodoConfigurationWindow(),
        new TodoOverlayWindow(),
        new TimersConfigurationWindow(),
        new TimersOverlayWindow(),
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

    private void DrawConfigUI()
    {
        if(Service.ClientState.IsPvP)
            Chat.PrintError("The configuration menu cannot be opened while in a PvP area");

        windows[0].IsOpen = true;
    }

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

    public void AddTimerStyleWindow(IModule parentModule, TimerSettings genericSettingsTimerSettings)
    {
        var windowTitle = $"{Strings.UserInterface.Timers.EditTimerTitle} - {parentModule.Name.GetTranslatedString()}";

        if (!windowSystem.Windows.Any(window => window.WindowName == windowTitle))
        {
            var newWindow = new TimersStyleWindow(parentModule, genericSettingsTimerSettings, windowTitle);

            windows.Add(newWindow);
            windowSystem.AddWindow(newWindow);
        }
    }

    public void RemoveTimerStyleWindow(TimersStyleWindow timersStyleWindow)
    {
        windows.Remove(timersStyleWindow);
        windowSystem.RemoveWindow(timersStyleWindow);
    }
}