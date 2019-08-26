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
                case Theme.UseSystem:
                    return QApis.AreAvailable ? ModeNightFollowSystem : ModeNightAuto;
                case Theme.Light:
                    return ModeNightNo;
                case Theme.Dark:
                    return ModeNightYes;
                default:
                    return DefaultNightMode;
            }
        }

        public static string Name(this Theme theme)
        {
            switch (theme)
            {
                case Theme.UseSystem:
                    return Shared.Resources.UseSystemDefault;
                case Theme.Light:
                    return Shared.Resources.LightTheme;
                case Theme.Dark:
                    return Shared.Resources.DarkTheme;
                default:
                    return Shared.Resources.UseSystemDefault;
            }
        }
    }
}
