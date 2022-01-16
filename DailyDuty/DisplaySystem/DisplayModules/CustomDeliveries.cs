using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class CustomDeliveries : DisplayModule
    {
        private readonly Weekly.CustomDeliveriesSettings settings = Service.Configuration.CustomDeliveriesSettings;

        public CustomDeliveries()
        {
            CategoryString = "Custom Deliveries";
        }

        protected override void DrawContents()
        {
            ImGui.Checkbox("Enabled##CustomDeliveries", ref settings.Enabled);

            if (settings.Enabled)
            {
                ImGui.Indent(15);

                ImGui.Checkbox("Notifications##CustomDeliveries", ref settings.NotificationEnabled);

                ImGui.Indent(-15);
            }

            ImGui.Spacing();
            ImGui.Separator();
        }

        public override void Dispose()
        {
        }
    }
}
