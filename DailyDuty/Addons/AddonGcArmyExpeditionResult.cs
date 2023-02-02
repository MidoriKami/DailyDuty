using System;
using System.Linq;
using DailyDuty.System;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;
using KamiLib.Caching;
using KamiLib.Hooking;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Addons;

public record ExpeditionResultArgs(uint MissionType, bool Successful);

public unsafe class AddonGcArmyExpeditionResult : IDisposable
{
    private static AddonGcArmyExpeditionResult? _instance;
    public static AddonGcArmyExpeditionResult Instance => _instance ??= new AddonGcArmyExpeditionResult();
    
    [Signature("48 89 5C 24 ?? 55 56 57 41 56 41 57 48 83 EC 30 44 8B FA", DetourName = nameof(OnSetup))]
    private readonly Hook<Delegates.Addon.OnSetup>? onSetupHook = null!;

    public event EventHandler<ExpeditionResultArgs>? Setup;

    private AddonGcArmyExpeditionResult()
    {
        SignatureHelper.Initialise(this);
        
        AddonManager.AddAddon(this);

        onSetupHook.Enable();
    }

    public void Dispose()
    {
        onSetupHook?.Dispose();
    }

    private nint OnSetup(AtkUnitBase* addon, int valueCount, AtkValue* values)
    {
        var result = onSetupHook!.Original(addon, valueCount, values);

        Safety.ExecuteSafe(() =>
        {
            var dutyName = values[4].GetString();

            var duty = LuminaCache<GcArmyExpedition>.Instance
                .Where(row => row.Name.RawString == dutyName)
                .FirstOrDefault();

            if (duty != null)
            {
                Setup?.Invoke(this, new ExpeditionResultArgs(duty.GcArmyExpeditionType.Row, values[2].Int == 1));
            }
        });
        
        return result;
    }
}