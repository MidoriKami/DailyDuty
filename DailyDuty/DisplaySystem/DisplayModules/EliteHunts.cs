using System;
using System.Numerics;
using DailyDuty.ConfigurationSystem;
using DailyDuty.Data;
using Dalamud.Interface;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using ImGuiNET;

#pragma warning disable CS0649
#pragma warning disable CS0169

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal unsafe class EliteHunts : DisplayModule
    {
        private Weekly.EliteHuntSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].EliteHuntSettings;

        protected override GenericSettings GenericSettings => Settings;

        // https://github.com/SheepGoMeh/HuntBuddy/blob/master/Structs/MobHuntStruct.cs
        [Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
        private EliteHuntStruct* huntData;

        public EliteHunts()
        {
            CategoryString = "Elite Hunts";

            SignatureHelper.Initialise(this);
        }

        protected override void DisplayData()
        {
            ImGui.Text("Only the checked lines will be evaluated for notifications");
            ImGui.Spacing();

            if (ImGui.BeginTable($"##EditTable{CategoryString}", 2))
            {
                foreach (var hunt in Settings.TrackedHunts)
                {
                    var label = hunt.Expansion.ToString();

                    ImGui.TableNextColumn();
                    ImGui.Checkbox($"##{hunt.Expansion}{CategoryString}", ref hunt.Tracked);
                    ImGui.SameLine();
                    ImGui.Text(label);

                    ImGui.TableNextColumn();
                    DrawConditionalText(huntData->Obtained(hunt.Expansion) || hunt.Obtained, "Mark Obtained", "Hunt Mark Available");
                }

                ImGui.EndTable();
            }
        }

        protected override void EditModeOptions()
        {
            ImGui.Text("If a mark is listed as 'Available' but you already killed it this week\n" +
                       "click 'Collected' to mark it as completed this week.");

            if (ImGui.BeginTable($"##EditTable{CategoryString}", 3))
            {
                foreach (var hunt in Settings.TrackedHunts)
                {
                    var label = hunt.Expansion.ToString();

                    ImGui.TableNextColumn();
                    ImGui.Text(label);

                    ImGui.TableNextColumn();
                    if (ImGui.Button($"Collected##{label}{CategoryString}", ImGuiHelpers.ScaledVector2(100, 25)))
                    {
                        hunt.Obtained = true;
                        Service.Configuration.Save();
                    }

                    ImGui.TableNextColumn();
                    if (ImGui.Button($"Not Collected##{label}{CategoryString}", ImGuiHelpers.ScaledVector2(100, 25)))
                    {
                        hunt.Obtained = false;
                        Service.Configuration.Save();
                    }

                }

                ImGui.EndTable();
            }
        }

        protected override void NotificationOptions()
        {
            OnLoginReminderCheckbox(Settings);

            OnTerritoryChangeCheckbox(Settings);
        }

        public override void Dispose()
        {
        }

        private static void DrawConditionalText(bool condition, string trueString, string falseString)
        {
            if (condition)
            {
                ImGui.TextColored(new Vector4(0, 255, 0, 0.8f), trueString);
            }
            else
            {
                ImGui.TextColored(new Vector4(185, 0, 0, 0.8f), falseString);
            }
        }
    }
}
