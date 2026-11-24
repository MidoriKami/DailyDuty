using KamiToolKit.Controls.Builders;

namespace DailyDuty.Interfaces;

/// <summary>
/// Interface representing a feature that has a native UI configuration element.
/// </summary>
public interface IConfigurable {

    /// <summary>
    /// Builds the UI nodes used to configure this component.
    /// </summary>
    void BuildConfigUi(NodeBuilder builder);
}
