﻿using System;
using System.Reactive.Linq;
using MvvmCross.Plugin;
using MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross
{
    public sealed class App<TFirstViewModelWhenNotLoggedIn> : MvxApplication
        where TFirstViewModelWhenNotLoggedIn : MvxViewModel
    {
        private readonly UiDependencyContainer dependencyContainer;

        public App()
        {
        }

        public App(UiDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }
        
        public override void Initialize()
        {
            var appStart = new AppStart<TFirstViewModelWhenNotLoggedIn>(this, dependencyContainer);
            RegisterAppStart(appStart);
        }

        public override void LoadPlugins(IMvxPluginManager pluginManager)
        {
        }

        protected override IMvxViewModelLocator CreateDefaultViewModelLocator()
            => new TogglViewModelLocator(dependencyContainer);
    }

    [Preserve(AllMembers = true)]
    public sealed class AppStart<TFirstViewModelWhenNotLoggedIn> : MvxAppStart
        where TFirstViewModelWhenNotLoggedIn : MvxViewModel
    {
        private readonly UiDependencyContainer dependencyContainer;

        public AppStart(IMvxApplication app, UiDependencyContainer dependencyContainer)
            : base (app, dependencyContainer.NavigationService)
        {
            this.dependencyContainer = dependencyContainer;
        }

        protected override async void NavigateToFirstViewModel(object hint = null)
        {
            var timeService = dependencyContainer.TimeService;
            var navigationService = dependencyContainer.NavigationService;
            var onboardingStorage = dependencyContainer.OnboardingStorage;
            var accessRestrictionStorage = dependencyContainer.AccessRestrictionStorage;

            onboardingStorage.SetFirstOpened(timeService.CurrentDateTime);

            if (accessRestrictionStorage.IsApiOutdated() || accessRestrictionStorage.IsClientOutdated())
            {
                await navigationService.Navigate<OutdatedAppViewModel>();
                return;
            }

            if (!dependencyContainer.UserAccessManager.CheckIfLoggedIn())
            {
                await navigationService.Navigate<TFirstViewModelWhenNotLoggedIn>();
                return;
            }
            
            var user = await dependencyContainer.InteractorFactory.GetCurrentUser().Execute();
            if (accessRestrictionStorage.IsUnauthorized(user.ApiToken))
            {
                await navigationService.Navigate<TokenResetViewModel>();
                return;
            }

            dependencyContainer.SyncManager.ForceFullSync().Subscribe();

            await navigationService.Navigate<MainTabBarViewModel>();
        }
    }
}
