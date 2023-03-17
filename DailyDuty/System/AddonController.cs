using System;
using Dalamud.Hooking;
using Dalamud.Memory;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Hooking;

namespace DailyDuty.System;

public unsafe class SetupAddonArgs 
{
    public required AtkUnitBase* Addon { get; init; }
    private string? addonName;
    public string AddonName => addonName ??= MemoryHelper.ReadString(new nint(Addon->Name), 0x20).Split('\0')[0];
}

public unsafe class AddonController : IDisposable
{
    private delegate void* AddonSetupDelegate(AtkUnitBase* addon);
    
    [Signature("E8 ?? ?? ?? ?? 8B 83 ?? ?? ?? ?? C1 E8 14", DetourName = nameof(AddonSetupDetour))]
    private readonly Hook<AddonSetupDelegate>? addonSetupHook = null;
    
    public static event Action<SetupAddonArgs>? AddonSetup; 

    public AddonController()
    {
        SignatureHelper.Initialise(this);
        
        addonSetupHook?.Enable();
    }
    
    public void Dispose()
    {
        addonSetupHook?.Dispose();
    }
    
    private void* AddonSetupDetour(AtkUnitBase* addon) 
    {
        Safety.ExecuteSafe(() =>
        {
            AddonSetup?.Invoke(new SetupAddonArgs
            {
                Addon = addon
            });
        });
        
        return addonSetupHook!.Original(addon);
    }
}