using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers.Settings.Models
{
    public class ToggleRow : ISettingRow
    {
        public string Title { get; }
        public UIAction Action { get; }

        public bool Value { get; }

        public ToggleRow(string title, bool value, UIAction action)
        {
            Title = title;
            Value = value;
            Action = action;
        }
    }
}
