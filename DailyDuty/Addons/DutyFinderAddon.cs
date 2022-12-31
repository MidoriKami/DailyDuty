using System;
using DailyDuty.DataModels;
using DailyDuty.System;
using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.ExceptionSafety;
using KamiLib.Utilities;

namespace DailyDuty.Addons;

public unsafe class DutyFinderAddon : IDisposable
{
    private static DutyFinderAddon? _instance;
    public static DutyFinderAddon Instance => _instance ??= new DutyFinderAddon();
    
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

    private static AtkUnitBase* ContentsFinderAddon => (AtkUnitBase*) Service.GameGui.GetAddonByName("ContentsFinder", 1);

    private DutyFinderAddon()
    {
        AddonManager.AddAddon(this);
        
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

        Safety.ExecuteSafe(() =>
        {
            ReceiveEvent?.Invoke(this, new ReceiveEventArgs(agent, rawData, eventArgs, eventArgsCount, sender));
        });

        return result;
    }

    private void* OnSetup(AtkUnitBase* addon, uint a2, void* a3)
    {
        var result = onSetupHook!.Original(addon, a2, a3);

        Safety.ExecuteSafe(() =>
        {
            Show?.Invoke(this, new IntPtr(addon));
        });

        return result;
    }

    private byte OnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3)
    {
        var result = onRefreshHook!.Original(atkUnitBase, a2, a3);

        Safety.ExecuteSafe(() =>
        {
            Refresh?.Invoke(this, new IntPtr(atkUnitBase));
        });

        return result;
    }

    private byte OnUpdate(AtkUnitBase* atkUnitBase)
    {
        var result = onUpdateHook!.Original(atkUnitBase);

        Safety.ExecuteSafe(() =>
        {
            Update?.Invoke(this, new IntPtr(atkUnitBase));
        });
        
        return result;
    }

    private void OnDraw(AtkUnitBase* atkUnitBase)
    {
        onDrawHook!.Original(atkUnitBase);

        Safety.ExecuteSafe(() =>
        {
            Draw?.Invoke(this, new IntPtr(atkUnitBase));
        });
    }

    private void OnFinalize(AtkUnitBase* atkUnitBase)
    {
        Safety.ExecuteSafe(() =>
        {
            Finalize?.Invoke(this, new IntPtr(atkUnitBase));
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