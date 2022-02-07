using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.WondrousTails;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.WeeklySettings;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Utility.Signatures;
using ImGuiNET;
#pragma warning disable CS0649

namespace DailyDuty.Modules.Weekly;

internal unsafe class WondrousTails : 
    IConfigurable, 
    IZoneChangeAlwaysNotification,
    IZoneChangeThrottledNotification,
    ILoginNotification,
    ICompletable,
    IUpdateable
{
    private WondrousTailsSettings Settings => Service.Configuration.Current().WondrousTails;
    public CompletionType Type => CompletionType.Weekly;
    public string HeaderText => "Wondrous Tails";
    public GenericSettings GenericSettings => Settings;

    private readonly Stopwatch delayStopwatch = new();

    private uint lastDutyInstanceID = 0;
    private bool lastInstanceWasDuty = false;

    [Signature("88 05 ?? ?? ?? ?? 8B 43 18", ScanType = ScanType.StaticAddress)]
    private readonly WondrousTailsStruct* wondrousTails;

    public WondrousTails()
    {
        SignatureHelper.Initialise(this);

        Settings.NumPlacedStickers = wondrousTails->Stickers;
    }

    public bool IsCompleted()
    {
        return IsBookComplete();
    }

    public void SendNotification()
    {
        if (Settings.Enabled && Settings.NewBookNotification)
        {
            NewBookNotification();
        }

        if (Settings.Enabled && Settings.RerollNotificationTasks && wondrousTails->SecondChance == 9 && !IsBookComplete())
        {
            if (RerollValid())
            {
                Chat.Print(HeaderText, "You have 9 Second-chance points, you can re-roll your stickers/tasks");
            }
        }
    }
    
    void IZoneChangeAlwaysNotification.SendNotification()
    {
        if (Settings.Enabled == false) return;

        if (Condition.IsBoundByDuty() && Settings.InstanceStartNotification == true && !IsBookComplete())
        {
            var e = Service.ClientState.TerritoryType;
            lastInstanceWasDuty = true;
            lastDutyInstanceID = e;
            OnDutyStartNotification();
        }
        else if(lastInstanceWasDuty == true && Settings.InstanceEndNotification == true && !IsBookComplete())
        {
            OnDutyEndNotification();
            lastInstanceWasDuty = false;
        }
        else
        {
            lastInstanceWasDuty = false;
        }
    }

    private void OnDutyEndNotification()
    {
        var node = FindNode(lastDutyInstanceID);
        if (node == null) return;

        var buttonState = node.Value.Item1;

        if (buttonState is ButtonState.Completable or ButtonState.AvailableNow)
        {
            Chat.Print(HeaderText, "You can claim a stamp for the last instance!");
        }
    }

    private void OnDutyStartNotification()
    {
        var node = FindNode(lastDutyInstanceID);
        if (node == null) return;

        var buttonState = node.Value.Item1;

        switch (buttonState)
        {
            case ButtonState.Unavailable:
                if (wondrousTails->SecondChance > 0)
                {
                    Chat.Print(HeaderText, $"This instance is available for a stamp if you re-roll it! You have {wondrousTails->SecondChance} Re-Rolls Available");
                }
                break;

            case ButtonState.AvailableNow:
                Chat.Print(HeaderText, "A stamp is already available for this instance");
                break;

            case ButtonState.Completable:
                Chat.Print(HeaderText, "Completing this instance will reward you with a stamp!");
                break;

            case ButtonState.Unknown:
                break;
        }
    }

    public void NotificationOptions()
    {
        Draw.NotificationField("Duty Start Notification",HeaderText, ref Settings.InstanceStartNotification, "When you join a duty, send a notification if the joined duty is available for a Wondrous Tails sticker");

        Draw.NotificationField("Duty End Reminder", HeaderText, ref Settings.InstanceEndNotification, "When exiting a duty, send a notification if the previous duty is now eligible for a sticker");

        Draw.NotificationField("Reroll Alert - Stickers", HeaderText, ref Settings.RerollNotificationStickers, "When changing zones, send a notification if you have the maximum number of second chance points\n" +
            "and between 3 and 7 stickers (inclusive)\n" +
            "Useful to re-roll the stickers for a chance at better rewards, while preventing over-capping");

        Draw.NotificationField("Reroll Alert - Tasks", HeaderText, ref Settings.RerollNotificationTasks, "When changing zones, send a notification if you have the maximum number of second chance points and have a task available to reroll\n" +
            "Useful to re-roll tasks that you might want to complete later");

        Draw.NotificationField("New Book Alert", HeaderText, ref Settings.NewBookNotification, "Notify me that a new book is available if I have a completed book");

        Settings.ZoneChangeReminder = Settings.InstanceEndNotification || Settings.InstanceStartNotification || Settings.RerollNotificationTasks;
        Settings.LoginReminder = Settings.NewBookNotification;
    }

    public void EditModeOptions()
    {
    }

    public void DisplayData()
    {
        PrintBookStatus();
    }

    public void Update()
    {
        if (Settings.Enabled == false) return;

        Time.UpdateDelayed(delayStopwatch, TimeSpan.FromSeconds(5), UpdateNumStamps );
    }

    public void Dispose()
    {
    }
    
    //
    //  Implementation
    // 

    private void UpdateNumStamps()
    {
        var numStickers = wondrousTails->Stickers;

        if (Settings.NumPlacedStickers != numStickers)
        {
            Settings.NumPlacedStickers = wondrousTails->Stickers;
            Settings.CompletionDate = DateTime.UtcNow;
            Settings.WeeklyKey = wondrousTails->WeeklyKey;
            Service.Configuration.Save();
        }
    }

    private void NewBookNotification()
    {
        // If we haven't set the key yet, set it.
        if (Settings.WeeklyKey == 0)
        {
            Settings.WeeklyKey = wondrousTails->WeeklyKey;
            Settings.CompletionDate = DateTime.UtcNow;
            Service.Configuration.Save();
        }

        // If the completion time is before the previous reset
        if (Settings.CompletionDate < Time.NextWeeklyReset().AddDays(-7))
        {
            // And we are still using the old key
            if (Settings.WeeklyKey == wondrousTails->WeeklyKey)
            {
                Chat.Print(HeaderText, "A new Wondrous Tails Book is Available");
            }
        }
    }

    private bool IsBookComplete()
    {
        return wondrousTails->Stickers == 9;
    }

    private bool RerollValid()
    {
        if (Settings.RerollNotificationTasks)
        {
            for (int i = 0; i < 16; ++i)
            {
                var status = wondrousTails->TaskStatus(i);
                if (status == ButtonState.AvailableNow || status == ButtonState.Unavailable)
                    return true;
            }
        }

        if (Settings.RerollNotificationStickers)
        {
            // We can reroll if any tasks are incomplete
            // We can spend re-rolls if we have more than 7 stickers
            if (wondrousTails->Stickers is >= 3 and <= 7)
            {
                return true;
            }
        }

        return false;
    }
    private IEnumerable<(ButtonState, List<uint>)> GetAllTaskData()
    {
        var result = new (ButtonState, List<uint>)[16];

        for (int i = 0; i < 16; ++i)
        {
            var taskButtonState = wondrousTails->TaskStatus(i);
            var instances = WondrousTailsTaskLookup.GetInstanceListFromID(wondrousTails->Tasks[i]);

            result[i] = (taskButtonState, instances);
        }

        return result;
    }

    private (ButtonState, List<uint>)? FindNode(uint instanceID)
    {
        foreach (var (buttonState, instanceList) in GetAllTaskData())
        {
            if (instanceList.Contains(instanceID))
            {
                return (buttonState, instanceList);
            }
        }

        return null;
    }

    private void PrintBookStatus()
    {
        ImGui.Text("Book Status");
        ImGui.SameLine();

        Draw.DrawConditionalText(Settings.NumPlacedStickers == 9, "Complete", "Incomplete");
    }
}