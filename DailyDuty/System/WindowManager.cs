using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Configuration;
using DailyDuty.UserInterface.Windows;
using DailyDuty.Utilities;
using Dalamud.Interface.Windowing;
using KamiLib.Utilities;

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
        new AboutWindow(),
    };

    public WindowManager()
    {
        foreach (var window in windows)
        {
            windowSystem.AddWindow(window);
        }

        Service.PluginInterface.UiBuilder.Draw += DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        Service.ConfigurationManager.OnCharacterDataLoaded += OnCharacterDataLoaded;
        Service.ConfigurationManager.OnCharacterDataUnloaded += OnCharacterDataUnloaded;

        if (Service.ConfigurationManager.CharacterDataLoaded)
        {
            OnCharacterDataLoaded(this, Service.ConfigurationManager.CharacterConfiguration);
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
        Service.ConfigurationManager.OnCharacterDataLoaded -= OnCharacterDataLoaded;
        Service.ConfigurationManager.OnCharacterDataUnloaded -= OnCharacterDataUnloaded;

        foreach (var window in windows.OfType<IDisposable>())
        {
            window.Dispose();
        }

        windowSystem.RemoveAllWindows();
    }

    private void OnCharacterDataLoaded(object? sender, CharacterConfiguration e)
    {
        AddWindow(new TimersOverlayWindow());
        AddWindow(new TodoOverlayWindow());
    }
    
    private void OnCharacterDataUnloaded(object? sender, EventArgs e)
    {
        var windowList = new List<Window>();
        
        windowList.AddRange(windows.OfType<TimersOverlayWindow>());
        windowList.AddRange(windows.OfType<TodoOverlayWindow>());
        
        foreach (var overlay in windowList)
        {
            RemoveWindow(overlay);
        }
    }

    public void AddWindow(Window newWindow)
    {
        if (!windowSystem.Windows.Any(window => window.WindowName == newWindow.WindowName))
        {
            windows.Add(newWindow);
            windowSystem.AddWindow(newWindow);
        }
    }

    public void RemoveWindow(Window window)
    {
        windows.Remove(window);
        windowSystem.RemoveWindow(window);
    }
}