using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DailyDuty.Data.ModuleData.DutyRoulette;
using DailyDuty.Data.ModuleData.WondrousTails;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers.WondrousTails;
using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Utility;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.Excel;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using Lumina.Text;
using Microsoft.Win32.SafeHandles;

namespace DailyDuty.Windows.WondrousTailsDutyFinderOverlay
{
    internal unsafe class WondrousTailsOverlay : IDisposable
    {
        private delegate void AddonOnDraw(AtkUnitBase* atkUnitBase);
        private delegate void* AddonOnFinalize(AtkUnitBase* atkUnitBase);
        private delegate byte AddonOnUpdate(AtkUnitBase* atkUnitBase);
        private delegate byte AddonOnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3);

        [Signature("88 05 ?? ?? ?? ?? 8B 43 18", ScanType = ScanType.StaticAddress)]
        private readonly WondrousTailsStruct* wondrousTails = null;

        private Hook<AddonOnDraw>? onDrawHook = null;
        private Hook<AddonOnFinalize>? onFinalizeHook = null;
        private Hook<AddonOnUpdate>? onUpdateHook = null;
        private Hook<AddonOnRefresh>? onRefreshHook = null;

        private readonly Stopwatch delayStopwatch = new();

        private readonly List<DutyFinderSearchResult> contentFinderDuties = new();
        private List<uint> wondrousTailsDuties;

        public WondrousTailsOverlay()
        {
            SignatureHelper.Initialise(this);

            var contentFinderData = Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                !.Where(cfc => cfc.Name != string.Empty);

            foreach (var cfc in contentFinderData)
            {
                var simplifiedString = Regex.Replace(cfc.Name.ToString().ToLower(), "[^a-z]", "");

                contentFinderDuties.Add(new()
                {
                    TerritoryType = cfc.TerritoryType.Row,
                    SearchKey = simplifiedString
                });
            }

            wondrousTailsDuties = GetAllWondrousTailsDuties();

            Service.Framework.Update += FrameworkOnUpdate;
        }

        private void FrameworkOnUpdate(Framework framework)
        {
            if (IsContentFinderOpen() == true)
            {
                var addonContentsFinder = GetContentsFinderPointer();

                var drawAddress = addonContentsFinder->AtkEventListener.vfunc[40];
                var finalizeAddress = addonContentsFinder->AtkEventListener.vfunc[38];
                var updateAddress = addonContentsFinder->AtkEventListener.vfunc[39];
                var onRefreshAddress = addonContentsFinder->AtkEventListener.vfunc[47];

                onDrawHook ??= new Hook<AddonOnDraw>(new IntPtr(drawAddress), OnDraw);
                onFinalizeHook ??= new Hook<AddonOnFinalize>(new IntPtr(finalizeAddress), OnFinalize);
                onUpdateHook ??= new Hook<AddonOnUpdate>(new IntPtr(updateAddress), OnUpdate);
                onRefreshHook ??= new Hook<AddonOnRefresh>(new IntPtr(onRefreshAddress), OnRefresh);

                onDrawHook.Enable();
                onFinalizeHook.Enable();
                onUpdateHook.Enable();
                onRefreshHook.Enable();
                
                Service.Framework.Update -= FrameworkOnUpdate;
            }
        }

