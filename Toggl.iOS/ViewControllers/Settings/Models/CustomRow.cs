using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers.Settings.Models
{
    public class CustomRow<TCustomValue> : ISettingRow
    {
        public string Title { get; }
        public UIAction Action { get; }
        public TCustomValue CustomValue { get; }

        public CustomRow(TCustomValue customValue, UIAction action = null)
        {
            CustomValue = customValue;
            Action = action;
        }
    }
}
