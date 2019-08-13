using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers.Settings.Models
{
    public class NavigationRow : ISettingRow
    {
        public string Title { get; }
        public string Detail { get; }
        public UIAction Action { get; }

        public NavigationRow(string title, string detail, UIAction action = null)
        {
            Title = title;
            Action = action;
            Detail = detail;
        }

        public NavigationRow(string title, UIAction action = null)
            : this(title, null, action)
        {
        }
    }
}
