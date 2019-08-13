using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers.Settings.Models
{
    public class ButtonRow : ISettingRow
    {
        public string Title { get; }
        public UIAction Action { get; }

        public ButtonRow(string title, UIAction action)
        {
            Title = title;
            Action = action;
        }
    }
}
