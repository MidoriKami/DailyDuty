using DailyDuty.Classes;

namespace DailyDuty.Interfaces;

/// <summary>
/// Interface for a non-module feature. This is things like the Timers Overlay.
/// </summary>
public interface IFeature : IToggleable;

public abstract class Feature<T> where T : FeatureConfigFile<T>, new();
