using DailyDuty.Interfaces;

namespace DailyDuty.Classes;

/// <summary>
/// Config data file for a <see cref="IFeature"/>
/// </summary>
public abstract class FeatureConfigFile<T>  : FileBase<T> where T: FeatureConfigFile<T>, new();
