namespace DailyDuty.Interfaces;

/// <summary>
/// Interface for elements that can be updated in a Framework.OnUpdate event.
/// </summary>
/// <remarks>
/// It is unsafe to do UI updates via this interface.
/// </remarks>
public interface IFrameworkUpdatable {

    /// <summary>
    /// Called each frame during FrameworkUpdate.
    /// </summary>
    void Update();
}
