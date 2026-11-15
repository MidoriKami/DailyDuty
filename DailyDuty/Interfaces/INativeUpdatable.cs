namespace DailyDuty.Interfaces;

/// <summary>
/// Interface for elements that should be updated in a NativeAddon's Update event, instead of IFramework's.
/// </summary>
public interface INativeUpdatable {

    /// <summary>
    /// Called each frame during NativeAddon's Update call.
    /// </summary>
    void Update();
}
