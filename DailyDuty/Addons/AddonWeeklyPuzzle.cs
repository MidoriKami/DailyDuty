using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using DailyDuty.System;
using Dalamud.Utility.Signatures;
using KamiLib.Hooking;

namespace DailyDuty.Addons;

public unsafe class AddonWeeklyPuzzle : IDisposable
{
    private static AddonWeeklyPuzzle? _instance;
    public static AddonWeeklyPuzzle Instance => _instance ??= new AddonWeeklyPuzzle();
    
    public event EventHandler<nint>? Show;

    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 30 48 8B F9 48 81 C1", DetourName = nameof(OnSetup))]
    private readonly Hook<Delegates.Addon.OnSetup>? onSetupHook = null;

    private AddonWeeklyPuzzle()
    {
        SignatureHelper.Initialise(this);
        AddonManager.AddAddon(this);
        
        onSetupHook?.Enable();
    }

    public void Dispose()
    {
        onSetupHook?.Dispose();
    }
    
    public nint OnSetup(AtkUnitBase* root, int valueCount, AtkValue* values)
    {
        Safety.ExecuteSafe(() =>
        {
            Show?.Invoke(this, new nint(root));
        });
        
        return onSetupHook!.Original(root, valueCount, values);
    }
}