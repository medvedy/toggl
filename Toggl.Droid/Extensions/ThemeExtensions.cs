using Toggl.Droid.Helper;
using Toggl.Storage.Settings;
using static Android.Support.V7.App.AppCompatDelegate;

namespace Toggl.Droid.Extensions
{
    public static class ThemeExtensions
    {
        public static int NightModeFlag(this Theme theme)
        {
            switch (theme)
            {
                case Theme.SystemDefault:
                    return QApis.AreAvailable ? ModeNightFollowSystem : ModeNightAuto;
                case Theme.Light:
                    return ModeNightNo;
                case Theme.Dark:
                    return ModeNightYes;
                default:
                    return ModeNightFollowSystem;
            }
        }

        public static string Name(this Theme theme)
        {
            switch (theme)
            {
                case Theme.SystemDefault:
                    return Shared.Resources.SystemDefault;
                case Theme.Light:
                    return Shared.Resources.Light;
                case Theme.Dark:
                    return Shared.Resources.Dark;
                default:
                    return Shared.Resources.SystemDefault;
            }
        }
    }
}
