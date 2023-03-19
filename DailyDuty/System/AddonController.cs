using System;
using Dalamud.Hooking;
using Dalamud.Memory;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Hooking;

namespace DailyDuty.System;

public unsafe class AddonArgs 
{
    public required AtkUnitBase* Addon { get; init; }
    private string? addonName;
    public string AddonName => addonName ??= MemoryHelper.ReadString(new nint(Addon->Name), 0x20).Split('\0')[0];
}

public unsafe class AddonController : IDisposable
{
    private delegate void* AddonSetupDelegate(AtkUnitBase* addon);
    private delegate void AddonFinalizeDelegate(AtkUnitManager* unitManager, AtkUnitBase** atkUnitBase);

    [Signature("E8 ?? ?? ?? ?? 8B 83 ?? ?? ?? ?? C1 E8 14", DetourName = nameof(AddonSetupDetour))]
    private readonly Hook<AddonSetupDelegate>? addonSetupHook = null;

    [Signature("E8 ?? ?? ?? ?? 48 8B 7C 24 ?? 41 8B C6", DetourName = nameof(AddonFinalizeDetour))]
    private readonly Hook<AddonFinalizeDelegate>? addonFinalizeHook = null;
    
    public static event Action<AddonArgs>? AddonPreSetup;     
    public static event Action<AddonArgs>? AddonPostSetup; 
    public static event Action<AddonArgs>? AddonFinalize;

    public AddonController()
    {
        SignatureHelper.Initialise(this);
        
        addonSetupHook?.Enable();
        addonFinalizeHook?.Enable();
    }
    
    public void Dispose()
    {
        addonSetupHook?.Dispose();
        addonFinalizeHook?.Dispose();
    }
    
    private void* AddonSetupDetour(AtkUnitBase* addon) 
    {
        Safety.ExecuteSafe(() =>
        {
            AddonPreSetup?.Invoke(new AddonArgs
            {
                Addon = addon
            });
        });
        
        var result = addonSetupHook!.Original(addon);
        
        Safety.ExecuteSafe(() =>
        {
            AddonPostSetup?.Invoke(new AddonArgs
            {
                Addon = addon
            });
        });

        return result;
    }
    
    private void AddonFinalizeDetour(AtkUnitManager* unitManager, AtkUnitBase** atkUnitBase) 
    {
        Safety.ExecuteSafe(() =>
        {
            AddonFinalize?.Invoke(new AddonArgs
            {
                Addon = atkUnitBase[0]
            });
        });
        
        addonFinalizeHook?.Original(unitManager, atkUnitBase);
    }
}