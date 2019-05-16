using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI
{
    public sealed class App<TFirstViewModelWhenNotLoggedIn, TInput>
        where TFirstViewModelWhenNotLoggedIn : ViewModel<TInput, Unit>
        where TInput : new()
    {
        private readonly UIDependencyContainer dependencyContainer;
        private readonly ITimeService timeService;
        private readonly INavigationService navigationService;
        private readonly IOnboardingStorage onboardingStorage;
        private readonly IAccessRestrictionStorage accessRestrictionStorage;

        public App(UIDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
            timeService = dependencyContainer.TimeService;
            navigationService = dependencyContainer.NavigationService;
            onboardingStorage = dependencyContainer.OnboardingStorage;
            accessRestrictionStorage = dependencyContainer.AccessRestrictionStorage;
        }

        public async Task Start()
        {
            await processCommonStart();
            await navigationService.Navigate<MainTabBarViewModel>(null);
        }

        public async Task StartWithNavigationUrl(Uri navigationUrl)
        {
            await processCommonStart();

            var urlHandler = dependencyContainer.UrlHandler;
            var urlWasHandled = await urlHandler.Handle(navigationUrl);
            if (urlWasHandled) return;

            await navigationService.Navigate<MainTabBarViewModel>(null);
        }

        private async Task processCommonStart()
        {
            revokeNewUserIfNeeded();

            dependencyContainer.BackgroundSyncService.SetupBackgroundSync(dependencyContainer.UserAccessManager);

            onboardingStorage.SetFirstOpened(timeService.CurrentDateTime);

            if (accessRestrictionStorage.IsApiOutdated() || accessRestrictionStorage.IsClientOutdated())
            {
                await navigationService.Navigate<OutdatedAppViewModel>(null);
                return;
            }

            if (!dependencyContainer.UserAccessManager.CheckIfLoggedIn())
            {
                await navigationService.Navigate<TFirstViewModelWhenNotLoggedIn, TInput>(new TInput(), null);
                return;
            }

            var user = await dependencyContainer.InteractorFactory.GetCurrentUser().Execute();
            if (accessRestrictionStorage.IsUnauthorized(user.ApiToken))
            {
                await navigationService.Navigate<TokenResetViewModel>(null);
                return;
            }

            dependencyContainer.SyncManager.ForceFullSync().Subscribe();
        }

        private void revokeNewUserIfNeeded()
        {
            const int newUserThreshold = 60;
            var now = dependencyContainer.TimeService.CurrentDateTime;
            var lastUsed = dependencyContainer.OnboardingStorage.GetLastOpened();
            dependencyContainer.OnboardingStorage.SetLastOpened(now);
            if (!lastUsed.HasValue)
                return;

            var offset = now - lastUsed;
            if (offset < TimeSpan.FromDays(newUserThreshold))
                return;

            dependencyContainer.OnboardingStorage.SetIsNewUser(false);
        }
    }
}