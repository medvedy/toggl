﻿using System;
using System.Threading.Tasks;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Core.UI.ViewModels.Settings;

namespace Toggl.Core.UI.Navigation
{
    public sealed class ViewModelLoader
    {
        private readonly UIDependencyContainer dependencyContainer;

        public ViewModelLoader(UIDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        public async Task<ViewModel<TInput, TOutput>> Load<TInput, TOutput>(Type viewModelType, TInput payload)
        {
            var viewModel = (ViewModel<TInput, TOutput>)findViewModel(viewModelType);
            await viewModel.Initialize(payload);
            return viewModel;
        }

        private IViewModel findViewModel(Type viewModelType)
        {
            if (viewModelType == typeof(BrowserViewModel))
                return new BrowserViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(EditDurationViewModel))
                return new EditDurationViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.TimeService,
                    dependencyContainer.DataSource,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.SchedulerProvider);

            if (viewModelType == typeof(EditProjectViewModel))
                return new EditProjectViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.StopwatchProvider,
                    dependencyContainer.NavigationService);

            if (viewModelType == typeof(EditTimeEntryViewModel))
                return new EditTimeEntryViewModel(
                    dependencyContainer.TimeService,
                    dependencyContainer.DataSource,
                    dependencyContainer.SyncManager,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.StopwatchProvider,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.SchedulerProvider);

            if (viewModelType == typeof(ForgotPasswordViewModel))
                return new ForgotPasswordViewModel(
                    dependencyContainer.TimeService,
                    dependencyContainer.UserAccessManager,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(LoginViewModel))
                return new LoginViewModel(
                    dependencyContainer.UserAccessManager,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.NavigationService,
                    dependencyContainer.ErrorHandlingService,
                    dependencyContainer.LastTimeUsageStorage,
                    dependencyContainer.TimeService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(MainTabBarViewModel))
                return new MainTabBarViewModel(
                    dependencyContainer.TimeService,
                    dependencyContainer.DataSource,
                    dependencyContainer.SyncManager,
                    dependencyContainer.RatingService,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.BackgroundService,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.PermissionsChecker,
                    dependencyContainer.NavigationService,
                    dependencyContainer.RemoteConfigService,
                    dependencyContainer.SuggestionProviderContainer,
                    dependencyContainer.IntentDonationService,
                    dependencyContainer.AccessRestrictionStorage,
                    dependencyContainer.StopwatchProvider,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.UserAccessManager,
                    dependencyContainer.PrivateSharedStorageService,
                    dependencyContainer.PlatformInfo);

