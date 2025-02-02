﻿using Xamarin.UITest;
using Xamarin.UITest.Android;

namespace Toggl.Tests.UI
{
    public static class Configuration
    {
        public static AndroidApp GetApp()
            => ConfigureApp
                .Android
                .InstalledApp("com.toggl.giskard.debug")
                .EnableLocalScreenshots()
                .StartApp();
    }
}
