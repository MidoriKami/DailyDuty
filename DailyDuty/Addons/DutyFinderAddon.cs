using System;
using DailyDuty.Addons.DataModels;
using DailyDuty.Utilities;
using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons;

internal unsafe class DutyFinderAddon : IDisposable
{
    public event EventHandler<IntPtr>? Show;
    public event EventHandler<IntPtr>? Draw;
    public event EventHandler<IntPtr>? Finalize;
    public event EventHandler<IntPtr>? Update;
    public event EventHandler<IntPtr>? Refresh;
    public event EventHandler<ReceiveEventArgs>? ReceiveEvent;

    private delegate void AddonDraw(AtkUnitBase* atkUnitBase);
    private delegate byte AddonOnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3);    
    private delegate void AddonFinalize(AtkUnitBase* atkUnitBase);
    private delegate byte AddonUpdate(AtkUnitBase* atkUnitBase);
    private delegate void* AddonOnSetup(AtkUnitBase* addon, uint a2, void* a3);
    private delegate void* AgentReceiveEvent(AgentInterface* agent, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender);

    private Hook<AddonDraw>? onDrawHook;
    private Hook<AddonFinalize>? onFinalizeHook;
    private Hook<AddonUpdate>? onUpdateHook;
    private Hook<AddonOnRefresh>? onRefreshHook;
    private Hook<AddonOnSetup>? onSetupHook;
    private readonly Hook<AgentReceiveEvent>? onReceiveEventHook;

    private AtkUnitBase* ContentsFinderAddon => (AtkUnitBase*) Service.GameGui.GetAddonByName("ContentsFinder", 1);

    public DutyFinderAddon()
    {
        SignatureHelper.Initialise(this);

        Service.Framework.Update += OnFrameworkUpdate;

        var agent = AgentContentsFinder.Instance()->AgentInterface;

        onReceiveEventHook ??= Hook<AgentReceiveEvent>.FromAddress(new IntPtr(agent.VTable->ReceiveEvent), DutyFinder_ReceiveEvent);
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

        onDrawHook ??= Hook<AddonDraw>.FromAddress(new IntPtr(addon->AtkEventListener.vfunc[41]), OnDraw);
        onFinalizeHook ??= Hook<AddonFinalize>.FromAddress(new IntPtr(addon->AtkEventListener.vfunc[39]), OnFinalize);
        onUpdateHook ??= Hook<AddonUpdate>.FromAddress(new IntPtr(addon->AtkEventListener.vfunc[40]), OnUpdate);
        onRefreshHook ??= Hook<AddonOnRefresh>.FromAddress(new IntPtr(addon->AtkEventListener.vfunc[48]), OnRefresh);
        onSetupHook ??= Hook<AddonOnSetup>.FromAddress(new IntPtr(addon->AtkEventListener.vfunc[46]), OnSetup);

        onDrawHook?.Enable();
        onFinalizeHook?.Enable();
        onUpdateHook?.Enable();
        onRefreshHook?.Enable();
        onSetupHook?.Enable();

        Service.Framework.Update -= OnFrameworkUpdate;
    }

    private void* DutyFinder_ReceiveEvent(AgentInterface* agent, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender)
    {
        var result = onReceiveEventHook!.Original(agent, rawData, eventArgs, eventArgsCount, sender);

        try
        {
            ReceiveEvent?.Invoke(this, new ReceiveEventArgs(agent, rawData, eventArgs, eventArgsCount, sender));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something when wrong on Duty Finder Receive Event");
        }

        return result;
    }

    private void* OnSetup(AtkUnitBase* addon, uint a2, void* a3)
    {
        var result = onSetupHook!.Original(addon, a2, a3);

        try
        {
            Show?.Invoke(this, new IntPtr(addon));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something when wrong on Duty Finder Setup");
        }

        return result;
    }

    private byte OnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3)
    {
        var result = onRefreshHook!.Original(atkUnitBase, a2, a3);

        try
        {
            Refresh?.Invoke(this, new IntPtr(atkUnitBase));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something when wrong on Duty Finder Tab Change or Duty Finder Refresh");
        }

        return result;
    }

    private byte OnUpdate(AtkUnitBase* atkUnitBase)
    {
        var result = onUpdateHook!.Original(atkUnitBase);

        try
        {
            Update?.Invoke(this, new IntPtr(atkUnitBase));
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Something when wrong on Duty Finder Update");
        }

        return result;
    }

    private void OnDraw(AtkUnitBase* atkUnitBase)
    {
        onDrawHook!.Original(atkUnitBase);

        try
        {
            Draw?.Invoke(this, new IntPtr(atkUnitBase));
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Something when wrong on Duty Finder Draw");
        }
    }

    private void OnFinalize(AtkUnitBase* atkUnitBase)
    {
        try
        {
            Finalize?.Invoke(this, new IntPtr(atkUnitBase));
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Something when wrong on Duty Finder Close");
        }

        onFinalizeHook!.Original(atkUnitBase);
    }

    public DutyFinderTreeList GetBaseTreeNode(AtkUnitBase* addonPointer)
    {
        return new DutyFinderTreeList(Node.GetComponentNode(addonPointer, 52));
    }

    public DutyFinderTabBar GetTabBar(AtkUnitBase* addonPointer)
    {
        return new DutyFinderTabBar(addonPointer);
    }

    public void HideCloverNodes()
    {
        if (ContentsFinderAddon != null)
        {
            GetBaseTreeNode(ContentsFinderAddon).HideCloverNodes();
        }
    }

    public void ResetLabelColors(ByteColor color)
    {
        if (ContentsFinderAddon != null)
        {
            GetBaseTreeNode(ContentsFinderAddon).SetColorAll(color);
        }
    }
}

public static class IntPointerExtensions
{
    public static unsafe AtkUnitBase* ToAtkUnitBase(this IntPtr pointer)
    {
        return (AtkUnitBase*) pointer.ToPointer();
    }
}