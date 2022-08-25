using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.System.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Ipc.Exceptions;
using Dalamud.Utility;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

internal enum TeleportLocation
{
    GoldSaucer,
    Idyllshire,
    DomanEnclave
}

internal record TeleportInfo(uint CommandID, TeleportLocation Target, Aetheryte Aetherite);

internal record TeleportLinkPayloads(TeleportLocation Location, DalamudLinkPayload Payload);

internal class TeleportManager : IDisposable
{
    private readonly ICallGateSubscriber<uint, byte, bool> teleportIpc;
    private readonly ICallGateSubscriber<bool> showChatMessageIpc;

    private readonly List<TeleportInfo> teleportInfoList = new()
    {
        new TeleportInfo(1, TeleportLocation.GoldSaucer, GetAetheryte(62)),
        new TeleportInfo(2, TeleportLocation.Idyllshire, GetAetheryte(75)),
        new TeleportInfo(3, TeleportLocation.DomanEnclave, GetAetheryte(127)),
    };

    private List<TeleportLinkPayloads> ChatLinkPayloads { get; } = new();

    public TeleportManager()
    {
        teleportIpc = Service.PluginInterface.GetIpcSubscriber<uint, byte, bool>("Teleport");
        showChatMessageIpc = Service.PluginInterface.GetIpcSubscriber<bool>("Teleport.ChatMessage");

        foreach (var teleport in teleportInfoList)
        {
            Service.PluginInterface.RemoveChatLinkHandler(teleport.CommandID);

            var linkPayload = Service.PluginInterface.AddChatLinkHandler(teleport.CommandID, TeleportAction);

            ChatLinkPayloads.Add(new TeleportLinkPayloads(teleport.Target, linkPayload));
        }
    }

    public void Dispose()
    {
        foreach (var payload in teleportInfoList)
        {
            Service.PluginInterface.RemoveChatLinkHandler(payload.CommandID);
        }
    }

    private void TeleportAction(uint command, SeString message)
    {
        var teleportInfo = teleportInfoList.First(teleport => teleport.CommandID == command);

        if (AetheryteUnlocked(teleportInfo.Aetherite, out var targetAetheriteEntry))
        {
            Teleport(targetAetheriteEntry!);
        }
        else
        {
            PluginLog.Error("User attempted to teleport to an aetheryte that is not unlocked");
            UserError(Strings.UserInterface.Teleport.NotUnlocked);
        }
    }

    public DalamudLinkPayload GetPayload(TeleportLocation targetLocation)
    {
        return ChatLinkPayloads.First(payload => payload.Location == targetLocation).Payload;
    }

    private void Teleport(AetheryteEntry aetheryte)
    {
        try
        {
            var didTeleport = teleportIpc.InvokeFunc(aetheryte.AetheryteId, aetheryte.SubIndex);
            var showMessage = showChatMessageIpc.InvokeFunc();

            if (!didTeleport)
            {
                UserError(Strings.UserInterface.Teleport.Error);
            }
            else if (showMessage)
            {
                Chat.Print(Strings.UserInterface.Teleport.Label, Strings.UserInterface.Teleport.Teleporting.Format(GetAetheryteName(aetheryte)));
            }
        }
        catch (IpcNotReadyError)
        {
            PluginLog.Error("Teleport IPC not found");
            UserError(Strings.UserInterface.Teleport.CommunicationError);
        }
    }

    private void UserError(string error)
    {
        Service.Chat.PrintError(error);
        Service.Toast.ShowError(error);
    }

    private string GetAetheryteName(AetheryteEntry aetheryte)
    {
        var gameData = aetheryte.AetheryteData.GameData;
        var placeName = gameData?.PlaceName.Value;

        return placeName == null ? "[Name Lookup Failed]" : placeName.Name;
    }

    private static Aetheryte GetAetheryte(uint id)
    {
        return Service.DataManager.GetExcelSheet<Aetheryte>()!.GetRow(id)!;
    }

    private bool AetheryteUnlocked(ExcelRow aetheryte, out AetheryteEntry? entry)
    {
        if (Service.AetheryteList.Any(entry => entry.AetheryteId == aetheryte.RowId))
        {
            entry = Service.AetheryteList.Where(entry => entry.AetheryteId == aetheryte.RowId).First();
            return true;
        }
        else
        {
            entry = null;
            return false;
        }
    }
}
