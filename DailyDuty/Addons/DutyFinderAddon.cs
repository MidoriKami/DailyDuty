using System;
using DailyDuty.Addons.DataModels;
using DailyDuty.Utilities;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons;

internal unsafe class DutyFinderAddon : IDisposable
{
    public event EventHandler<IntPtr>? OnDraw;
    public event EventHandler<IntPtr>? OnFinalize;
    public event EventHandler<IntPtr>? OnUpdate;
    public event EventHandler<IntPtr>? OnRefresh;
    public event EventHandler<ReceiveEventArgs>? OnReceiveEvent;

    private delegate void* AddonShow(AgentInterface* a1);
    private delegate void AddonDraw(AtkUnitBase* atkUnitBase);
    private delegate byte AddonOnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3);    
    private delegate void* AddonFinalize(AtkUnitBase* atkUnitBase);
    private delegate byte AddonUpdate(AtkUnitBase* atkUnitBase);
    private delegate void* AgentReceiveEvent(AgentInterface* agent, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender);
    
    [Signature("40 53 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 84 C0 74 30 48 8B 4B 10", DetourName = nameof(DutyFinder_Show))]
    private readonly Hook<AddonShow>? contentsFinderShowHook = null;

    private Hook<AddonDraw>? onDrawHook;
    private Hook<AddonFinalize>? onFinalizeHook;
    private Hook<AddonUpdate>? onUpdateHook;
    private Hook<AddonOnRefresh>? onRefreshHook;
    private readonly Hook<AgentReceiveEvent>? onReceiveEventHook;

    public DutyFinderAddon()
    {
        SignatureHelper.Initialise(this);

        contentsFinderShowHook?.Enable();

        var agent = AgentContentsFinder.Instance()->AgentInterface;

        onReceiveEventHook ??= Hook<AgentReceiveEvent>.FromAddress(new IntPtr(agent.VTable->ReceiveEvent), DutyFinder_ReceiveEvent);
        onReceiveEventHook?.Enable();
    }

    public void Dispose()
    {
        contentsFinderShowHook?.Dispose();

        onDrawHook?.Dispose();
        onFinalizeHook?.Dispose();
        onUpdateHook?.Dispose();
        onRefreshHook?.Dispose();
        onReceiveEventHook?.Dispose();
    }

    private void* DutyFinder_Show(AgentInterface* agent)
    {
        var result = contentsFinderShowHook!.Original(agent);

        try
        {
            var addon = GetContentsFinderPointer();

            onDrawHook ??= Hook<AddonDraw>.FromAddress(new IntPtr(addon->AtkEventListener.vfunc[41]), DutyFinder_Draw);
            onFinalizeHook ??= Hook<AddonFinalize>.FromAddress(new IntPtr(addon->AtkEventListener.vfunc[39]), DutyFinder_Finalize);
            onUpdateHook ??= Hook<AddonUpdate>.FromAddress(new IntPtr(addon->AtkEventListener.vfunc[40]), DutyFinder_Update);
            onRefreshHook ??= Hook<AddonOnRefresh>.FromAddress(new IntPtr(addon->AtkEventListener.vfunc[48]), DutyFinder_OnRefresh);

            onDrawHook?.Enable();
            onFinalizeHook?.Enable();
            onUpdateHook?.Enable();
            onRefreshHook?.Enable();

            contentsFinderShowHook!.Disable();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something went wrong when the Duty Finder was opened");
        }

        return result;
    }

    private void* DutyFinder_ReceiveEvent(AgentInterface* agent, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender)
    {
        var result = onReceiveEventHook!.Original(agent, rawData, eventArgs, eventArgsCount, sender);

        try
        {
            OnReceiveEvent?.Invoke(this, new ReceiveEventArgs(agent, rawData, eventArgs, eventArgsCount, sender));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something when wrong on Duty Finder Receive Event");
        }

        return result;
    }


    private byte DutyFinder_OnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3)
    {
        var result = onRefreshHook!.Original(atkUnitBase, a2, a3);

        try
        {
            OnRefresh?.Invoke(this, new IntPtr(atkUnitBase));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something when wrong on Duty Finder Tab Change or Duty Finder Refresh");
        }

        return result;
    }

    private byte DutyFinder_Update(AtkUnitBase* atkUnitBase)
    {
        var result = onUpdateHook!.Original(atkUnitBase);

        try
        {
            OnUpdate?.Invoke(this, new IntPtr(atkUnitBase));
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Something when wrong on Duty Finder Update");
        }

        return result;
    }

    private void DutyFinder_Draw(AtkUnitBase* atkUnitBase)
    {
        onDrawHook!.Original(atkUnitBase);

        try
        {
            OnDraw?.Invoke(this, new IntPtr(atkUnitBase));
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Something when wrong on Duty Finder Draw");
        }
    }

    private void* DutyFinder_Finalize(AtkUnitBase* atkUnitBase)
    {
        try
        {
            OnFinalize?.Invoke(this, new IntPtr(atkUnitBase));
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Something when wrong on Duty Finder Close");
        }

        return onFinalizeHook!.Original(atkUnitBase);
    }

    private AtkUnitBase* GetContentsFinderPointer()
    {
        return (AtkUnitBase*)Service.GameGui.GetAddonByName("ContentsFinder", 1);
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
        var addonPointer = GetContentsFinderPointer();

        if (addonPointer != null)
        {
            GetBaseTreeNode(addonPointer).HideCloverNodes();
        }
    }

    public void ResetLabelColors(ByteColor color)
    {
        var addonPointer = GetContentsFinderPointer();

        if (addonPointer != null)
        {
            GetBaseTreeNode(addonPointer).SetColorAll(color);
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