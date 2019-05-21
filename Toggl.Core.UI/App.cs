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

        public App(UIDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        public App<TFirstViewModelWhenNotLoggedIn, TInput> Initialize()
        {
            revokeNewUserIfNeeded();
            dependencyContainer.BackgroundSyncService
                .SetupBackgroundSync(dependencyContainer.UserAccessManager);

            dependencyContainer.OnboardingStorage
                .SetFirstOpened(dependencyContainer.TimeService.CurrentDateTime);

            return this;
        }

        public async Task<bool> NavigateWhenUserDoesNotHaveFullAppAccess()
        {
            var navigationService = dependencyContainer.NavigationService;
            var accessRestrictionStorage = dependencyContainer.AccessRestrictionStorage;

            if (accessRestrictionStorage.IsApiOutdated() || accessRestrictionStorage.IsClientOutdated())
            {
                navigationService.Navigate<OutdatedAppViewModel>(null);
                return false;
            }

            if (!dependencyContainer.UserAccessManager.CheckIfLoggedIn())
            {
                navigationService.Navigate<TFirstViewModelWhenNotLoggedIn, TInput>(new TInput(), null);
                return false;
            }

            var user = await dependencyContainer.InteractorFactory.GetCurrentUser().Execute();
            if (accessRestrictionStorage.IsUnauthorized(user.ApiToken))
            {
                navigationService.Navigate<TokenResetViewModel>(null);
                return false;
            }

            return true;
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
