using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DailyDuty.Addons.ContentsFinder;
using DailyDuty.System;
using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;
using KamiLib.Caching;
using KamiLib.Hooking;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Addons;

public unsafe partial class AddonContentsFinder : IDisposable
{
    private static AddonContentsFinder? _instance;
    public static AddonContentsFinder Instance => _instance ??= new AddonContentsFinder();
    
    public event EventHandler<nint>? Draw;
    public event EventHandler<nint>? Finalize;
    public event EventHandler<nint>? Update;
    public event EventHandler<nint>? Refresh;

    private Hook<Delegates.Addon.Draw>? onDrawHook;
    private Hook<Delegates.Addon.Finalize>? onFinalizeHook;
    private Hook<Delegates.Addon.Update>? onUpdateHook;
    private Hook<Delegates.Addon.OnRefresh>? onRefreshHook;

    private static AtkUnitBase* ContentsFinderAtkUnitBase => (AtkUnitBase*) Service.GameGui.GetAddonByName("ContentsFinder");

    [GeneratedRegex("[^\\p{L}\\p{N}]")]
    public static partial Regex Alphanumeric();
    
    public record DutyFinderSearchResult(string SearchKey, uint TerritoryType);
    public readonly List<DutyFinderSearchResult> Roulettes = new();
    public readonly List<DutyFinderSearchResult> Duties = new();
    
    private AddonContentsFinder()
    {
        AddonManager.AddAddon(this);
        
        // Get a normalized list of Roulette Names
        var rouletteData = LuminaCache<ContentRoulette>.Instance
            .Where(cr => cr.Name != string.Empty);

        foreach (var cr in rouletteData)
        {
            var simplifiedString = Alphanumeric().Replace(cr.Category.ToString().ToLower(), "");
            Roulettes.Add(new DutyFinderSearchResult(simplifiedString, cr.RowId));
        }
        
        // Get a normalized list of Duty Names
        var contentFinderData = LuminaCache<ContentFinderCondition>.Instance
            .Where(cfc => cfc.Name != string.Empty);
        
        foreach (var cfc in contentFinderData)
        {
            var simplifiedString = Alphanumeric().Replace(cfc.Name.ToString().ToLower(), "");
            Duties.Add(new DutyFinderSearchResult(simplifiedString, cfc.TerritoryType.Row));
        }
        
        Service.Framework.Update += OnFrameworkUpdate;
    }

    public void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;

        onFinalizeHook?.Dispose();
        onUpdateHook?.Dispose();
        onDrawHook?.Dispose();
        onRefreshHook?.Dispose();
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        if (ContentsFinderAtkUnitBase == null) return;

        var addon = ContentsFinderAtkUnitBase;

        onFinalizeHook ??= Hook<Delegates.Addon.Finalize>.FromAddress(new nint(addon->AtkEventListener.vfunc[40]), OnFinalize);
        onUpdateHook ??= Hook<Delegates.Addon.Update>.FromAddress(new nint(addon->AtkEventListener.vfunc[41]), OnUpdate);
        onDrawHook ??= Hook<Delegates.Addon.Draw>.FromAddress(new nint(addon->AtkEventListener.vfunc[42]), OnDraw);
        onRefreshHook ??= Hook<Delegates.Addon.OnRefresh>.FromAddress(new nint(addon->AtkEventListener.vfunc[49]), OnRefresh);

        onFinalizeHook?.Enable();
        onUpdateHook?.Enable();
        onDrawHook?.Enable();
        onRefreshHook?.Enable();

        Service.Framework.Update -= OnFrameworkUpdate;
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
        if (ContentsFinderAtkUnitBase != null)
        {
            GetBaseTreeNode().HideCloverNodes();
        }
    }

    public static void ResetLabelColors(ByteColor color)
    {
        if (ContentsFinderAtkUnitBase != null)
        {
            GetBaseTreeNode().SetColorAll(color);
        }
    }
}