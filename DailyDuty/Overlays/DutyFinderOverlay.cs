using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.DutyRoulette;
using DailyDuty.Data.ModuleData.WondrousTails;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.DailySettings;
using DailyDuty.Data.SettingsObjects.WeeklySettings;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers.WondrousTails;
using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Overlays
{
    internal unsafe class DutyFinderOverlay : IDisposable, IOverlay
    {
        private DutyFinderOverlaySettings Settings => Service.Configuration.DutyFinderOverlaySettings;
        private DutyRouletteSettings DutyRouletteSettings => Service.Configuration.Current().DutyRoulette;
        private WondrousTailsSettings WondrousTailsSettings => Service.Configuration.Current().WondrousTails;

        public OverlayName OverlayName => OverlayName.DutyFinder;


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

        private readonly List<DutyFinderSearchResult> contentFinderDuties = new();
        private readonly List<DutyFinderSearchResult> dutyRouletteDuties = new();
        private IEnumerable<TrackedRoulette> DutyRoulettes => DutyRouletteSettings.TrackedRoulettes;
        private List<(ButtonState, List<uint>)> wondrousTailsStatus;

        private ByteColor userDefaultTextColor;

        public DutyFinderOverlay()
        {
            SignatureHelper.Initialise(this);

            var contentFinderData = Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                !.Where(cfc => cfc.Name != string.Empty);

            foreach (var cfc in contentFinderData)
            {
                var simplifiedString = Regex.Replace(cfc.Name.ToString().ToLower(), "[^\\p{L}\\p{N}]", "");

                contentFinderDuties.Add(new()
                {
                    Value = cfc.TerritoryType.Row,
                    SearchKey = simplifiedString
                });
            }

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

            wondrousTailsStatus = GetAllTaskData().ToList();

            Service.Framework.Update += FrameworkOnUpdate;
        }

        private void FrameworkOnUpdate(Framework framework)
        {
            if (IsContentFinderOpen() != true) return;

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

            SaveDefaultTextColor();
                
            Service.Framework.Update -= FrameworkOnUpdate;
        }
        
        public void Dispose()
        {
            Service.Framework.Update -= FrameworkOnUpdate;

            onDrawHook?.Dispose();
            onFinalizeHook?.Dispose();
            onUpdateHook?.Dispose();
            onRefreshHook?.Dispose();
        }

        private byte OnRefresh(AtkUnitBase* atkUnitBase, int a2, long a3)
        {
            var result = onRefreshHook!.Original(atkUnitBase, a2, a3);

            if (WondrousTailsSettings.Enabled && Settings.Enabled)
            {
                wondrousTailsStatus = GetAllTaskData().ToList();
            }

            if (Settings.DutyRouletteOverlayEnabled == true && DutyRouletteSettings.Enabled)
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

        private void SaveDefaultTextColor()
        {
            var textNode = GetListItemTextNode(6);

            Chat.Debug("Saving Colors");

            userDefaultTextColor = textNode->TextColor;

            Chat.Debug($"Color:{userDefaultTextColor.R}::{userDefaultTextColor.G}::{userDefaultTextColor.B}::{userDefaultTextColor.A}");
        }

        private void ResetDefaultTextColor()
        {
            foreach (var i in Enumerable.Range(61001, 15).Append(6))
            {
                var id = (uint)i;

                var textNode = GetListItemTextNode(id);

                textNode->TextColor = userDefaultTextColor;
            }
        }

        private byte OnUpdate(AtkUnitBase* atkUnitBase)
        {
            var result = onUpdateHook!.Original(atkUnitBase);

            if (Settings.Enabled == false) return result;
            
            UpdateWondrousTails();
            
            return result;
        }

        private void UpdateWondrousTails()
        {
            foreach (var i in Enumerable.Range(61001, 15).Append(6))
            {
                var id = (uint)i;

                var taskState = IsWondrousTailsDuty(id);

                if (taskState == null)
                {
                    SetImageNodeVisibility(id, 29, false);
                    SetImageNodeVisibility(id, 30, false);
                }
                else if (taskState == ButtonState.Unavailable)
                {
                    SetImageNodeVisibility(id, 29, false);
                    SetImageNodeVisibility(id, 30, true);
                }
                else if (taskState is ButtonState.AvailableNow or ButtonState.Completable)
                {
                    SetImageNodeVisibility(id, 29, true);
                    SetImageNodeVisibility(id, 30, false);
                }
            }
        }

        private void OnDraw(AtkUnitBase* atkUnitBase)
        {
            onDrawHook!.Original(atkUnitBase);
            if (atkUnitBase == null) return;

            if (Settings.Enabled == false) return;

            if (Settings.WondrousTailsOverlayEnabled == true && WondrousTailsSettings.Enabled)
            {
                foreach (var i in Enumerable.Range(61001, 15).Append(6))
                {
                    var id = (uint)i;

                    AddImageNodeByID(id);
                }
            }

            if (Settings.DutyRouletteOverlayEnabled == true && DutyRouletteSettings.Enabled)
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

        private void* OnFinalize(AtkUnitBase* atkUnitBase)
        {
            if (atkUnitBase == null) return null;

            foreach (var i in Enumerable.Range(61001, 15).Append(6))
            {
                var id = (uint) i;
                DestroyNode(id, 29);
                DestroyNode(id, 30);
            }

            return onFinalizeHook!.Original(atkUnitBase);
        }

        private void DestroyNode(uint id, uint nodeId)
        {
            var firstNode = GetListItemNode(id);

            if (firstNode == null) return;

            var uldManager = firstNode->Component->UldManager;
            var customNode = Node.GetNodeByID<AtkImageNode>(uldManager, nodeId, NodeType.Image);

            if (customNode != null)
            {
                if (customNode->AtkResNode.PrevSiblingNode != null)
                    customNode->AtkResNode.PrevSiblingNode->NextSiblingNode = customNode->AtkResNode.NextSiblingNode;

                if (customNode->AtkResNode.NextSiblingNode != null)
                    customNode->AtkResNode.NextSiblingNode->PrevSiblingNode = customNode->AtkResNode.PrevSiblingNode;


                firstNode->Component->UldManager.UpdateDrawNodeList();

                customNode->PartsList->Parts->UldAsset->AtkTexture.Destroy(true);
                IMemorySpace.Free(customNode->PartsList->Parts->UldAsset, (ulong) sizeof(AtkUldAsset));

                IMemorySpace.Free(customNode->PartsList->Parts, (ulong) sizeof(AtkUldPart));

                IMemorySpace.Free(customNode->PartsList, (ulong) sizeof(AtkUldPartsList));
                
                customNode->UnloadTexture();
                customNode->AtkResNode.Destroy(true);
                IMemorySpace.Free(customNode, (ulong)sizeof(AtkImageNode));
            }
        }
        
        //
        //  Implementation
        //

        private ButtonState? IsWondrousTailsDuty(uint id)
        {
            var textNode = GetListItemTextNode(id);

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
            foreach (var (buttonState, task) in wondrousTailsStatus)
            {
                if (task.Contains(duty))
                {
                    return buttonState;
                }
            }

            return null;
        }

        private void AddImageNodeByID(uint id)
        {
            var treeNode = GetTreeListBaseNode();
            var targetNode = GetListItemNode(id);

            if (treeNode == null || targetNode == null) return;

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
            return Node.GetComponentNode(GetContentsFinderPointer(), 52);
        }

        private AtkComponentNode* GetListItemNode(uint nodeID)
        {
            return Node.GetNodeByID<AtkComponentNode>(GetTreeListBaseNode(), nodeID);
        }

        private AtkResNode* GetTabBarNode()
        {
            var addonNode = GetContentsFinderPointer();

            return (AtkResNode*) addonNode->GetNodeById(40);
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
        
        private AtkImageNode* MakeImageNode(AtkComponentNode* rootNode, AtkResNode* beforeNode, uint newNodeID, Vector2 textureCoordinates, Vector2 positionOffset = default)
        {
            var customNode = IMemorySpace.GetUISpace()->Create<AtkImageNode>();
            customNode->AtkResNode.Type = NodeType.Image;
            customNode->AtkResNode.NodeID = newNodeID;
            customNode->AtkResNode.Flags = 35;
            customNode->AtkResNode.DrawFlags = 0;
            customNode->WrapMode = 2;
            customNode->Flags = 0;

            var partsList = (AtkUldPartsList*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldPartsList), 8);
            if (partsList == null) {
                customNode->AtkResNode.Destroy(true);
                return null;
            }

            partsList->Id = 0;
            partsList->PartCount = 1;

            var part = (AtkUldPart*) IMemorySpace.GetUISpace()->Malloc((ulong) sizeof(AtkUldPart), 8);
            if (part == null) {
                IMemorySpace.Free(partsList, (ulong)sizeof(AtkUldPartsList));
                customNode->AtkResNode.Destroy(true);
                return null;
            }

            //part->U = 97;
            //part->V = 65;
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
                return null;
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

            return customNode;
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

        private void SetImageNodeVisibility(uint id, uint nodeTypeID, bool visible)
        {
            var firstNode = GetListItemNode(id);

            if (firstNode == null) return;

            var uldManager = firstNode->Component->UldManager;
            var customNode = Node.GetNodeByID<AtkImageNode>(uldManager, nodeTypeID, NodeType.Image);

            if (customNode != null)
            {
                customNode->AtkResNode.ToggleVisibility(visible);
            }
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
