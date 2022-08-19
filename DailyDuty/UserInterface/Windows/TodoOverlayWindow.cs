using System;
using DailyDuty.Utilities;
using Dalamud.Interface.Windowing;

namespace DailyDuty.UserInterface.Windows;

internal class TodoOverlayWindow : Window, IDisposable
{
    public TodoOverlayWindow() : base("###DailyDutyTodoOverlayWindow")
    {
    }

    public void Dispose()
    {
    }

    public override void PreOpenCheck()
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
        if(Condition.InCutsceneOrQuestEvent()) IsOpen = false;
    }

    public override void PreDraw()
    {
    }

    public override void Draw()
    {
    }

    public override void PostDraw()
    {
    }


}