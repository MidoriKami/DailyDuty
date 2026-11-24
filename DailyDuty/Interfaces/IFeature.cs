using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Enums;
using KamiToolKit.Controls.Builders;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Interfaces;

/// <summary>
/// Interface for a non-module feature. This is things like the Timers Overlay.
/// </summary>
public interface IFeature : IToggleable, IConfigurable {

    /// <summary>
    /// Gets the current state of this feature.
    /// </summary>
    State State { get; set; }

    /// <summary>
    /// Gets an error message if the feature encountered an error.
    /// </summary>
    ReadOnlySeString? ErrorMessage { get; set; }

    /// <summary>
    /// Gets whether this feature is currently active.
    /// </summary>
    bool IsActive();

    /// <summary>
    /// Update config ui, for cases when features depend on modules that can be unloaded/loaded.
    /// </summary>
    void UpdateConfigUi();

}

/// <summary>
/// Abstract base class for features that use a config file.
/// </summary>
/// <typeparam name="T">The config file type.</typeparam>
public abstract class Feature<T> : IFeature where T : FeatureConfigFile<T>, new() {

    /// <inheritdoc/>
    public abstract string DisplayName { get; }

    /// <summary>
    /// Gets the config file associated with this feature.
    /// </summary>
    public abstract T Config { get; protected set; }

    /// <inheritdoc/>
    public State State { get; set; }

    /// <inheritdoc/>
    public ReadOnlySeString? ErrorMessage { get; set; }

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

    /// <inheritdoc/>
    public virtual bool IsActive()
        => State is State.Enabled;

    /// <inheritdoc/>
    public abstract void BuildConfigUi(NodeBuilder builder);

    /// <inheritdoc/>
    public virtual void UpdateConfigUi() { }
}
