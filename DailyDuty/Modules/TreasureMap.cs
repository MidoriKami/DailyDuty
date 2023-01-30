using System;
using System.Linq;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Caching;
using KamiLib.Drawing;
using Lumina.Excel.GeneratedSheets;
using Condition = KamiLib.GameState.Condition;

namespace DailyDuty.Modules;

public class TreasureMapSettings : GenericSettings
{            
    public DateTime LastMapGathered;
}

public unsafe class TreasureMap : AbstractModule
{
    public override ModuleName Name => ModuleName.TreasureMap;
    public override CompletionType CompletionType => CompletionType.Daily;

    private static TreasureMapSettings Settings => Service.ConfigurationManager.CharacterConfiguration.TreasureMap;
    public override GenericSettings GenericSettings => Settings;

    private delegate long GetNextMapAvailableTimeDelegate(UIState* uiState);
    [Signature("E8 ?? ?? ?? ?? 48 8B F8 E8 ?? ?? ?? ?? 49 8D 9F")]
    private readonly GetNextMapAvailableTimeDelegate getNextMapUnixTimestamp = null!;

    private static AtkUnitBase* ContentsTimerAgent => (AtkUnitBase*) Service.GameGui.GetAddonByName("ContentsInfo");
    
    public TreasureMap()
    {
        SignatureHelper.Initialise(this);

        Service.Chat.ChatMessage += OnChatMessage;
        Service.Framework.Update += OnFrameworkUpdate;
    }

    public override void Dispose()
    {
        Service.Chat.ChatMessage -= OnChatMessage;
        Service.Framework.Update -= OnFrameworkUpdate;
    }

    private void OnChatMessage(XivChatType type, uint senderID, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!Settings.Enabled) return;

        if ((int)type != 2115 || !Condition.IsGathering())
            return;

        if (message.Payloads.FirstOrDefault(p => p is ItemPayload) is not ItemPayload item)
            return;

        if (!IsMap(item.ItemId))
            return;

        Settings.LastMapGathered = DateTime.UtcNow;
        Service.ConfigurationManager.Save();
    }
    
    private void OnFrameworkUpdate(Framework framework)
    {
        if (ContentsTimerAgent == null) return;

        var nextAvailable = GetNextMapAvailableTime();

        if (nextAvailable != DateTime.MinValue)
        {
            var storedTime = Settings.LastMapGathered;
            storedTime = storedTime.AddSeconds(-storedTime.Second);

            var retrievedTime = nextAvailable;
            retrievedTime = retrievedTime.AddSeconds(-retrievedTime.Second).AddHours(-18);

            if (storedTime != retrievedTime)
            {
                Settings.LastMapGathered = retrievedTime;
                Service.ConfigurationManager.Save();
            }
        }
    }

    public override string GetStatusMessage() => Strings.TreasureMap_MapAvailable;
    public override ModuleStatus GetModuleStatus() => TimeUntilNextMap() == TimeSpan.Zero ? ModuleStatus.Incomplete : ModuleStatus.Complete;
    public override TimeSpan GetTimerPeriod() => TimeSpan.FromHours(18);
    public override DateTime GetNextReset() => Settings.LastMapGathered + TimeSpan.FromHours(18);

    private static bool IsMap(uint itemID) => LuminaCache<TreasureHuntRank>.Instance
        .Any(item => item.ItemName.Row == itemID && item.ItemName.Row != 0);

    private static TimeSpan TimeUntilNextMap()
    {
        var lastMapTime = Settings.LastMapGathered;
        var nextAvailableTime = lastMapTime.AddHours(18);

        if (DateTime.UtcNow >= nextAvailableTime)
        {
            return TimeSpan.Zero;
        }
        else
        {
            return nextAvailableTime - DateTime.UtcNow;
        }
    }

    private static string GetNextTreasureMap()
    {
        var span = TimeUntilNextMap();

        if (span == TimeSpan.Zero)
        {
            return Strings.TreasureMap_MapAvailable;
        }

        return span.FormatTimespan(Settings.TimerSettings.TimerStyle.Value);
    }

    private DateTime GetNextMapAvailableTime()
    {
        var unixTimestamp = getNextMapUnixTimestamp(UIState.Instance());

        return unixTimestamp == -1 ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
    }

    protected override void DrawStatus()
    {
        InfoBox.Instance.DrawGenericStatus(this);

        InfoBox.Instance
            .AddTitle(Strings.TreasureMap_NextMap)
            .BeginTable()
            .BeginRow()
            .AddString(Strings.TreasureMap_NextMap)
            .AddString(GetNextTreasureMap())
            .EndRow()
            .EndTable()
            .Draw();
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}