using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Configuration;
using DailyDuty.Configuration.Components;
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
        new TimersConfigurationWindow(),
        new TimersOverlayWindow(),
    };

    public WindowManager()
    {
        foreach (var window in windows)
        {
            windowSystem.AddWindow(window);
        }

        Service.PluginInterface.UiBuilder.Draw += DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        Service.ConfigurationManager.OnCharacterDataAvailable += LoginListener;

        if (Service.ConfigurationManager.CharacterDataLoaded)
        {
            LoginListener(this, Service.ConfigurationManager.CharacterConfiguration);
        }
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
        Service.ConfigurationManager.OnCharacterDataAvailable -= LoginListener;

        foreach (var window in windows.OfType<IDisposable>())
        {
            window.Dispose();
        }

        windowSystem.RemoveAllWindows();
    }

    private void LoginListener(object? sender, CharacterConfiguration e)
    {
        AddTodoOverlayWindow();
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

        timersStyleWindow.Dispose();
    }

    public void AddTodoOverlayWindow()
    {
        var newWindow = new TodoOverlayWindow();

        windows.Add(newWindow);
        windowSystem.AddWindow(newWindow);
    }

    public void RemoveTodoOverlayWindow(TodoOverlayWindow todoOverlayWindow)
    {
        windows.Remove(todoOverlayWindow);
        windowSystem.RemoveWindow(todoOverlayWindow);

        todoOverlayWindow.Dispose();
    }
}