using System;
using System.Linq;
using DailyDuty.Utilities;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons;

public record ExpeditionResultArgs(int MissionType, bool Successful);

public unsafe class GcArmyExpeditionResult : IDisposable
{
    private delegate IntPtr AddonOnSetup(AtkUnitBase* addon, uint valueCount, AtkValue* values);
    [Signature("48 89 5C 24 ?? 55 56 57 41 56 41 57 48 83 EC 30 44 8B FA", DetourName = nameof(OnSetup))]
    private readonly Hook<AddonOnSetup>? onSetupHook = null!;

    public event EventHandler<ExpeditionResultArgs>? Setup; 

    public GcArmyExpeditionResult()
    {
        SignatureHelper.Initialise(this);

        onSetupHook.Enable();
    }

    public void Dispose()
    {
        onSetupHook?.Dispose();
    }

    private IntPtr OnSetup(AtkUnitBase* addon, uint valueCount, AtkValue* values)
    {
        var result = onSetupHook!.Original(addon, valueCount, values);

        try
        {
#if DEBUG
            PluginLog.Debug($"Argument Count: {valueCount}");
            foreach (var index in Enumerable.Range(0, (int)valueCount))
            {
                AtkValueHelper.PrintAtkValue(values[index], index);
            }
#endif
            
            Setup?.Invoke(this, new ExpeditionResultArgs(values[6].Int, values[2].Int == 1));
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Something when wrong on GcArmyExpeditionResult Setup");
        }

        return result;
    }
}