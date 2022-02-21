using System;
using System.Collections.Generic;
using DailyDuty.Data.Enums;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Ipc.Exceptions;

namespace DailyDuty.System
{
    public class TeleportManager : IDisposable
    {
        // Shamelessly Stolen from Goat
        // https://github.com/goaaats/Dalamud.FindAnything/blob/a74cd2bd23997b9ffa6c573abb3c30cdc4798b9b/Dalamud.FindAnything/FindAnythingPlugin.cs
        private readonly ICallGateSubscriber<uint, byte, bool> teleportIpc;

        private readonly Dictionary<TeleportPayloads, DalamudLinkPayload> payloads = new();

        public TeleportManager()
        {
            teleportIpc = Service.PluginInterface.GetIpcSubscriber<uint, byte, bool>("Teleport");

            payloads.Add(TeleportPayloads.GoldSaucerTeleport, AddPayload(TeleportPayloads.GoldSaucerTeleport));
            payloads.Add(TeleportPayloads.IdyllshireTeleport, AddPayload(TeleportPayloads.IdyllshireTeleport));

        }

        private DalamudLinkPayload AddPayload(TeleportPayloads payload)
        {
            return Service.PluginInterface.AddChatLinkHandler((uint)payload, HandleTeleport);
        }

        public void Dispose()
        {
            foreach(var key in payloads.Keys)
            {
                Service.PluginInterface.RemoveChatLinkHandler((uint)key);
            }
        }
        
        public DalamudLinkPayload GetPayload(TeleportPayloads payload)
        {
            return payloads[payload];
        }

        private void HandleTeleport(uint command, SeString message)
        {
            switch ((TeleportPayloads) command)
            {
                case TeleportPayloads.IdyllshireTeleport:
                    Teleport(AetheryteHelper.Get(TeleportLocation.Idyllshire));
                    break;

                case TeleportPayloads.GoldSaucerTeleport:
                    Teleport(AetheryteHelper.Get(TeleportLocation.GoldSaucer));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, "Un-configured Teleport Location");
            }
        }

        private void Teleport(AetheryteEntry aetheryte)
        {
            try
            {
                var didTeleport = teleportIpc.InvokeFunc(aetheryte.AetheryteId, aetheryte.SubIndex);

                if (!didTeleport)
                {
                    UserError("Cannot teleport in this situation.");
                }
                else
                {
                    Chat.Print("Teleport", $"Teleporting to {GetAetheryteName(aetheryte)}...");
                }
            }
            catch (IpcNotReadyError)
            {
                PluginLog.Error("Teleport IPC not found.");
                UserError("To use the teleport function, you must install the \"Teleporter\" plugin.");
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
            if (gameData == null) return "[Name Lookup Failed]";

            var placeName = gameData.PlaceName.Value;
            if (placeName == null) return "[Name Lookup Failed]";

            return placeName.Name;
        }
    }
}
