using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers.Settings.Models
{
    public interface ISettingRow
    {
        string Title { get; }
        UIAction Action { get; }
    }
}
