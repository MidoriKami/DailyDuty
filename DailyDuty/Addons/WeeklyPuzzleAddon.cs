using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using DailyDuty.System;
using Dalamud.Utility.Signatures;
using KamiLib.ExceptionSafety;

namespace DailyDuty.Addons;

public unsafe class WeeklyPuzzleAddon : IDisposable
{
    private static WeeklyPuzzleAddon? _instance;
    public static WeeklyPuzzleAddon Instance => _instance ??= new WeeklyPuzzleAddon();
    
    public event EventHandler<nint>? Show;

    private delegate nint WeeklyPuzzleOnSetup(AtkUnitBase* root);
    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 30 48 8B F9 48 81 C1", DetourName = nameof(WeeklyPuzzle_OnSetup))]
    private readonly Hook<WeeklyPuzzleOnSetup>? onSetupHook = null;

    private WeeklyPuzzleAddon()
    {
        SignatureHelper.Initialise(this);
        AddonManager.AddAddon(this);
        
        onSetupHook?.Enable();
    }

    public void Dispose()
    {
        onSetupHook?.Dispose();
    }
    
    public nint WeeklyPuzzle_OnSetup(AtkUnitBase* root)
    {
        Safety.ExecuteSafe(() =>
        {
            Show?.Invoke(this, new nint(root));
        });
        
        return onSetupHook!.Original(root);
    }
}