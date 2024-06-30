using DailyDuty.Localization;
using DailyDuty.Models;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Ipc.Exceptions;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Classes;

public class TeleporterController {
    private readonly ICallGateSubscriber<uint, byte, bool> teleportIpc = Service.PluginInterface.GetIpcSubscriber<uint, byte, bool>("Teleport");
    private readonly ICallGateSubscriber<bool> showChatMessageIpc = Service.PluginInterface.GetIpcSubscriber<bool>("Teleport.ChatMessage");

    public void Teleport(Aetheryte aetheryte) {
        try {
            var didTeleport = teleportIpc.InvokeFunc(aetheryte.RowId, (byte) aetheryte.SubRowId);
            var showMessage = showChatMessageIpc.InvokeFunc();

            if (!didTeleport) {
                UserError(Strings.CannotTeleportNow);
            }
            else if (showMessage) {
                StatusMessage.PrintTaggedMessage(string.Format(Strings.TeleportingTo, aetheryte.AethernetName.Value?.Name ?? "Unable to read name"), Strings.Teleport);
            }
        }
        catch (IpcNotReadyError) {
            Service.Log.Error("Teleport IPC not found");
            UserError(Strings.InstallTeleporterError);
        }
    }

    private void UserError(string error) {
        Service.Chat.PrintError(error);
        Service.Toast.ShowError(error);
    }
}