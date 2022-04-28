using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Features
{
    internal unsafe class DutyRouletteDutyFinderOverlayAddonModule : IDisposable
    {
        private DutyRouletteDutyFinderOverlaySettings Settings => Service.SystemConfiguration.Addons.DutyRouletteOverlaySettings;
        private DutyRouletteSettings DutyRouletteSettings => Service.CharacterConfiguration.DutyRoulette;

        private delegate void* AgentHide(void* a1);
        private delegate void* AgentShow(void* a1);
        private delegate void AddonDraw(AtkUnitBase* atkUnitBase);
        private delegate byte AddonOnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3);    
        

        [Signature("40 53 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 84 C0 74 30 48 8B 4B 10", DetourName = nameof(ContentsFinder_Show))]
        private readonly Hook<AgentShow>? contentsFinderShowHook = null;

        [Signature("48 89 5C 24 ?? 57 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 48 8B CB E8 ?? ?? ?? ?? 48 8B CB", DetourName = nameof(ContentsFinder_Hide))]
        private readonly Hook<AgentHide>? contentsFinderHideHook = null;

        private Hook<AddonDraw>? onDrawHook;
        private Hook<AddonOnRefresh>? onRefreshHook;

        private readonly List<DutyFinderSearchResult> dutyRouletteDuties = new();
        private IEnumerable<TrackedRoulette> DutyRoulettes => DutyRouletteSettings.TrackedRoulettes;

        private bool defaultColorSaved;
        private ByteColor userDefaultTextColor;
        
        public DutyRouletteDutyFinderOverlayAddonModule()
        {
            SignatureHelper.Initialise(this);

            contentsFinderShowHook?.Enable();

            var rouletteData = Service.DataManager.GetExcelSheet<ContentRoulette>()
                !.Where(cr => cr.Name != string.Empty);

            foreach (var cr in rouletteData)
            {
                var simplifiedString = Regex.Replace(cr.Category.ToString().ToLower(), "[^\\p{L}\\p{N}]", "");

                dutyRouletteDuties.Add(new()
                {
                    SearchKey = simplifiedString,
                    Value = cr.RowId
                });
            }
        }

        public void Dispose()
        {
            contentsFinderShowHook?.Dispose();
            contentsFinderHideHook?.Dispose();

            onDrawHook?.Dispose();
            onRefreshHook?.Dispose();

            var frameworkInstance = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
            var contentsFinderAgent = frameworkInstance->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.ContentsFinder);
            if (contentsFinderAgent->IsAgentActive())
            {
                ResetDefaultTextColor();
            }
        }

        private void* ContentsFinder_Show(void* a1)
        {
            var result = contentsFinderShowHook!.Original(a1);

            if (Settings.Enabled)
            {
                var frameworkInstance = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
                var contentsFinderAgent = frameworkInstance->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.ContentsFinder);

                if (contentsFinderAgent->IsAgentActive())
                {
                    var addonContentsFinder = GetContentsFinderPointer();

                    if (addonContentsFinder == null)
                    {
                        Chat.Debug("Addon null");
                        return result;
                    }

                    var drawAddress = addonContentsFinder->AtkEventListener.vfunc[41];
                    var onRefreshAddress = addonContentsFinder->AtkEventListener.vfunc[48];
                    var finalizeAddress = addonContentsFinder->AtkEventListener.vfunc[39];

                    onDrawHook ??= new Hook<AddonDraw>(new IntPtr(drawAddress), ContentsFinder_Draw);
                    onRefreshHook ??= new Hook<AddonOnRefresh>(new IntPtr(onRefreshAddress), ContentsFinder_OnRefresh);

                    onDrawHook.Enable();
                    onRefreshHook.Enable();

                    contentsFinderHideHook?.Enable();
                    contentsFinderShowHook!.Disable();
                }
            }

            return result;
        }

        private byte ContentsFinder_OnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3)
        {
            var result = onRefreshHook!.Original(atkUnitBase, a2, a3);

            if (Settings.Enabled && DutyRouletteSettings.Enabled)
            {
                if (IsTabSelected(DutyFinderTab.Roulette) == false)
                {
                    ResetDefaultTextColor();
                }
                else
                {
                    SetRouletteColors();
                }
            }
            else
            {
                ResetDefaultTextColor();
            }
            
            return result;
        }

        private void ContentsFinder_Draw(AtkUnitBase* atkUnitBase)
        {
            onDrawHook!.Original(atkUnitBase);
            if (atkUnitBase == null) return;

            if (defaultColorSaved == false)
            {
                var textNode = GetListItemTextNode(6);

                if (textNode == null) return;

                userDefaultTextColor = textNode->TextColor;
                defaultColorSaved = true;
            }

            if (Settings.Enabled && DutyRouletteSettings.Enabled)
            {
                if (IsTabSelected(DutyFinderTab.Roulette) == false)
                {
                    ResetDefaultTextColor();
                }
                else
                {
                    SetRouletteColors();
                }
            }
            else
            {
                ResetDefaultTextColor();
            }
        }

        private void* ContentsFinder_Hide(void* a1)
        {
            ResetDefaultTextColor();

            return contentsFinderHideHook!.Original(a1);
        }

        //
        //  Implementation
        //

        private void ResetDefaultTextColor()
        {
            foreach (var i in Enumerable.Range(61001, 15).Append(6))
            {
                var id = (uint)i;

                var textNode = GetListItemTextNode(id);

                textNode->TextColor = userDefaultTextColor;
            }
        }

        private void SetRouletteColors()
        {
            foreach (var i in Enumerable.Range(61001, 15).Append(6))
            {
                var id = (uint) i;

                var rouletteState = IsRouletteDuty(id);

                if (rouletteState != null)
                {
                    var textNode = GetListItemTextNode(id);

                    if (rouletteState is {Tracked: true, Completed: true})
                    {
                        textNode->TextColor.R = (byte) (Settings.DutyRouletteCompleteColor.X * 255);
                        textNode->TextColor.G = (byte) (Settings.DutyRouletteCompleteColor.Y * 255);
                        textNode->TextColor.B = (byte) (Settings.DutyRouletteCompleteColor.Z * 255);
                        textNode->TextColor.A = (byte) (Settings.DutyRouletteCompleteColor.W * 255);
                    }
                    else if (rouletteState is {Tracked: true, Completed: false})
                    {
                        textNode->TextColor.R = (byte) (Settings.DutyRouletteIncompleteColor.X * 255);
                        textNode->TextColor.G = (byte) (Settings.DutyRouletteIncompleteColor.Y * 255);
                        textNode->TextColor.B = (byte) (Settings.DutyRouletteIncompleteColor.Z * 255);
                        textNode->TextColor.A = (byte) (Settings.DutyRouletteIncompleteColor.W * 255);
                    }
                    else
                    {
                        textNode->TextColor = userDefaultTextColor;
                    }
                }
            }
        }

        private TrackedRoulette? IsRouletteDuty(uint id)
        {
            var textNode = GetListItemTextNode(id);

            var nodeString = textNode->NodeText.ToString().ToLower();
            var nodeRegexString = Regex.Replace(nodeString, "[^\\p{L}\\p{N}]", "");

            foreach (var result in dutyRouletteDuties)
            {
                if (result.SearchKey == nodeRegexString)
                {
                    var trackedRoulette = DutyRoulettes
                        .Where(duty => (uint) duty.Type == result.Value)
                        .FirstOrDefault();

                    return trackedRoulette;
                }
            }

            return null;
        }
        
        private AtkUnitBase* GetContentsFinderPointer()
        {
            return (AtkUnitBase*)Service.GameGui.GetAddonByName("ContentsFinder", 1);
        }
        private AtkComponentNode* GetTreeListBaseNode()
        {
            return Node.GetComponentNode(GetContentsFinderPointer(), 52);
        }

        private AtkComponentNode* GetListItemNode(uint nodeID)
        {
            return Node.GetNodeByID<AtkComponentNode>(GetTreeListBaseNode(), nodeID);
        }
        private AtkTextNode* GetTextNode(AtkComponentNode* listItemNode)
        {
            return Node.GetNodeByID<AtkTextNode>(listItemNode, 5);
        }

        private AtkTextNode* GetListItemTextNode(uint id)
        {
            var listItemNode = GetListItemNode(id);

            if(listItemNode == null) return null;

            var textNode = GetTextNode(listItemNode);

            return textNode;
        }
        
        private bool IsTabSelected(DutyFinderTab tab)
        {
            var tabNodeId = 41 + (uint) tab;

            var baseNode = GetContentsFinderPointer();
            if(baseNode == null) return false;

            var specificTab = baseNode->GetNodeById(tabNodeId);
            if(specificTab == null) return false;

            var tabComponentNode = specificTab->GetAsAtkComponentNode();
            if(tabComponentNode == null) return false;

            var targetResNode = Node.GetNodeByID<AtkResNode>(tabComponentNode, 5);

            return targetResNode->AddRed > 16;
        }
    }
}
