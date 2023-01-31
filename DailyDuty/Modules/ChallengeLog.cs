using System.Collections.Generic;
using System.Linq;
using DailyDuty.Configuration;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.Caching;
using KamiLib.Configuration;
using KamiLib.Drawing;
using LuminaContentsNote = Lumina.Excel.GeneratedSheets.ContentsNote;

namespace DailyDuty.Modules;

public class ChallengeLogSettings : GenericSettings
{
    public List<TrackedContentNote> TrackedTasks = new();
}

public unsafe class ChallengeLog : AbstractModule
{
    public override ModuleName Name => ModuleName.ChallengeLog;
    public override CompletionType CompletionType => CompletionType.Weekly;
    
    private static ChallengeLogSettings Settings => Service.ConfigurationManager.CharacterConfiguration.ChallengeLog;
    public override GenericSettings GenericSettings => Settings;

    public ChallengeLog()
    {
        Service.Framework.Update += OnFrameworkUpdate;
        Service.ConfigurationManager.OnCharacterDataLoaded += RefreshTrackedTasks;
    }

    public override void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ConfigurationManager.OnCharacterDataLoaded -= RefreshTrackedTasks;
    }
    
    private void OnFrameworkUpdate(Framework framework)
    {
        if (!Settings.Enabled) return;
        if (Settings.Suppressed) return;
        
        foreach (var task in Settings.TrackedTasks)
        {
            task.Completed = ContentsNote.Instance()->IsContentNoteComplete(task.RowID);
        }
    }
    
    private void RefreshTrackedTasks(object? sender, CharacterConfiguration e)
    {
        var luminaData = LuminaCache<LuminaContentsNote>.Instance
            .Where(row => row.RequiredAmount is not 0)
            .ToList();
        
        var configCount = Settings.TrackedTasks.Count;

        // Check for mismatch between saved data and gamedata
        if (luminaData.Count != configCount)
        {
            // Find which entries we are missing and add them
            foreach (var luminaEntry in luminaData)
            {
                // If none of our saved entries has this value add it
                if (!Settings.TrackedTasks.Any(task => task.RowID == luminaEntry.RowId))
                {
                    Settings.TrackedTasks.Add(new TrackedContentNote((int)luminaEntry.RowId, new Setting<bool>(false), false));
                }
            }
        }
    }

    private static int GetIncompleteCount() => Settings.TrackedTasks
        .Where(task => task.Enabled && !task.Completed)
        .Count();

    public override string GetStatusMessage() => $"{GetIncompleteCount()} {Strings.Common_AllowancesAvailable}";

    public override ModuleStatus GetModuleStatus() => Settings.TrackedTasks
        .Where(task => task.Enabled)
        .All(task => task.Completed) 
        ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    public override bool HasLongLabel => true;

    public override string GetLongTaskLabel()
    {
        var strings = Settings.TrackedTasks
            .Where(task => task.Enabled && !task.Completed)
            .OrderBy(task => task.RowID)
            .Select(task => task.GetTaskName())
            .ToList();

        return strings.Any() ? string.Join("\n", strings) : TodoTaskLabel;
    }

    protected override void DrawConfiguration()
    {
        InfoBox.Instance
            .AddTitle(Strings.Config_Options)
            .AddConfigCheckbox(Strings.Common_Enabled, Settings.Enabled)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.GrandCompany_Tracked)
            .AddList(Settings.TrackedTasks)
            .Draw();
        
        InfoBox.Instance.DrawNotificationOptions(this);
    }
    
    protected override void DrawStatus()
    {
        InfoBox.Instance.DrawGenericStatus(this);
        
        if (Settings.TrackedTasks.Any(hunt => hunt.Enabled))
        {
            InfoBox.Instance
                .AddTitle(Strings.GrandCompany_Tracked)
                .BeginTable()
                .AddDataRows(Settings.TrackedTasks.Where(task => task.Enabled))
                .EndTable()
                .Draw();
        }
        else
        {
            InfoBox.Instance
                .AddTitle(Strings.GrandCompany_Tracked, out var innerWidth)
                .AddStringCentered(Strings.MaskedCarnivale_NothingTracked, innerWidth, Colors.Orange)
                .Draw();
        }
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}