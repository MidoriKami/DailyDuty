using DailyDuty.System.Localization;
using Dalamud.Logging;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Ipc.Exceptions;
using KamiLib.ChatCommands;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class TeleporterController
{
    private static TeleporterController? _instance;
    public static TeleporterController Instance => _instance ??= new TeleporterController();
    
    private readonly ICallGateSubscriber<uint, byte, bool> teleportIpc;
    private readonly ICallGateSubscriber<bool> showChatMessageIpc;
    
    private TeleporterController()
    {
        teleportIpc = Service.PluginInterface.GetIpcSubscriber<uint, byte, bool>("Teleport");
        showChatMessageIpc = Service.PluginInterface.GetIpcSubscriber<bool>("Teleport.ChatMessage");
    }
    
    public void Teleport(Aetheryte aetheryte)
    {
        try
        {
            var didTeleport = teleportIpc.InvokeFunc(aetheryte.RowId, (byte) aetheryte.SubRowId);
            var showMessage = showChatMessageIpc.InvokeFunc();

            if (!didTeleport)
            {
                UserError(Strings.CannotTeleportNow);
            }
            else if (showMessage)
            {
                Chat.Print(Strings.Teleport, string.Format(Strings.TeleportingTo, aetheryte.AethernetName.Value?.Name ?? "Unable to read name"));
            }
        }
        catch (IpcNotReadyError)
        {
            PluginLog.Error("Teleport IPC not found");
            UserError(Strings.InstallTeleporterError);
        }
    }

    private void UserError(string error)
    {
        Service.Chat.PrintError(error);
        Service.Toast.ShowError(error);
    }
}