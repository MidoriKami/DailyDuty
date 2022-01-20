using DailyDuty.ConfigurationSystem;

namespace DailyDuty.System.Modules
{
    internal class CustomDeliveriesModule : Module
    {
        private readonly Weekly.CustomDeliveriesSettings Settings = Service.Configuration.CustomDeliveriesSettings;

        public CustomDeliveriesModule()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void Dispose()
        {
        }

        public override bool IsCompleted()
        {
            return false;
        }
    }
}
