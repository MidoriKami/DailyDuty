using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Modules;
using DailyDuty.Structs;
using DailyDuty.Utilities;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Features
{
    internal unsafe class WondrousTailsDutyFinderOverlayAddonModule : IDisposable
    {
        private WondrousTailsDutyFinderOverlaySettings Settings => Service.SystemConfiguration.Addons.WondrousTailsOverlaySettings;

        private delegate void* AgentShow(void* a1);
        private delegate void AddonDraw(AtkUnitBase* atkUnitBase);
        private delegate byte AddonOnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3);    
        private delegate void* AddonFinalize(AtkUnitBase* atkUnitBase);
        private delegate byte AddonUpdate(AtkUnitBase* atkUnitBase);

        [Signature("88 05 ?? ?? ?? ?? 8B 43 18", ScanType = ScanType.StaticAddress)]
        private readonly WondrousTailsStruct* wondrousTails = null;

        [Signature("40 53 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 84 C0 74 30 48 8B 4B 10", DetourName = nameof(ContentsFinder_Show))]
        private readonly Hook<AgentShow>? contentsFinderShowHook = null;

        private Hook<AddonDraw>? onDrawHook;
        private Hook<AddonFinalize>? onFinalizeHook;
        private Hook<AddonUpdate>? onUpdateHook;
        private Hook<AddonOnRefresh>? onRefreshHook;

        private readonly List<DutyFinderSearchResult> contentFinderDuties = new();

        private List<WondrousTailsTask> wondrousTailsStatus;

        public WondrousTailsDutyFinderOverlayAddonModule()
        {
            SignatureHelper.Initialise(this);

            contentsFinderShowHook?.Enable();

            var contentFinderData = Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                !.Where(cfc => cfc.Name != string.Empty);

            foreach (var cfc in contentFinderData)
            {
                var simplifiedString = Regex.Replace(cfc.Name.ToString().ToLower(), "[^\\p{L}\\p{N}]", "");

                contentFinderDuties.Add(new DutyFinderSearchResult
                {
                    Value = cfc.TerritoryType.Row,
                    SearchKey = simplifiedString
                });
            }

            wondrousTailsStatus = WondrousTailsModule.GetAllTaskData(wondrousTails);
        }

        public void Dispose()
        {
            contentsFinderShowHook?.Dispose();

            onDrawHook?.Dispose();
            onFinalizeHook?.Dispose();
            onUpdateHook?.Dispose();
            onRefreshHook?.Dispose();

            var frameworkInstance = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
            var contentsFinderAgent = frameworkInstance->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.ContentsFinder);
            if (contentsFinderAgent->IsAgentActive())
            {
                var addonPointer = GetContentsFinderPointer();

                foreach (var i in Enumerable.Range(61001, 15).Append(6))
                {
                    var id = (uint)i;

                    SetImageNodeVisibility(addonPointer, id, 29, false);
                    SetImageNodeVisibility(addonPointer, id, 30, false);
                }
            }
        }

        private void* ContentsFinder_Show(void* a1)
        {
            var result = contentsFinderShowHook!.Original(a1);

            try
            {
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
                        var finalizeAddress = addonContentsFinder->AtkEventListener.vfunc[39];
                        var updateAddress = addonContentsFinder->AtkEventListener.vfunc[40];
                        var onRefreshAddress = addonContentsFinder->AtkEventListener.vfunc[48];

                        onDrawHook ??= new Hook<AddonDraw>(new IntPtr(drawAddress), ContentsFinder_Draw);
                        onFinalizeHook ??= new Hook<AddonFinalize>(new IntPtr(finalizeAddress), ContentsFinder_Finalize);
                        onUpdateHook ??= new Hook<AddonUpdate>(new IntPtr(updateAddress), ContentsFinder_Update);
                        onRefreshHook ??= new Hook<AddonOnRefresh>(new IntPtr(onRefreshAddress), ContentsFinder_OnRefresh);

                        onDrawHook.Enable();
                        onFinalizeHook.Enable();
                        onUpdateHook.Enable();
                        onRefreshHook.Enable();

                        contentsFinderShowHook!.Disable();
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Something went wrong when the Duty Finder was opened");
            }

            return result;
        }

        private byte ContentsFinder_OnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3)
        {
            var result = onRefreshHook!.Original(atkUnitBase, a2, a3);

            try
            {
                if (Settings.Enabled)
                {
                    wondrousTailsStatus = WondrousTailsModule.GetAllTaskData(wondrousTails);
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Something when wrong on Duty Finder Tab Change or Duty Finder Refresh");
            }

            return result;
        }

        private byte ContentsFinder_Update(AtkUnitBase* atkUnitBase)
        {
            var result = onUpdateHook!.Original(atkUnitBase);
            try
            {
                if (Settings.Enabled)
                {
                    UpdateWondrousTails(atkUnitBase);
                }
            }
            catch (Exception e)
            {
                PluginLog.Error(e, "Something when wrong on Duty Finder Update");
            }

            return result;
        }

        private void ContentsFinder_Draw(AtkUnitBase* atkUnitBase)
        {
            onDrawHook!.Original(atkUnitBase);
            if (atkUnitBase == null) return;

            try
            {
                if (Settings.Enabled)
                {
                    foreach (var i in Enumerable.Range(61001, 15).Append(6))
                    {
                        var id = (uint)i;

                        AddImageNodeByID(atkUnitBase, id);
                    }
                }
            }
            catch (Exception e)
            {
                PluginLog.Error(e, "Something when wrong on Duty Finder Draw");
            }
        }

        private void* ContentsFinder_Finalize(AtkUnitBase* atkUnitBase)
        {
            if (atkUnitBase == null) return null;

            try
            {
                foreach (var i in Enumerable.Range(61001, 15).Append(6))
                {
                    var id = (uint)i;

                    SetImageNodeVisibility(atkUnitBase, id, 29, false);
                    SetImageNodeVisibility(atkUnitBase, id, 30, false);
                }
            }
            catch (Exception e)
            {
                PluginLog.Error(e, "Something when wrong on Duty Finder Close");
            }

            return onFinalizeHook!.Original(atkUnitBase);
        }

        //
        //  Implementation
        //
        private ButtonState? IsWondrousTailsDuty(AtkUnitBase* baseNode, uint id)
        {
            var textNode = GetListItemTextNode(baseNode, id);

            if(textNode == null) return null;

            var nodeString = textNode->NodeText.ToString().ToLower();
            var nodeRegexString = Regex.Replace(nodeString, "[^\\p{L}\\p{N}]", "");
            
            var containsEllipsis = nodeString.Contains("...");

            foreach (var result in contentFinderDuties)
            {
                if (containsEllipsis)
                {
                    var nodeStringLength = nodeRegexString.Length;

                    if (result.SearchKey.Length <= nodeStringLength) continue;

                    if (result.SearchKey[..nodeStringLength] == nodeRegexString)
                    {
                        return InWondrousTailsBook(result.Value);
                    }
                }
                else if (result.SearchKey == nodeRegexString)
                {
                    return InWondrousTailsBook(result.Value);
                }
            }

            return null;
        }

        private ButtonState? InWondrousTailsBook(uint duty)
        {
            foreach (var taskStatus in wondrousTailsStatus)
            {
                if (taskStatus.DutyList.Contains(duty))
                {
                    return taskStatus.TaskState;
                }
            }

            return null;
        }

        private void UpdateWondrousTails(AtkUnitBase* baseNode)
        {
            foreach (var i in Enumerable.Range(61001, 15).Append(6))
            {
                var id = (uint)i;

                var taskState = IsWondrousTailsDuty(baseNode, id);

                if (taskState == null || !WondrousTailsModule.InventoryContainsWondrousTailsBook())
                {
                    SetImageNodeVisibility(baseNode, id, 29, false);
                    SetImageNodeVisibility(baseNode, id, 30, false);
                }
                else if (taskState == ButtonState.Unavailable)
                {
                    SetImageNodeVisibility(baseNode, id, 29, false);
                    SetImageNodeVisibility(baseNode, id, 30, true);
                }
                else if (taskState is ButtonState.AvailableNow or ButtonState.Completable)
                {
                    SetImageNodeVisibility(baseNode, id, 29, true);
                    SetImageNodeVisibility(baseNode, id, 30, false);
                }
            }
        }

        private void AddImageNodeByID(AtkUnitBase* baseNode, uint id)
        {

            var targetNode = GetListItemNode(baseNode, id);

            if (targetNode == null) return;

            var uldManager = targetNode->Component->UldManager;
            var cloverNode = Node.GetNodeByID<AtkImageNode>(uldManager, 29, NodeType.Image);
            var emptyCloverNode = Node.GetNodeByID<AtkImageNode>(uldManager, 30, NodeType.Image);

            if (emptyCloverNode == null && cloverNode == null)
            {
                // Place new node before the text node
                var textNode = (AtkResNode*)GetTextNode(targetNode);

                // Coordinates of clover node
                var clover = new Vector2(97, 65);

                // Coordinates of missing clover node
                var empty = new Vector2(75, 63);

                MakeImageNode(targetNode, textNode, 29, clover);
                MakeImageNode(targetNode, textNode, 30, empty, new Vector2(1, 0));
            }
        }

        private AtkUnitBase* GetContentsFinderPointer()
        {
            return (AtkUnitBase*)Service.GameGui.GetAddonByName("ContentsFinder", 1);
        }

        private AtkComponentNode* GetListItemNode(AtkUnitBase* rootNode, uint nodeID)
        {
            var treeListBaseNode = Node.GetComponentNode(rootNode, 52);

            if (treeListBaseNode == null) return null;

            var listItemNode = Node.GetNodeByID<AtkComponentNode>(treeListBaseNode, nodeID);

            return listItemNode;
        }

        private AtkTextNode* GetTextNode(AtkComponentNode* listItemNode)
        {
            return Node.GetNodeByID<AtkTextNode>(listItemNode, 5);
        }

        private AtkTextNode* GetListItemTextNode(AtkUnitBase* rootNode, uint id)
        {
            var listItemNode = GetListItemNode(rootNode, id);

            if(listItemNode == null) return null;

            var textNode = GetTextNode(listItemNode);

            return textNode;
        }

        private void MakeImageNode(AtkComponentNode* rootNode, AtkResNode* beforeNode, uint newNodeID, Vector2 textureCoordinates, Vector2 positionOffset = default)
        {
            var customNode = IMemorySpace.GetUISpace()->Create<AtkImageNode>();
            customNode->AtkResNode.Type = NodeType.Image;
            customNode->AtkResNode.NodeID = newNodeID;
            customNode->AtkResNode.Flags = 8243;
            customNode->AtkResNode.DrawFlags = 0;
            customNode->WrapMode = 1;
            customNode->Flags = 0;

            var partsList = (AtkUldPartsList*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldPartsList), 8);
            if (partsList == null)
            {
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

            part->U = (ushort)textureCoordinates.X;
            part->V = (ushort)textureCoordinates.Y;
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

            customNode->LoadTexture("ui/uld/WeeklyBingo.tex");

            customNode->AtkResNode.ToggleVisibility(true);

            customNode->AtkResNode.SetWidth(20);
            customNode->AtkResNode.SetHeight(20);

            short xPosition = (short)(290 + positionOffset.X);
            short yPosition = (short)(2 + positionOffset.Y);

            customNode->AtkResNode.SetPositionShort(xPosition, yPosition);

            var prev = beforeNode->PrevSiblingNode;
            customNode->AtkResNode.ParentNode = beforeNode->ParentNode;

            beforeNode->PrevSiblingNode = (AtkResNode*) customNode;
            prev->NextSiblingNode = (AtkResNode*) customNode;

            customNode->AtkResNode.PrevSiblingNode = prev;
            customNode->AtkResNode.NextSiblingNode = beforeNode;

            rootNode->Component->UldManager.UpdateDrawNodeList();
        }

        private void SetImageNodeVisibility(AtkUnitBase* baseNode, uint id, uint nodeTypeID, bool visible)
        {
            var firstNode = GetListItemNode(baseNode, id);

            if (firstNode == null) return;

            var uldManager = firstNode->Component->UldManager;
            var customNode = Node.GetNodeByID<AtkImageNode>(uldManager, nodeTypeID, NodeType.Image);

            if (customNode != null)
            {
                customNode->AtkResNode.ToggleVisibility(visible);
            }
        }
    }
}