        private byte OnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3)
        {
            var result = onRefreshHook!.Original(atkUnitBase, a2, a3);

            wondrousTailsDuties = GetAllWondrousTailsDuties();

            return result;
        }
        
        private byte OnUpdate(AtkUnitBase* atkUnitBase)
        {
            var result = onUpdateHook!.Original(atkUnitBase);

            Time.UpdateDelayed(delayStopwatch, TimeSpan.FromMilliseconds(30), () =>
            {
                foreach (var i in Enumerable.Range(61001, 15).Append(6))
                {
                    var id = (uint)i;

                    var visible = IsWondrousTailsDuty(id);

                    SetImageNodeVisibility(id, visible);
                }
            });

            return result;
        }

        private void OnDraw(AtkUnitBase* atkUnitBase)
        {
            onDrawHook!.Original(atkUnitBase);

            if (atkUnitBase == null) return;

            //AddImageNodeByID(6);

            foreach (var i in Enumerable.Range(61001, 15).Append(6))
            {
                var id = (uint)i;

                AddImageNodeByID(id);
            }
        }

        private bool IsWondrousTailsDuty(uint id)
        {
            var listItemNode = GetListItemNode(id);
            var textNode = GetTextNode(listItemNode);

            var nodeString = textNode->NodeText.ToString().ToLower();
            var nodeRegexString = Regex.Replace(nodeString, "[^a-z]", "");

            foreach (var result in contentFinderDuties)
            {
                // If we found the entry
                if (result.SearchKey == nodeRegexString)
                {
                    return wondrousTailsDuties.Contains(result.TerritoryType);
                }
            }

            return false;
        }

        private void AddImageNodeByID(uint id)
        {
            var treeNode = GetTreeListBaseNode();
            var targetNode = GetListItemNode(id);

            if (treeNode == null || targetNode == null)
            {
                Chat.Debug("Null, sadge");
                return;
            }

            var uldManager = targetNode->Component->UldManager;
            var customNode = GetNodeByID<AtkImageNode>(uldManager, 29, NodeType.Image);

            if (customNode == null)
            {
                MakeCustomNode(targetNode);
            }
        }

        private void* OnFinalize(AtkUnitBase* atkUnitBase)
        {
            if (atkUnitBase == null) return null;

            foreach (var i in Enumerable.Range(61001, 15).Append(6))
            {
                var id = (uint) i;
                DestroyNode(id);
            }

            return onFinalizeHook!.Original(atkUnitBase);
        }

        private void DestroyNode(uint id)
        {
            var firstNode = GetListItemNode(id);

            if (firstNode == null) return;

            var uldManager = firstNode->Component->UldManager;
            var customNode = GetNodeByID<AtkImageNode>(uldManager, 29, NodeType.Image);

            if (customNode != null)
            {
                if (customNode->AtkResNode.PrevSiblingNode != null)
                    customNode->AtkResNode.PrevSiblingNode->NextSiblingNode = customNode->AtkResNode.NextSiblingNode;

                if (customNode->AtkResNode.NextSiblingNode != null)
                    customNode->AtkResNode.NextSiblingNode->PrevSiblingNode = customNode->AtkResNode.PrevSiblingNode;

                firstNode->Component->UldManager.UpdateDrawNodeList();

                IMemorySpace.Free(customNode->PartsList->Parts->UldAsset, (ulong) sizeof(AtkUldAsset));
                IMemorySpace.Free(customNode->PartsList->Parts, (ulong) sizeof(AtkUldPart));
                IMemorySpace.Free(customNode->PartsList, (ulong) sizeof(AtkUldPartsList));

                //customNode->UnloadTexture();

                customNode->AtkResNode.Destroy(true);
            }
        }

        public void Dispose()
        {
            Service.Framework.Update -= FrameworkOnUpdate;

            onDrawHook?.Dispose();
            onFinalizeHook?.Dispose();
            onUpdateHook?.Dispose();
            onRefreshHook?.Dispose();
        }


        //
        //  Implementation
        //
        private T* GetNodeByID<T>(AtkUldManager uldManager, uint nodeId, NodeType? type = null) where T : unmanaged 
        {
            for (var i = 0; i < uldManager.NodeListCount; i++) 
            {
                var n = uldManager.NodeList[i];
                if (n->NodeID != nodeId || type != null && n->Type != type.Value) continue;
                return (T*)n;
            }
            return null;
        }

        private bool IsContentFinderOpen()
        {
            return GetContentsFinderPointer() != null;
        }

        private AtkUnitBase* GetContentsFinderPointer()
        {
            return (AtkUnitBase*)Service.GameGui.GetAddonByName("ContentsFinder", 1);
        }

        private AtkComponentNode* GetTreeListBaseNode()
        {
            var pointer = GetContentsFinderPointer();

            if (pointer != null)
            {
                return (AtkComponentNode*)pointer->GetNodeById(52);
            }

            return null;
        }

        private AtkComponentNode* GetListItemNode(uint nodeID)
        {
            var pointer = GetTreeListBaseNode();

            if (pointer != null)
            {
                var uldManager = pointer->Component->UldManager;

                return GetNodeByID<AtkComponentNode>(uldManager, nodeID);
            }

            return null;
        }

        private AtkTextNode* GetTextNode(AtkComponentNode* listItemNode)
        {
            if (listItemNode != null)
            {
                var ulManager = listItemNode->Component->UldManager;

                return GetNodeByID<AtkTextNode>(ulManager, 5);
            }

            return null;
        }

        private AtkResNode* GetAdjacentResNode(AtkComponentNode* listItemNode)
        {
            if (listItemNode != null)
            {
                var ulManager = listItemNode->Component->UldManager;

                return GetNodeByID<AtkResNode>(ulManager, 6);
            }

            return null;
        }

        private void MakeCustomNode(AtkComponentNode* rootNode)
        {
            var customNode = IMemorySpace.GetUISpace()->Create<AtkImageNode>();
            customNode->AtkResNode.Type = NodeType.Image;
            customNode->AtkResNode.NodeID = 29;
            customNode->AtkResNode.Flags = 35;
            customNode->AtkResNode.DrawFlags = 0;
            customNode->WrapMode = 2;
            customNode->Flags = 0;


            var partsList = (AtkUldPartsList*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldPartsList), 8);
            if (partsList == null) {
                customNode->AtkResNode.Destroy(true);
                return;
            }

            partsList->Id = 0;
            partsList->PartCount = 1;

            var part = (AtkUldPart*) IMemorySpace.GetUISpace()->Malloc((ulong) sizeof(AtkUldPart), 8);
            if (part == null) {
                IMemorySpace.Free(partsList, (ulong)sizeof(AtkUldPartsList));
                customNode->AtkResNode.Destroy(true);
                return;
            }

            part->U = 97;
            part->V = 65;
            part->Width = 20;
            part->Height = 20;
            
            partsList->Parts = part;

            var asset = (AtkUldAsset*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldAsset), 8);
            if (asset == null) {
                IMemorySpace.Free(part, (ulong)sizeof(AtkUldPart));
                IMemorySpace.Free(partsList, (ulong)sizeof(AtkUldPartsList));
                customNode->AtkResNode.Destroy(true);
                return;
            }

            asset->Id = 0;
            asset->AtkTexture.Ctor();
            part->UldAsset = asset;
            customNode->PartsList = partsList;

            //customNode->LoadTexture("ui/uld/WeeklyBingo_hr1.tex");
            customNode->LoadTexture("ui/uld/WeeklyBingo.tex");

            customNode->AtkResNode.ToggleVisibility(true);

            customNode->AtkResNode.SetWidth(20);
            customNode->AtkResNode.SetHeight(20);
            customNode->AtkResNode.SetPositionShort(290, 2);

            var adjacentResNode = GetAdjacentResNode(rootNode);

            var prev = adjacentResNode->PrevSiblingNode;
            customNode->AtkResNode.ParentNode = adjacentResNode->ParentNode;

            adjacentResNode->PrevSiblingNode = (AtkResNode*) customNode;
            prev->NextSiblingNode = (AtkResNode*) customNode;

            customNode->AtkResNode.PrevSiblingNode = prev;
            customNode->AtkResNode.NextSiblingNode = adjacentResNode;

            rootNode->Component->UldManager.UpdateDrawNodeList();
        }

        private List<uint> GetAllWondrousTailsDuties()
        {
            var allTasks = GetAllTaskData();

            List<uint> result = new();

            foreach (var (_, tasks) in allTasks)
            {
                result.AddRange(tasks);
            }

            return result.Distinct().ToList();
        }

        private IEnumerable<(ButtonState, List<uint>)> GetAllTaskData()
        {
            var result = new (ButtonState, List<uint>)[16];

            for (int i = 0; i < 16; ++i)
            {
                var taskButtonState = wondrousTails->TaskStatus(i);
                var instances = TaskLookup.GetInstanceListFromID(wondrousTails->Tasks[i]);

                result[i] = (taskButtonState, instances);
            }

            return result;
        }

        private void SetImageNodeVisibility(uint id, bool visible)
        {
            var firstNode = GetListItemNode(id);

            if (firstNode == null) return;

            var uldManager = firstNode->Component->UldManager;
            var customNode = GetNodeByID<AtkImageNode>(uldManager, 29, NodeType.Image);

            customNode->AtkResNode.ToggleVisibility(visible);
        }

    }
}
