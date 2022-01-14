using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem.DisplayModules;
using DailyDuty.System.Utilities;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System.Modules
{
    internal unsafe class WondrousTailsModule : Module
    {
        protected readonly Daily.WondrousTailsSettings Settings = Service.Configuration.WondrousTailsSettings;
        private readonly Stopwatch loginNoticeStopwatch = new();
        private readonly Stopwatch BookUpdateStopwatch = new();

        public WondrousTailsModule()
        {
            Service.ClientState.Login += OnLogin;
            Service.ClientState.TerritoryChanged += OnTerritoryChanged;

            Settings.BookDeadline = DateTime.MinValue;
            OnLogin(this, EventArgs.Empty);
        }

        private void OnTerritoryChanged(object? sender, ushort e)
        {
            var node = FindNode(e);

            if (node != null)
            {
                var buttonState = node.Value.Item1;

                switch (buttonState)
                {
                    case ButtonState.Unavailable:
                        if (Settings.SecondChancePoints > 0)
                        {
                            Util.PrintMessage("[WondrousTails] This instance is available for a stamp if you re-roll it!");
                        }
                        break;

                    case ButtonState.AvailableNow:
                        Util.PrintMessage("[WondrousTails] A stamp is already available for this instance.");
                        break;

                    case ButtonState.Completable:
                        Util.PrintMessage("[WondrousTails] Completing this instance will reward you with a stamp!");
                        break;

                    case ButtonState.Unknown:
                        break;
                }
            }
        }
        
        public override void Update()
        {
            var frameCount = Service.PluginInterface.UiBuilder.FrameCount;
            if (frameCount % 10 != 0) return;

            var dataStale = DateTime.Now >= Settings.BookDeadline;

            if (loginNoticeStopwatch.Elapsed >= TimeSpan.FromSeconds(5) && loginNoticeStopwatch.IsRunning)
            {
                if (dataStale)
                {
                    Util.PrintMessage("WondrousTails Data is out of date, please open your Wondrous Tails Book.");
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

                    Util.PrintMessage("WondrousTails Book Successfully Updated!");

                    if (loginNoticeStopwatch.IsRunning)
                        loginNoticeStopwatch.Stop();
                }
            }

            if (IsBookOpen())
            {
                if (BookUpdateStopwatch.IsRunning && BookUpdateStopwatch.Elapsed >= TimeSpan.FromSeconds(1))
                {
                    BookUpdateStopwatch.Stop();
                    BookUpdateStopwatch.Reset();
                }


                if (BookUpdateStopwatch.IsRunning == false)
                {
                    BookUpdateStopwatch.Start();
                    UpdateBook();
                }

            }
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

                //PluginLog.Information($"NodeData: {(IntPtr)nextNode:X8}, {id}, {i}, {Settings.Data[i].Item1}");


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
            switch (id)
            {
                // Dungeons 1-49
                case 113001:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 1 and <= 49)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                //The Feast
                case 113049:
                    return new(){0};

                //Memoria Misera (Extreme)
                case 113114:
                    return new() { 913};

                // Hells' Kier (Extreme)
                case 113090:
                    return new() { 811};

                //Emanation (Extreme)
                case 113079:
                    return new() { 720};

                //The Striking Tree(Extreme)
                case 113023:
                    return new() { 375};

                //The Navel (Extreme)
                case 113019:
                    return new() { 296};

                // Palace of the Dead / Heaven on High
                case 113056:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId is 21)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Alphascape 1.0
                case 113081:
                    return new() {789};

                // Alexander - Arm of the Father
                case 113012:
                    return new() {444};

                //The Final Coil of Bahamut - Turn 3
                case 113043:
                    return new() {195};

                // the Orbonne Monastery
                case 113080:
                    return new() {826};

                // The World of Darkness
                case 113007:
                    return new() {151};

                // Dungeons Level 90
                case 113110:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 90)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Level 80
                case 113087:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 80)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons 71-79
                case 113086:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 71 and <= 79)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

            }

            PluginLog.Information($"[WondrousTails] Unrecognized Image ID: {id}");
            return new List<uint>();
        }

        private void OnLogin(object? sender, EventArgs e)
        {
            loginNoticeStopwatch.Start();
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
