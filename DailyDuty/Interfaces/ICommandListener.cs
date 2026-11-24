namespace DailyDuty.Interfaces;

/// <summary>
/// Interface representing something that listens for commands.
/// </summary>
public interface ICommandListener {

    /// <summary>
    /// Try to process the provided command.
    /// </summary>
    void OnCommand(string command, ref bool isHandled);
}
