using System;
using System.Threading.Tasks;
using DailyDuty.Data;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using KamiToolKit.Controls.Builders;
using KamiToolKit.UI.Nodes.ComponentNode;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Interfaces;

public interface IModule : IToggleable, IResettable {

    /// <summary>
    /// Builds the data display ui.
    /// </summary>
    void BuildDataUi(NodeBuilder nodeBuilder);

    /// <summary>
    /// Update any data nodes that need to refresh their data.
    /// </summary>
    void UpdateDataUi();

    /// <summary>
    /// Gets the current completion status of this module.
    /// </summary>
    CompletionStatus GetStatus();

    /// <summary>
    /// Builds the default generic config for all modules.
    /// </summary>
    /// <remarks>
    /// This is implemented by <see cref="Module{T,TU}"/>, and is not meant to be implemented by modules directly.
    /// </remarks>
    void BuildGenericConfig(NodeBuilder builder);

    /// <summary>
    /// Sends the module's login message.
    /// </summary>
    void SendLoginMessage();

    /// <summary>
    /// Sends the module's Zone Changed message.
    /// </summary>
    void SendZoneChangeMessage();

    /// <summary>
    /// Sends the module's reset message.
    /// </summary>
    void SendResetMessage();
}

public abstract class Module<T, TU> : IModule where T : ModuleDataFile<T>, new() where TU : ModuleConfigFile<TU>, new() {

    /// <inheritdoc/>
    public abstract string DisplayName { get; }

    /// <summary>
    /// Gets the data file associated with this module.
    /// </summary>
    /// <remarks>
    /// This contains at minimum the time for the next reset.
    /// </remarks>
    public abstract T Data { get; protected set; }

    /// <summary>
    /// Gets the config file associated with this module.
    /// </summary>
    /// <remarks>
    /// This contains at minimum settings values for chat notifications, and the snooze state.
    /// </remarks>
    public abstract TU Config { get; protected set; }

    /// <inheritdoc/>
    public bool Enabled { get; set; }

    /// <inheritdoc/>
    public State LoadState { get; set; }

    /// <inheritdoc/>
    public string? LoadErrorMessage { get; set; }

    /// <inheritdoc/>
    public abstract Task EnableAsync();

    /// <inheritdoc/>
    public abstract Task DisableAsync();

    /// <summary>
    /// Builds the data ui to display the modules current state.
    /// </summary>
    public abstract void BuildDataUi(NodeBuilder builder);

    /// <summary>
    /// Update any data nodes that need to refresh their data.
    /// </summary>
    public abstract void UpdateDataUi();

    /// <inheritdoc/>
    public bool IsResetDue()
        => GetNextResetTimeInternal().ToUniversalTime() < DateTime.UtcNow;

    private DateTime GetNextResetTimeInternal() => this switch {
        IDailyResettable => Time.NextDailyReset(),
        IWeeklyResettable => Time.NextWeeklyReset(),
        ISpecialResettable => GetSpecialNextResetTime(),
        _ => Data.NextReset,
    };

    /// <summary>
    /// Resets this modules data and properties.
    /// </summary>
    /// <returns></returns>
    protected abstract Task ResetAsync();

    /// <summary>
    /// Determine the modules completion status.
    /// </summary>
    /// <remarks>
    /// Might consider caching this instead, as some modules may perform heavy work to determine this.
    /// </remarks>
    protected abstract CompletionStatus GetCompletionStatus();

    /// <summary>
    /// Send a chat message reporting the modules current status.
    /// </summary>
    /// <param name="message">Nullable message, this is the user override message if not null.</param>
    protected abstract void SendStatusMessage(ReadOnlySeString? message);

    /// <summary>
    /// Send a chat message reporting that the module was reset.
    /// </summary>
    /// <param name="message">Nullable message, this is the user override message if not null.</param>
    protected abstract void SendResetMessage(ReadOnlySeString? message);

    /// <summary>
    /// Gets the reset period for non-standard modules.
    /// </summary>
    protected virtual TimeSpan GetSpecialResetPeriod() => TimeSpan.FromDays(1);

    /// <summary>
    /// Gets the next reset time for non-standard modules.
    /// </summary>
    protected virtual DateTime GetSpecialNextResetTime() => Data.NextReset;

    /// <summary>
    /// Gets the event window end time override for modules with a separate window end from reset time.
    /// Returns null by default (meaning window end equals reset time).
    /// </summary>
    protected virtual DateTime? GetWindowEndTimeOverride() => null;

    private TextButtonNode? suppressionButton;

