using System.Threading.Tasks;
using DailyDuty.Enums;

namespace DailyDuty.Interfaces;

/// <summary>
/// Represents an element that can be toggled.
/// </summary>
public interface IToggleable : INamed {

    /// <summary>
    /// Gets if this element is toggled on or off.
    /// </summary>
    bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the elements current load state.
    /// </summary>
    State LoadState { get; set; }

    /// <summary>
    /// Gets or sets a message describing a load error.
    /// </summary>
    string? LoadErrorMessage { get; set; }

    /// <summary>
    /// Enable this element.
    /// </summary>
    Task EnableAsync();

    /// <summary>
    /// Disable this element.
    /// </summary>
    Task DisableAsync();

    /// <summary>
    /// Toggle this element.
    /// </summary>
    async Task Toggle() {
        if (!Enabled) {
            await EnableAsync();
        }
        else {
            await DisableAsync();
        }
    }
}
