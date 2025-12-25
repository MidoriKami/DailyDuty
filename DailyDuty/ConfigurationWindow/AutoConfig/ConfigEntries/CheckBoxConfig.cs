using DailyDuty.Extensions;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace DailyDuty.ConfigurationWindow.AutoConfig.ConfigEntries;

public class CheckBoxConfig : BaseConfigEntry {
    public required bool InitialState { get; set; }

    public override NodeBase BuildNode() {
        return new CheckboxNode {
            OnClick = OnOptionChanged,
            Height = 24.0f,
            String = Label,
            IsChecked = InitialState,
        };
    }

    private void OnOptionChanged(bool newValue) {
        InitialState = newValue;
        MemberInfo.SetValue(Config, newValue);
        Config.Save();
    }
}
