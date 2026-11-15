using System.Threading.Tasks;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features;

public class TimerOverlay : IFeature, IFrameworkUpdatable {

    public string Name => Strings.TimersOverlay_DisplayName;
    public State State { get; set; }
    public ReadOnlySeString? ErrorMessage { get; set; }

    public bool Enabled { get; set; }
    public State LoadState { get; set; }
    public string? LoadErrorMessage { get; set; }

    public Task EnableAsync() {
        return Task.CompletedTask;
    }

    public Task DisableAsync() {
        return Task.CompletedTask;
    }

    public bool IsActive()
        => State is State.Enabled;

    public void Update() {

    }
}