            if (viewModelType == typeof(MainViewModel))
                return new MainViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.SyncManager,
                    dependencyContainer.TimeService,
                    dependencyContainer.RatingService,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.RemoteConfigService,
                    dependencyContainer.SuggestionProviderContainer,
                    dependencyContainer.IntentDonationService,
                    dependencyContainer.AccessRestrictionStorage,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.StopwatchProvider,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(NoWorkspaceViewModel))
                return new NoWorkspaceViewModel(
                    dependencyContainer.SyncManager,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.AccessRestrictionStorage,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(OnboardingViewModel))
                return new OnboardingViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.SchedulerProvider);

            if (viewModelType == typeof(OutdatedAppViewModel))
                return new OutdatedAppViewModel(
                    dependencyContainer.BrowserService,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.NavigationService);

            if (viewModelType == typeof(RatingViewModel))
                return new RatingViewModel(
                    dependencyContainer.TimeService,
                    dependencyContainer.DataSource,
                    dependencyContainer.RatingService,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.NavigationService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(ReportsCalendarViewModel))
                return new ReportsCalendarViewModel(
                    dependencyContainer.TimeService,
                    dependencyContainer.DataSource,
                    dependencyContainer.IntentDonationService,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.NavigationService);

            if (viewModelType == typeof(SelectBeginningOfWeekViewModel))
                return new SelectBeginningOfWeekViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(SelectClientViewModel))
                return new SelectClientViewModel(
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(SelectColorViewModel))
                return new SelectColorViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(SelectCountryViewModel))
                return new SelectCountryViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(SelectDateFormatViewModel))
                return new SelectDateFormatViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(SelectDateTimeViewModel))
                return new SelectDateTimeViewModel(
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.NavigationService);

            if (viewModelType == typeof(SelectDefaultWorkspaceViewModel))
                return new SelectDefaultWorkspaceViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.AccessRestrictionStorage,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(SelectDurationFormatViewModel))
                return new SelectDurationFormatViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(SelectProjectViewModel))
                return new SelectProjectViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.StopwatchProvider);

            if (viewModelType == typeof(SelectTagsViewModel))
                return new SelectTagsViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.StopwatchProvider,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(SelectWorkspaceViewModel))
                return new SelectWorkspaceViewModel(
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(SignupViewModel))
                return new SignupViewModel(
                    dependencyContainer.ApiFactory,
                    dependencyContainer.UserAccessManager,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.NavigationService,
                    dependencyContainer.ErrorHandlingService,
                    dependencyContainer.LastTimeUsageStorage,
                    dependencyContainer.TimeService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.PlatformInfo);

            if (viewModelType == typeof(StartTimeEntryViewModel))
                return new StartTimeEntryViewModel(
                    dependencyContainer.TimeService,
                    dependencyContainer.DataSource,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.IntentDonationService,
                    dependencyContainer.StopwatchProvider,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(SuggestionsViewModel))
                return new SuggestionsViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.SuggestionProviderContainer,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.NavigationService);

            if (viewModelType == typeof(SyncFailuresViewModel))
                return new SyncFailuresViewModel(
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService);

            if (viewModelType == typeof(TermsOfServiceViewModel))
                return new TermsOfServiceViewModel(
                    dependencyContainer.BrowserService,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.NavigationService);

            if (viewModelType == typeof(TokenResetViewModel))
                return new TokenResetViewModel(
                    dependencyContainer.UserAccessManager,
                    dependencyContainer.DataSource,
                    dependencyContainer.NavigationService,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.InteractorFactory);

            if (viewModelType == typeof(CalendarPermissionDeniedViewModel))
                return new CalendarPermissionDeniedViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.PermissionsChecker,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(CalendarViewModel))
                return new CalendarViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.TimeService,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.BackgroundService,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.PermissionsChecker,
                    dependencyContainer.NavigationService,
                    dependencyContainer.StopwatchProvider,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(SelectUserCalendarsViewModel))
                return new SelectUserCalendarsViewModel(
                    dependencyContainer.UserPreferences,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(ReportsViewModel))
                return new ReportsViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.TimeService,
                    dependencyContainer.NavigationService,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.IntentDonationService,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.StopwatchProvider,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(AboutViewModel))
                return new AboutViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(CalendarSettingsViewModel))
                return new CalendarSettingsViewModel(
                    dependencyContainer.UserPreferences,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.NavigationService,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.PermissionsChecker);

            if (viewModelType == typeof(LicensesViewModel))
                return new LicensesViewModel(
                    dependencyContainer.LicenseProvider,
                    dependencyContainer.NavigationService);

            if (viewModelType == typeof(NotificationSettingsViewModel))
                return new NotificationSettingsViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.BackgroundService,
                    dependencyContainer.PermissionsChecker,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(SendFeedbackViewModel))
                return new SendFeedbackViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.SchedulerProvider,
                    dependencyContainer.RxActionFactory);

            if (viewModelType == typeof(SettingsViewModel))
                return new SettingsViewModel(
                    dependencyContainer.DataSource,
                    dependencyContainer.SyncManager,
                    dependencyContainer.PlatformInfo,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.AnalyticsService,
                    dependencyContainer.UserAccessManager,
                    dependencyContainer.InteractorFactory,
                    dependencyContainer.OnboardingStorage,
                    dependencyContainer.NavigationService,
                    dependencyContainer.PrivateSharedStorageService,
                    dependencyContainer.IntentDonationService,
                    dependencyContainer.StopwatchProvider,
                    dependencyContainer.RxActionFactory,
                    dependencyContainer.PermissionsChecker,
                    dependencyContainer.SchedulerProvider);

            if (viewModelType == typeof(UpcomingEventsNotificationSettingsViewModel))
                return new UpcomingEventsNotificationSettingsViewModel(
                    dependencyContainer.NavigationService,
                    dependencyContainer.UserPreferences,
                    dependencyContainer.RxActionFactory);


            throw new InvalidOperationException($"Trying to locate ViewModel {viewModelType.Name} failed.");
        }
    }
}
