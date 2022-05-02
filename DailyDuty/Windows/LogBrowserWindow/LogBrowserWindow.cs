using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using DailyDuty.Data;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Windows.LogBrowserWindow
{
    internal class LogBrowserWindow : Window, ICommand, IDisposable
    {

        private readonly List<CharacterLogFile> logFiles = new();
        private CharacterLogFile? selectedLogFile;

        public LogBrowserWindow() : base("DailyDuty Log Browser")
        {
            Service.WindowSystem.AddWindow(this);

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(700, 500),
                MaximumSize = new(9999,9999)
            };

            Flags |= ImGuiWindowFlags.NoScrollbar;
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;

            LoadLogFiles();
        }

        private void LoadLogFiles()
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var directory = new DirectoryInfo(appdata + @"\XIVLauncher\pluginConfigs\DailyDuty");

            logFiles.Clear();

            if (directory.Exists)
            {
                foreach (var logFile in directory.GetFiles("*.log"))
                {
                    var localContentID = ulong.Parse(Path.GetFileNameWithoutExtension(logFile.Name));
                    var characterLogFile = Configuration.LoadCharacterLogFile(localContentID);

                    logFiles.Add(characterLogFile);
                }
            }
        }

        public void Dispose()
        {
            Service.WindowSystem.RemoveWindow(this);
        }

        public override void PreOpenCheck()
        {
            if (!Service.SystemConfiguration.DeveloperMode || Service.ClientState.IsPvP)
                IsOpen = false;
        }

        public override void Draw()
        {
            var contentWidth = ImGui.GetContentRegionAvail();

            if(ImGui.BeginChild("SelectionPane", ImGuiHelpers.ScaledVector2(250.0f, -29.0f), true))
            {
                ImGui.BeginListBox("###CharacterSelectListBox", new Vector2(-1));

                foreach (var characterLogFile in logFiles.OrderBy(log => log.CharacterName))
                {
                    if (ImGui.Selectable(characterLogFile.CharacterName, selectedLogFile == characterLogFile))
                    {
                        selectedLogFile = characterLogFile;
                    }
                }

                ImGui.EndListBox();

                ImGui.EndChild();
            }

            var cursorPosition = ImGui.GetCursorPos();

            ImGui.SameLine();
            if (ImGui.BeginChild("ContentsPane", contentWidth with {X = contentWidth.X - 260.0f * ImGuiHelpers.GlobalScale}, true))
            {

                if(ImGui.BeginTabBar("LogTabBar"))
                {
                    if (selectedLogFile != null)
                    {
                        foreach (var key in selectedLogFile.Messages.Keys)
                        {
                            if(ImGui.BeginTabItem(key.ToString()))
                            {
                                foreach (var message in selectedLogFile.Messages[key].OrderByDescending(t => t.Time))
                                {
                                    message.Draw();
                                }

                                ImGui.EndTabItem();
                            }
                        }
                    }

                    ImGui.EndTabBar();
                }

                ImGui.EndChild();
            }

            ImGui.SetCursorPos(cursorPosition);
            if (ImGui.Button("Refresh", ImGuiHelpers.ScaledVector2(250.0f, 25)))
            {
                LoadLogFiles();
            }
        }

        void ICommand.Execute(string? primaryCommand, string? secondaryCommand)
        {
            switch (primaryCommand)
            {
                case "loghelper":
                    IsOpen = true;
                    break;

                case "supersecretpassword":
                    Chat.Debug("Password Accepted - Admin Mode Enabled");
                    Service.SystemConfiguration.DeveloperMode = true;
                    Service.SystemConfiguration.Save();
                    break;

                case "goodnight" when Service.SystemConfiguration.DeveloperMode:
                    Chat.Debug("Good Night - Admin Mode Disabled");
                    Service.SystemConfiguration.DeveloperMode = false;
                    Service.SystemConfiguration.Save();
                    break;
            }
        }
    }
}
