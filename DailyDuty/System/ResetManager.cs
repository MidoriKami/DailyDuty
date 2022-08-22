using System;
using System.Threading;

namespace DailyDuty.System;

internal class ResetManager : IDisposable
{
    private readonly CancellationTokenSource cancellationToken = new();

    public ResetManager()
    {
        Service.Framework.RunOnTick(FrameworkOnUpdate,TimeSpan.FromSeconds(5), cancellationToken: cancellationToken.Token);
    }

    private void FrameworkOnUpdate()
    {
        Service.Framework.RunOnTick(FrameworkOnUpdate,TimeSpan.FromSeconds(5), cancellationToken: cancellationToken.Token);

        if (Service.ConfigurationManager.CharacterDataLoaded)
        {
            foreach (var module in Service.ModuleManager.GetLogicComponents())
            {
                var now = DateTime.UtcNow;

                if (now >= module.ParentModule.GenericSettings.NextReset)
                {
                    module.DoReset();
                    module.ParentModule.GenericSettings.NextReset = module.GetNextReset();
                    Service.ConfigurationManager.Save();
                }
            }
        }
    }

    public void Dispose()
    {
        cancellationToken.Cancel();
    }
}