using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PartnerUp
{
    internal class SettingsWindow : Window, IDisposable
    {
        private Tab CurrentTab = Tab.General;
        private readonly Vector2 WindowSize = new(400, 200);
        private int AddToBlacklistValue;
        private int RemoveFromBlacklistValue;

        public SettingsWindow() : base("Partner Up Settings Window")
        {
            IsOpen = false;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(WindowSize.X, WindowSize.Y),
                MaximumSize = new(WindowSize.X + 300, WindowSize.Y + 400)
            };
        }

        private enum Tab
        {
            General,
            DancePartner,
            Faerie,
            Kardion,
            Blacklist
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            DrawTabs();

            switch (CurrentTab)
            {
                case Tab.General:
                    DrawGeneralTab();
                    break;

                case Tab.DancePartner:
                    DrawDancePartnerTab();
                    break;

                case Tab.Faerie:
                    DrawFaerieTab();
                    break;

                case Tab.Kardion:
                    DrawKardionTab();
                    break;

                case Tab.Blacklist:
                    DrawBlacklistTab();
                    break;
            }

            DrawSaveAndCloseButtons();
        }

        private void DrawTabs()
        {
            if (ImGui.BeginTabBar("Partner Up Tab Toolbar", ImGuiTabBarFlags.NoTooltip))
            {
                if (ImGui.BeginTabItem("General"))
                {
                    CurrentTab = Tab.General;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Dance Partner"))
                {
                    CurrentTab = Tab.DancePartner;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Faerie"))
                {
                    CurrentTab = Tab.Faerie;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Kardion"))
                {
                    CurrentTab = Tab.Kardion;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Blacklist"))
                {
                    CurrentTab = Tab.Blacklist;
                    ImGui.EndTabItem();
                }
            }
        }

        private void DrawGeneralTab()
        {
            DrawGeneralEnableDisableAll();

            DrawStatus();
        }

        private void DrawGeneralEnableDisableAll()
        {
            ImGui.Text("General Settings");
            ImGui.Separator();
            ImGui.Spacing();

            if (ImGui.Button("Enable All", new(100, 25)))
            {
                Service.Configuration.EnableDancePartnerBanner = true;
                Service.Configuration.EnableKardionBanner = true;
                Service.Configuration.EnableFaerieBanner = true;
            }

            ImGui.SameLine();

            if (ImGui.Button("Disable All", new(100, 25)))
            {
                Service.Configuration.EnableDancePartnerBanner = false;
                Service.Configuration.EnableKardionBanner = false;
                Service.Configuration.EnableFaerieBanner = false;
            }

            ImGui.Spacing();

            ImGui.Checkbox("Disable in Alliance Raids", ref Service.Configuration.DiableInAllianceRaid);

            ImGui.Spacing();

            DrawInstanceLoadDelayTimeTextField();

            ImGui.Spacing();
        }

        private void DrawInstanceLoadDelayTimeTextField()
        {
            ImGui.Text("Grace Period");
            ImGui.InputInt("", ref Service.Configuration.TerritoryChangeDelayTime, 1000, 5000);
            ImGuiComponents.HelpMarker("Hide warnings on map change for (milliseconds)");
            ImGui.Spacing();
        }

        private void DrawStatus()
        {
            ImGui.Text("Warning Statuses");

            ImGui.Separator();
            ImGui.Spacing();

            if ( ImGui.BeginTable("##StatusTable", 2) )
            {
                ImGui.TableNextColumn();
                ImGui.Text("Dance Partner");

                ImGui.TableNextColumn();
                DrawConditionalText(Service.Configuration.EnableDancePartnerBanner, "Enabled", "Disabled");

                ImGui.TableNextColumn();
                ImGui.Text("Faerie");

                ImGui.TableNextColumn();
                DrawConditionalText(Service.Configuration.EnableFaerieBanner, "Enabled", "Disabled");

                ImGui.TableNextColumn();
                ImGui.Text("Kardion");

                ImGui.TableNextColumn();
                DrawConditionalText(Service.Configuration.EnableKardionBanner, "Enabled", "Disabled");

                ImGui.EndTable();
            }
            ImGui.Spacing();

        }

        private void DrawDancePartnerTab()
        {
            ImGui.Text("Dance Partner Warning Settings");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Checkbox("Enable Missing Dance Partner Warning", ref Service.Configuration.EnableDancePartnerBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Force Show Banner", ref Service.Configuration.ForceShowDancePartnerBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Reposition Banner", ref Service.Configuration.RepositionModeDancePartnerBanner);
            ImGui.Spacing();

        }

        private void DrawFaerieTab()
        {
            ImGui.Text("Faerie Warning Settings");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Checkbox("Enable Missing Faerie Warning", ref Service.Configuration.EnableFaerieBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Force Show Banner", ref Service.Configuration.ForceShowFaerieBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Reposition Banner", ref Service.Configuration.RepositionModeFaerieBanner);
            ImGui.Spacing();
        }

        private void DrawKardionTab()
        {
            ImGui.Text("Kardion Warning Settings");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Checkbox("Enable Missing Kardion Warning", ref Service.Configuration.EnableKardionBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Force Show Banner", ref Service.Configuration.ForceShowKardionBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Reposition Banner", ref Service.Configuration.RepositionModeKardionBanner);
            ImGui.Spacing();
        }

        private void DrawBlacklistTab()
        {
            ImGui.Text("Blacklist Settings");
            ImGui.Separator();
            ImGui.Spacing();

            DrawBlacklistSettings();

            ImGui.Spacing();

        }

        private void DrawBlacklistSettings()
        {
            ImGui.Spacing();
            PrintBlackList();
            PrintAddToBlacklist();
            PrintRemoveFromBlacklist();

            ImGui.Spacing();
        }

        private void PrintBlackList()
        {
            ImGui.Text("Currently Blacklisted: ");

            if (Service.Configuration.TerritoryBlacklist.Count > 0)
            {
                var blacklist = Service.Configuration.TerritoryBlacklist;
                ImGui.Text("{" + string.Join(", ", blacklist) + "}");
            }
            else
            {
                ImGui.Text("Blacklist is empty.");
            }

            ImGui.Text($"Currently In: {Service.ClientState.TerritoryType}");
        }

        private void PrintAddToBlacklist()
        {
            ImGui.Spacing();

            ImGui.Text("Add To BlackList");

            ImGui.PushItemWidth(150);

            ImGui.InputInt("##AddToBlacklist", ref AddToBlacklistValue, 0, 0);

            ImGui.PopItemWidth();

            ImGui.SameLine();

            if (ImGui.Button("Add", new(75, 25)))
            {
                AddToBlacklist();
            }
            ImGui.SameLine();

            ImGuiComponents.HelpMarker("Add specified territory to blacklist");
        }

        private void PrintRemoveFromBlacklist()
        {
            ImGui.Spacing();

            ImGui.Text("Remove from Blacklist");

            ImGui.PushItemWidth(150);

            ImGui.InputInt("##RemoveFromBlacklist", ref RemoveFromBlacklistValue, 0, 0);

            ImGui.PopItemWidth();
            ImGui.SameLine();

            if (ImGui.Button("Remove", new(75, 25)))
            {
                RemoveFromBlacklist();
            }
            ImGui.SameLine();

            ImGuiComponents.HelpMarker("Removes specified territory from blacklist");
        }

        private void RemoveFromBlacklist()
        {
            var blacklist = Service.Configuration.TerritoryBlacklist;

            if (blacklist.Contains(RemoveFromBlacklistValue))
            {
                blacklist.Remove(RemoveFromBlacklistValue);
                Service.Configuration.ForceWindowUpdate = true;
            }
        }

        private void AddToBlacklist()
        {
            var blacklist = Service.Configuration.TerritoryBlacklist;

            if (!blacklist.Contains(AddToBlacklistValue))
            {
                blacklist.Add(AddToBlacklistValue);
                Service.Configuration.ForceWindowUpdate = true;
            }
        }

        private void DrawSaveAndCloseButtons()
        {
            ImGui.Spacing();

            if (ImGui.Button("Save", new(100, 25)))
            {
                Service.Configuration.Save();
            }

            ImGui.SameLine(ImGui.GetWindowWidth() -155);

            if (ImGui.Button("Save & Close", new(150, 25)))
            {
                Service.Configuration.Save();
                IsOpen = false;
            }

            ImGui.Spacing();
        }

        private void DrawConditionalText(bool condition, string trueString, string falseString)
        {
            if (condition)
            {
                ImGui.Text(trueString);
            }
            else
            {
                ImGui.Text(falseString);
            }
        }

        public void Dispose()
        {

        }
    }
}
