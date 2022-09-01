using System;
using DailyDuty.Addons.DataModels;
using DailyDuty.Addons.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons;

internal unsafe class DutyFinderAddon : IAddon
{
    public AddonName Name => AddonName.DutyFinder;

    public event EventHandler<IntPtr>? OnDraw;
    public event EventHandler<IntPtr>? OnFinalize;
    public event EventHandler<IntPtr>? OnUpdate;
    public event EventHandler<IntPtr>? OnRefresh;

    private delegate void* AgentShow(void* a1);
    private delegate void AddonDraw(AtkUnitBase* atkUnitBase);
    private delegate byte AddonOnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3);    
    private delegate void* AddonFinalize(AtkUnitBase* atkUnitBase);
    private delegate byte AddonUpdate(AtkUnitBase* atkUnitBase);

    [Signature("40 53 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 84 C0 74 30 48 8B 4B 10", DetourName = nameof(DutyFinder_Show))]
    private readonly Hook<AgentShow>? contentsFinderShowHook = null;

    private Hook<AddonDraw>? onDrawHook;
    private Hook<AddonFinalize>? onFinalizeHook;
    private Hook<AddonUpdate>? onUpdateHook;
    private Hook<AddonOnRefresh>? onRefreshHook;

    public DutyFinderAddon()
    {
        SignatureHelper.Initialise(this);

        contentsFinderShowHook?.Enable();
    }

    public void Dispose()
    {
        contentsFinderShowHook?.Dispose();

        onDrawHook?.Dispose();
        onFinalizeHook?.Dispose();
        onUpdateHook?.Dispose();
        onRefreshHook?.Dispose();
    }

    private void* DutyFinder_Show(void* a1)
    {
        var result = contentsFinderShowHook!.Original(a1);

        try
        {
            var contentsFinderAgent = Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.ContentsFinder);

            if (contentsFinderAgent->IsAgentActive())
            {
                var addonContentsFinder = GetContentsFinderPointer();

                if (addonContentsFinder == null)
                {
                    return result;
                }

                var drawAddress = addonContentsFinder->AtkEventListener.vfunc[41];
                var finalizeAddress = addonContentsFinder->AtkEventListener.vfunc[39];
                var updateAddress = addonContentsFinder->AtkEventListener.vfunc[40];
                var onRefreshAddress = addonContentsFinder->AtkEventListener.vfunc[48];

                onDrawHook ??= Hook<AddonDraw>.FromAddress(new IntPtr(drawAddress), DutyFinder_Draw);
                onFinalizeHook ??= Hook<AddonFinalize>.FromAddress(new IntPtr(finalizeAddress), DutyFinder_Finalize);
                onUpdateHook ??= Hook<AddonUpdate>.FromAddress(new IntPtr(updateAddress), DutyFinder_Update);
                onRefreshHook ??= Hook<AddonOnRefresh>.FromAddress(new IntPtr(onRefreshAddress), DutyFinder_OnRefresh);

                onDrawHook.Enable();
                onFinalizeHook.Enable();
                onUpdateHook.Enable();
                onRefreshHook.Enable();

                contentsFinderShowHook!.Disable();
            }
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something went wrong when the Duty Finder was opened");
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

            var treeNode = GetBaseTreeNode(atkUnitBase);

            foreach (var node in treeNode.Items)
            {
                PluginLog.Verbose(node.Label + " " + node.FilteredLabel);
            }

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

    public void HideCloverNodes()
    {
        var addonPointer = GetContentsFinderPointer();

        if (addonPointer != null)
        {
            GetBaseTreeNode(addonPointer).HideCloverNodes();
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