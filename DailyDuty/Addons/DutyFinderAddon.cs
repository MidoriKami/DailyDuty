using System;
using DailyDuty.DataModels;
using DailyDuty.System;
using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;
using KamiLib.Hooking;

namespace DailyDuty.Addons;

public unsafe class DutyFinderAddon : IDisposable
{
    private static DutyFinderAddon? _instance;
    public static DutyFinderAddon Instance => _instance ??= new DutyFinderAddon();
    
    public event EventHandler<nint>? Show;
    public event EventHandler<nint>? Draw;
    public event EventHandler<nint>? Finalize;
    public event EventHandler<nint>? Update;
    public event EventHandler<nint>? Refresh;
    public event EventHandler<ReceiveEventArgs>? ReceiveEvent;

    private Hook<Delegates.Addon.Draw>? onDrawHook;
    private Hook<Delegates.Addon.Finalize>? onFinalizeHook;
    private Hook<Delegates.Addon.Update>? onUpdateHook;
    private Hook<Delegates.Addon.OnRefresh>? onRefreshHook;
    private Hook<Delegates.Addon.OnSetup>? onSetupHook;
    private readonly Hook<Delegates.Agent.ReceiveEvent>? onReceiveEventHook;

    private static AtkUnitBase* ContentsFinderAddon => (AtkUnitBase*) Service.GameGui.GetAddonByName("ContentsFinder");

    private DutyFinderAddon()
    {
        AddonManager.AddAddon(this);
        
        Service.Framework.Update += OnFrameworkUpdate;

        var agent = AgentContentsFinder.Instance()->AgentInterface;

        onReceiveEventHook ??= Hook<Delegates.Agent.ReceiveEvent>.FromAddress(new nint(agent.VTable->ReceiveEvent), DutyFinder_ReceiveEvent);
        onReceiveEventHook?.Enable();
    }

    public void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;

        onDrawHook?.Dispose();
        onFinalizeHook?.Dispose();
        onUpdateHook?.Dispose();
        onRefreshHook?.Dispose();
        onSetupHook?.Dispose();

        onReceiveEventHook?.Dispose();
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        if (ContentsFinderAddon == null) return;

        var addon = ContentsFinderAddon;

        onDrawHook ??= Hook<Delegates.Addon.Draw>.FromAddress(new nint(addon->AtkEventListener.vfunc[41]), OnDraw);
        onFinalizeHook ??= Hook<Delegates.Addon.Finalize>.FromAddress(new nint(addon->AtkEventListener.vfunc[39]), OnFinalize);
        onUpdateHook ??= Hook<Delegates.Addon.Update>.FromAddress(new nint(addon->AtkEventListener.vfunc[40]), OnUpdate);
        onRefreshHook ??= Hook<Delegates.Addon.OnRefresh>.FromAddress(new nint(addon->AtkEventListener.vfunc[48]), OnRefresh);
        onSetupHook ??= Hook<Delegates.Addon.OnSetup>.FromAddress(new nint(addon->AtkEventListener.vfunc[46]), OnSetup);

        onDrawHook?.Enable();
        onFinalizeHook?.Enable();
        onUpdateHook?.Enable();
        onRefreshHook?.Enable();
        onSetupHook?.Enable();

        Service.Framework.Update -= OnFrameworkUpdate;
    }

    private nint DutyFinder_ReceiveEvent(AgentInterface* agent, nint rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender)
    {
        var result = onReceiveEventHook!.Original(agent, rawData, eventArgs, eventArgsCount, sender);

        Safety.ExecuteSafe(() =>
        {
            ReceiveEvent?.Invoke(this, new ReceiveEventArgs(agent, rawData, eventArgs, eventArgsCount, sender));
        });

        return result;
    }

    private nint OnSetup(AtkUnitBase* addon, int valueCount, AtkValue* values)
    {
        var result = onSetupHook!.Original(addon, valueCount, values);

        Safety.ExecuteSafe(() =>
        {
            Show?.Invoke(this, new nint(addon));
        });

        return result;
    }

    private byte OnRefresh(AtkUnitBase* atkUnitBase, int valueCount, AtkValue* values)
    {
        var result = onRefreshHook!.Original(atkUnitBase, valueCount, values);

        Safety.ExecuteSafe(() =>
        {
            Refresh?.Invoke(this, new nint(atkUnitBase));
        });

        return result;
    }

    private byte OnUpdate(AtkUnitBase* atkUnitBase)
    {
        var result = onUpdateHook!.Original(atkUnitBase);

        Safety.ExecuteSafe(() =>
        {
            Update?.Invoke(this, new nint(atkUnitBase));
        });
        
        return result;
    }

    private void OnDraw(AtkUnitBase* atkUnitBase)
    {
        onDrawHook!.Original(atkUnitBase);

        Safety.ExecuteSafe(() =>
        {
            Draw?.Invoke(this, new nint(atkUnitBase));
        });
    }

    private void OnFinalize(AtkUnitBase* atkUnitBase)
    {
        Safety.ExecuteSafe(() =>
        {
            Finalize?.Invoke(this, new nint(atkUnitBase));
        });
        
        onFinalizeHook!.Original(atkUnitBase);
    }

    public static DutyFinderTreeList GetBaseTreeNode()
    {
        var baseNode = new BaseNode("ContentsFinder");
        var treeNode = baseNode.GetComponentNode(52);

        return new DutyFinderTreeList(treeNode);
    }

    public static DutyFinderTabBar GetTabBar()
    {
        var baseNode = new BaseNode("ContentsFinder");

        return new DutyFinderTabBar(baseNode);
    }

    public static void HideCloverNodes()
    {
        if (ContentsFinderAddon != null)
        {
            GetBaseTreeNode().HideCloverNodes();
        }
    }

    public static void ResetLabelColors(ByteColor color)
    {
        if (ContentsFinderAddon != null)
        {
            GetBaseTreeNode().SetColorAll(color);
        }
    }
}