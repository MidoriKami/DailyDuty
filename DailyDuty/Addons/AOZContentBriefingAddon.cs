using System;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons;

public unsafe class AOZContentBriefingAddon : IDisposable
{
    private delegate IntPtr AddonOnSetup(AtkUnitBase* addon);

    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 55 41 54 41 55 41 56 41 57 48 8D 6C 24 ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 30 48 8B D9", DetourName = nameof(OnSetup))]
    private readonly Hook<AddonOnSetup>? onSetupHook = null!;

    public event EventHandler? Setup;
    
    public AOZContentBriefingAddon()
    {
        SignatureHelper.Initialise(this);
        
        onSetupHook.Enable();
    }
    
    public void Dispose()
    {
        onSetupHook?.Dispose();
    }

    private IntPtr OnSetup(AtkUnitBase* addon)
    {
        var result = onSetupHook!.Original(addon);

        try
        {
            Setup?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Something when wrong on AOZContentBriefing Setup");
        }

        return result;
    }
}