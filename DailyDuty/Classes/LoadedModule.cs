using DailyDuty.Enums;

namespace DailyDuty.Classes;

public class LoadedModule (ModuleBase moduleBase, LoadedState state = LoadedState.Unknown) {
    public ModuleBase ModuleBase { get; set; } = moduleBase;
    public LoadedState State { get; set; } = state;
    public string ErrorMessage { get; set; } = string.Empty;

    public string Name => ModuleBase.ModuleInfo.DisplayName;
}
