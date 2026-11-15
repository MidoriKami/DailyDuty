using System;

namespace DailyDuty.Interfaces;

/// <summary>
/// Base interface to representing a resettable element.
/// </summary>
public interface IResettable {

    /// <summary>
    /// Gets whether this element needs to be reset.
    /// </summary>
    /// <returns></returns>
    bool IsResetDue();

    /// <summary>
    /// Resets this element.
    /// </summary>
    void Reset();

    /// <summary>
    /// Gets the datetime of when this module needs to be reset next.
    /// </summary>
    DateTime NextResetTime { get; }

    /// <summary>
    /// Gets the duration of the reset cycle.
    /// </summary>
    TimeSpan ResetPeriod { get; }

    /// <summary>
    /// Gets the DateTime of the next reset for this module.
    /// </summary>
    DateTime GetNextResetTime();
}
