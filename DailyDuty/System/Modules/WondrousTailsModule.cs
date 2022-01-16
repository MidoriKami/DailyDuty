using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Utilities;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System.Modules
{
    internal unsafe class WondrousTailsModule : Module
    {
        protected readonly Weekly.WondrousTailsSettings Settings = Service.Configuration.WondrousTailsSettings;
        private readonly Stopwatch loginNoticeStopwatch = new();
        private readonly Stopwatch bookUpdateStopwatch = new();

        private uint LastDutyInstanceID = 0;
        private bool LastInstanceWasDuty = false;

        private readonly ConditionManager conditionManager = new();

        public WondrousTailsModule()
        {
            Service.ClientState.Login += OnLogin;
            Service.ClientState.TerritoryChanged += OnTerritoryChanged;
        }

        private void OnLogin(object? sender, EventArgs e)
        {
            if (Settings.Enabled == false) return;

            loginNoticeStopwatch.Start();
        }

        private void OnTerritoryChanged(object? sender, ushort e)
        {
            if (Settings.Enabled == false) return;

            if (ConditionManager.IsBoundByDuty() && Settings.NotificationEnabled == true)
            {
                LastInstanceWasDuty = true;
                LastDutyInstanceID = e;
                OnDutyStartNotification();
            }
            else if(LastInstanceWasDuty == true)
            {
                OnDutyEndNotification();
                LastInstanceWasDuty = false;
            }
            else
            {
                LastInstanceWasDuty = false;
            }
        }

        private void OnDutyEndNotification()
        {
            var node = FindNode(LastDutyInstanceID);
            if (node == null) return;

            var buttonState = node.Value.Item1;

            if (buttonState == ButtonState.Completable)
            {
                Util.PrintWondrousTails("You can claim a stamp for the last instance!");
            }
        }

        private void OnDutyStartNotification()
        {
            var node = FindNode(LastDutyInstanceID);
            if (node == null) return;

            var buttonState = node.Value.Item1;

            switch (buttonState)
            {
                case ButtonState.Unavailable:
                    if (Settings.SecondChancePoints > 0)
                    {
                        Util.PrintWondrousTails($"This instance is available for a stamp if you re-roll it! You have {Settings.SecondChancePoints} Re-Rolls Available.");
                    }
                    break;

                case ButtonState.AvailableNow:
                    Util.PrintWondrousTails("A stamp is already available for this instance.");
                    break;

                case ButtonState.Completable:
                    Util.PrintWondrousTails("Completing this instance will reward you with a stamp!");
                    break;

                case ButtonState.Unknown:
                    break;
            }
        }

        public override void Update()
        {
            if (Settings.Enabled == false) return;

            var frameCount = Service.PluginInterface.UiBuilder.FrameCount;
            if (frameCount % 10 != 0) return;

            var dataStale = DateTime.Now >= Settings.BookDeadline;

            if (loginNoticeStopwatch.Elapsed >= TimeSpan.FromSeconds(5) && loginNoticeStopwatch.IsRunning)
            {
                if (dataStale)
                {
                    Util.PrintWondrousTails("Data is out of date, please open your Wondrous Tails Book.");
                }

                loginNoticeStopwatch.Stop();
            }

            if (dataStale)
            {
                var wondrousTailsPointer = GetWondrousTailsPointer();

                if (wondrousTailsPointer != null)
                {
                    UpdateBook();
                    Service.Configuration.Save();

                    Util.PrintWondrousTails("Book Successfully Updated!");

                    if (loginNoticeStopwatch.IsRunning)
                        loginNoticeStopwatch.Stop();
                }
            }

            if (IsBookOpen())
            {
                Util.UpdateDelayed(bookUpdateStopwatch, TimeSpan.FromSeconds(1), UpdateBook);
            }
        }

        public static bool IsWondrousTailsBookComplete()
        {
            return Service.Configuration.WondrousTailsSettings.NumberOfPlacedStickers == 9;
        }
        
        private bool IsBookOpen()
        {
            var bookAtkBase = GetWondrousTailsPointer();
            if (bookAtkBase == null) return false;

            return true;
        }

        private void UpdateBook()
        {
            UpdateRows();

            UpdateStickerCount();

            UpdateSecondChanceCount();

            UpdateDeadline();
        }

        private void UpdateSecondChanceCount()
        {
            var bookAtkBase = GetWondrousTailsPointer();
            if (bookAtkBase == null) return;

            var secondChanceNode = bookAtkBase->AtkUnitBase.GetNodeById(33);

            var textNode = GetNodeList(secondChanceNode)[4]->GetAsAtkTextNode();

            Settings.SecondChancePoints = uint.Parse(textNode->NodeText.ToString());
        }

        private void UpdateStickerCount()
        {
            var bookAtkBase = GetWondrousTailsPointer();
            if(bookAtkBase == null) return;

            var stickerCountNode = bookAtkBase->AtkUnitBase.GetTextNodeById(60);

            Settings.NumberOfPlacedStickers = uint.Parse(stickerCountNode->NodeText.ToString());
        }

        private void UpdateDeadline()
        {
            Settings.BookDeadline = GetDeadlineDateTime();
        }

        private void UpdateRows()
        {
            // Node Order as Displayed: 12, 13, 14, 15
            var bookAtkBase = GetWondrousTailsPointer();
            if (bookAtkBase == null) return;
            
            var gridNodes = bookAtkBase->AtkUnitBase.GetNodeById(9);

            AtkResNode* nextNode = gridNodes->ChildNode;
            for (int i = 0; i < 16; ++i)
            {
                var id = GetImageID(nextNode);

                Settings.Data[i] =
                    new(GetButtonState(nextNode), GetInstanceListFromImageID(id));

                nextNode = nextNode->PrevSiblingNode;
            }
        }

        private ButtonState GetButtonState(AtkResNode* node)
        {
            if (GetNodeList(node)[16]->IsVisible)
                return ButtonState.AvailableNow;

            if (GetNodeList(node)[7]->IsVisible)
                return ButtonState.Unavailable;
            else
                return ButtonState.Completable;
        }

        private void PrintWondrousTailsData()
        {
            foreach(var (buttonState, list) in Settings.Data)
            {
                PluginLog.Information($"{buttonState}, " + "{ " + string.Join(", ", list) + " }");
            }
        }

        private (ButtonState, List<uint>)? FindNode(uint instanceID)
        {
            foreach (var (pointer, list) in Settings.Data)
            {
                if (list.Contains(instanceID))
                {
                    return (pointer, list);
                }
            }

            return null;
        }

        private AtkResNode** GetNodeList(AtkResNode* inputNode)
        {
            var atkResNode = inputNode;
            if (atkResNode == null) return null;

            if (atkResNode->GetAsAtkComponentNode() == null) return null;
            var atkComponentNode = atkResNode->GetAsAtkComponentNode();

            if (atkComponentNode == null) return null;
            var atkComponentBase = atkComponentNode->Component;

            if (atkComponentBase == null) return null;
            return atkComponentBase->UldManager.NodeList;
        }

        private uint GetImageID(AtkResNode* inputNode)
        {
            var nodeList = GetNodeList(inputNode);

            if (nodeList == null) return 0;
            var imageNode = (AtkImageNode*) nodeList[2];

            if(imageNode == null) return 0;

            return imageNode->PartsList->Parts->UldAsset->AtkTexture.Resource->Unk_1;
        }

        private List<uint> GetInstanceListFromImageID(uint id)
        {
            var values = TryGetFromDatabase(id);

            if (values != null)
            {
                return new() {values.Value};
            }

            switch (id)
            {
                // Dungeons Lv 1-49
                case 113001:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 1 and <= 49)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 50
                case 113002:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 50)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 51-59
                case 113003:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 51 and <= 59)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 60
                case 113004:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 60)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 61-69
                case 113062:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 61 and <= 69)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 70
                case 113063:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 70)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 71-79
                case 113086:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 71 and <= 79)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 80
                case 113087:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 80)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons lv 81-89
                case 113109:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 81 and <= 89)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 90
                case 113110:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 90)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Palace of the Dead / Heaven on High
                case 113056:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId is 21)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Treasure Maps
                case 113050:
                    // todo: Find Treasure Map Instance List
                    break;

                //The Feast
                case 113049:
                    return new() { 0 };

            }

            PluginLog.Information($"[WondrousTails] Unrecognized Image ID: {id}");
            return new List<uint>();
        }

        private uint? TryGetFromDatabase(uint id)
        {
            var timer = new Stopwatch();
            timer.Start();

            var instanceContentData = Service.DataManager.GetExcelSheet<WeeklyBingoOrderData>()
                !.Where(b => b.Icon == id)
                .Select(b => b.Data)
                .FirstOrDefault();

            if (instanceContentData < 20000)
            {
                return null;
            }

            var data = Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                !.Where(c => c.Content == instanceContentData)
                .Select(c => c.TerritoryType.Value!.RowId)
                .FirstOrDefault();

            timer.Stop();
            PluginLog.Debug($"Elapsed Time: {timer.ElapsedMilliseconds}");


            return data;
        }

        private AddonWeeklyBingo* GetWondrousTailsPointer()
        {
            return (AddonWeeklyBingo*)Service.GameGui.GetAddonByName("WeeklyBingo", 1);
        }

        private DateTime GetDeadlineDateTime()
        {
            var baseAtk = GetWondrousTailsPointer();

            if (baseAtk != null)
            {
                var deadlineAtkTextNode = baseAtk->AtkUnitBase.GetTextNodeById(8);

                if (deadlineAtkTextNode != null)
                {
                    var nodeText = deadlineAtkTextNode->NodeText.ToString();

                    // Strip "Deadline: " text
                    nodeText = nodeText.Remove(0, 11);

                    // Strip " 0:00 a.m." text
                    nodeText = nodeText.Remove(10);

                    return DateTime.Parse(nodeText);
                }
            }

            return DateTime.MinValue;
        }

        public override void Dispose()
        {
            Service.ClientState.Login -= OnLogin;
            Service.ClientState.TerritoryChanged -= OnTerritoryChanged;
        }
    }
}