    private bool IsActive => LoadState is State.Enabled && !Config.Suppressed;

    CompletionStatus IModule.GetStatus()
        => Config.Suppressed ? CompletionStatus.Suppressed : GetCompletionStatus();

    void IModule.SendLoginMessage() {
        if (!IsActive) return;
        if (!GetCompletionStatus().RequiresAction) return;

        switch (Config) {
            case { OnLoginMessage: true, CustomStatusMessage: { Length: > 0 } loginMessage }:
                SendStatusMessage(loginMessage);
                break;

            case { OnLoginMessage: true }:
                SendStatusMessage(null);
                break;
        }
    }

    void IModule.SendZoneChangeMessage() {
        if (!IsActive) return;
        if (!GetCompletionStatus().RequiresAction) return;

        switch (Config) {
            case { OnZoneChangeMessage: true, CustomStatusMessage: { Length: > 0 } statusMessage }:
                SendStatusMessage(statusMessage);
                break;

            case { OnZoneChangeMessage: true }:
                SendStatusMessage(null);
                break;
        }
    }

    void IModule.SendResetMessage() {
        if (!IsActive) return;
        if (!GetCompletionStatus().RequiresAction) return;

        switch (Config) {
            case { ResetMessages: true, CustomResetMessage: { Length: > 0 } resetMessage }:
                SendResetMessage(resetMessage);
                break;

            case { ResetMessages: true }:
                SendResetMessage(null);
                break;
        }
    }

    void IModule.BuildGenericConfig(NodeBuilder builder) => builder
        .TextCentered(Strings.NotificationSettingsNode_NotificationSettings, 14)
        .Separator()
        .Spacer(5.0f)
        .Checkbox(Strings.NotificationSettingsNode_LoginStatus, Config.OnLoginMessage, val => {
            Config.OnLoginMessage = val;
            Config.Save();
        })
        .Checkbox(Strings.NotificationSettingsNode_ZoneStatus, Config.OnZoneChangeMessage, val => {
            Config.OnZoneChangeMessage = val;
            Config.Save();
        })
        .Checkbox(Strings.NotificationSettingsNode_ResetStatus, Config.ResetMessages, val => {
            Config.ResetMessages = val;
            Config.Save();
        })
        .Spacer(10.0f)
        .TextCentered(Strings.NotificationSettingsNode_CustomStatus, 14)
        .Separator()
        .Spacer(5.0f)
        .InputText(inputChanged => {
            Config.CustomStatusMessage = inputChanged.ToString();
            Config.Save();
        }, Config.CustomStatusMessage)
        .Spacer(10.0f)
        .TextCentered(Strings.NotificationSettingsNode_CustomReset, 14)
        .Separator()
        .Spacer(5.0f)
        .InputText(inputChanged => {
            Config.CustomResetMessage = inputChanged.ToString();
            Config.Save();
        }, Config.CustomResetMessage)
        .Spacer(10.0f)
        .TextCentered(Strings.NotificationSettingsNode_ChatChannel, 14)
        .Separator()
        .Spacer(5.0f)
        .Combo(Config.MessageChatChannel, channel => {
            Config.MessageChatChannel = channel;
            Config.Save();
        })
        .Spacer(10.0f)
        .TextCentered(Strings.DataNodeBase_SnoozeTooltip, 14)
        .Separator()
        .Spacer(5.0f)
        .TextButton(Config.Suppressed ? Strings.DataNodeBase_Unsnooze : Strings.DataNodeBase_Snooze, () => {
            Config.Suppressed = !Config.Suppressed;
            suppressionButton?.String = Config.Suppressed ? Strings.DataNodeBase_Unsnooze : Strings.DataNodeBase_Snooze;
            Config.Save();
        }, out suppressionButton);

    void IResettable.Reset()
        => Task.Run(async () => {
            await ResetAsync();

            Data.NextReset = default;
            await Data.Save();

            Config.Suppressed = false;
            await Config.Save();
        });

    DateTime IResettable.NextResetTime => Data.NextReset;

    TimeSpan IResettable.ResetPeriod => this switch {
        IDailyResettable => TimeSpan.FromDays(1),
        IWeeklyResettable => TimeSpan.FromDays(7),
        ISpecialResettable => GetSpecialResetPeriod(),
        _ => TimeSpan.FromDays(1),
    };

    DateTime IResettable.GetNextResetTime() => GetNextResetTimeInternal();

    DateTime? IResettable.GetWindowEndTime() => GetWindowEndTimeOverride();
}
