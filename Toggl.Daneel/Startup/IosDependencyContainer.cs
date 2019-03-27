﻿using Foundation;
using MvvmCross.Navigation;
using MvvmCross.Platforms.Ios.Presenters;
using System;
using Toggl.Daneel.Presentation;
using Toggl.Daneel.Services;
using Toggl.Foundation;
using Toggl.Foundation.Analytics;
using Toggl.Foundation.Diagnostics;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.Services;
using Toggl.Foundation.Shortcuts;
using Toggl.Foundation.Suggestions;
using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Realm;
using Toggl.PrimeRadiant.Settings;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;

namespace Toggl.Daneel
{
    public sealed class IosDependencyContainer : UiDependencyContainer
    {
        private const int numberOfSuggestions = 3;
        private const string clientName = "Daneel";
        private const string remoteConfigDefaultsFileName = "RemoteConfigDefaults";
        private const ApiEnvironment environment =
#if USE_PRODUCTION_API
            ApiEnvironment.Production;
#else
            ApiEnvironment.Staging;
#endif
        
        private readonly Lazy<SettingsStorage> settingsStorage;

        public IMvxNavigationService ForkingNavigationService { get; internal set; }

        public TogglPresenter ViewPresenter { get; }

        public IosDependencyContainer(TogglPresenter viewPresenter, string version)
            : base(environment, new UserAgent(clientName, version))
        {
            ViewPresenter = viewPresenter;
            
            var appVersion = Version.Parse(version);
            
            settingsStorage = new Lazy<SettingsStorage>(() => new SettingsStorage(appVersion, KeyValueStorage));
        }

        public static IosDependencyContainer Instance { get; set; }

        protected override IAnalyticsService CreateAnalyticsService()
            => new AnalyticsServiceIos();

        protected override IBackgroundSyncService CreateBackgroundSyncService()
            => new BackgroundSyncServiceIos();

        protected override IBrowserService CreateBrowserService()
            => new BrowserServiceIos();

        protected override ICalendarService CreateCalendarService()
            => new CalendarServiceIos(PermissionsService);

        protected override ITogglDatabase CreateDatabase()
            => new Database();

        protected override IDialogService CreateDialogService()
            => new DialogServiceIos(ViewPresenter);

        protected override IGoogleService CreateGoogleService()
            => new GoogleServiceIos();

        protected override IIntentDonationService CreateIntentDonationService()
            => new IntentDonationServiceIos(AnalyticsService);

        protected override IKeyValueStorage CreateKeyValueStorage()
            => new UserDefaultsStorageIos();

        protected override ILicenseProvider CreateLicenseProvider()
            => new LicenseProviderIos();

        protected override INotificationService CreateNotificationService()
            => new NotificationServiceIos(PermissionsService, TimeService);

        protected override IPasswordManagerService CreatePasswordManagerService()
            => new OnePasswordServiceIos();

        protected override IPermissionsService CreatePermissionsService()
            => new PermissionsServiceIos();

        protected override IPlatformInfo CreatePlatformInfo()
            => new PlatformInfoIos();

        protected override IPrivateSharedStorageService CreatePrivateSharedStorageService()
            => new PrivateSharedStorageServiceIos();

        protected override IRatingService CreateRatingService()
            => new RatingServiceIos();

        protected override IRemoteConfigService CreateRemoteConfigService()
            => new RemoteConfigServiceIos(remoteConfigDefaultsFileName);

        protected override ISchedulerProvider CreateSchedulerProvider()
            => new IOSSchedulerProvider();

        protected override IApplicationShortcutCreator CreateShortcutCreator()
            => new ApplicationShortcutCreator();

        protected override IStopwatchProvider CreateStopwatchProvider()
            => new FirebaseStopwatchProviderIos();

        protected override ISuggestionProviderContainer CreateSuggestionProviderContainer()
            => new SuggestionProviderContainer(
                new MostUsedTimeEntrySuggestionProvider(Database, TimeService, numberOfSuggestions)
            );

        protected override IMvxNavigationService CreateNavigationService()
            => ForkingNavigationService;

        protected override ILastTimeUsageStorage CreateLastTimeUsageStorage()
            => settingsStorage.Value;

        protected override IOnboardingStorage CreateOnboardingStorage()
            => settingsStorage.Value;

        protected override IUserPreferences CreateUserPreferences()
            => settingsStorage.Value;

        protected override IAccessRestrictionStorage CreateAccessRestrictionStorage()
            => settingsStorage.Value;
    }
}