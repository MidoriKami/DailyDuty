using DailyDuty.Enums;

namespace DailyDuty.Classes;

public class LoadedModule(FeatureBase featureBase, LoadedState state = LoadedState.Unknown) {
    public FeatureBase FeatureBase { get; set; } = featureBase;
    public LoadedState State { get; set; } = state;
    public string ErrorMessage { get; set; } = string.Empty;

    public string Name => FeatureBase.ModuleInfo.DisplayName;
}
