using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.Views;
using Toggl.Droid.Activities;
using Fragment = Android.Support.V4.App.Fragment;

namespace Toggl.Droid.Presentation
{
    public sealed class ActivityPresenter : AndroidPresenter
    {
        private const ActivityFlags clearBackStackFlags = ActivityFlags.ClearTop | ActivityFlags.SingleTop;
        private const ActivityFlags startNewTaskFlags = ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask;

        protected override HashSet<Type> AcceptedViewModels { get; } = new HashSet<Type>
        {
            typeof(AboutViewModel),
            typeof(BrowserViewModel),
            typeof(CalendarSettingsViewModel),
            typeof(EditDurationViewModel),
            typeof(EditProjectViewModel),
            typeof(EditTimeEntryViewModel),
            typeof(ForgotPasswordViewModel),
            typeof(LoginViewModel),
            typeof(MainTabBarViewModel),
            typeof(OutdatedAppViewModel),
            typeof(SelectClientViewModel),
            typeof(SelectCountryViewModel),
            typeof(SelectProjectViewModel),
            typeof(SelectTagsViewModel),
            typeof(SendFeedbackViewModel),
            typeof(SignupViewModel),
            typeof(StartTimeEntryViewModel),
            typeof(TokenResetViewModel)
        };

        private readonly Dictionary<Type, ActivityPresenterInfo> presentableActivitiesInfos = new Dictionary<Type,ActivityPresenterInfo>
        {
            [typeof(AboutViewModel)] = new ActivityPresenterInfo(typeof(AboutActivity)),
            [typeof(BrowserViewModel)] = new ActivityPresenterInfo(typeof(BrowserActivity)),
            [typeof(CalendarSettingsViewModel)] = new ActivityPresenterInfo(typeof(CalendarSettingsActivity)),
            [typeof(EditDurationViewModel)] = new ActivityPresenterInfo(typeof(EditDurationActivity)),
            [typeof(EditProjectViewModel)] = new ActivityPresenterInfo(typeof(EditProjectActivity)),
            [typeof(EditTimeEntryViewModel)] = new ActivityPresenterInfo(typeof(EditTimeEntryActivity)),
            [typeof(ForgotPasswordViewModel)] = new ActivityPresenterInfo(typeof(ForgotPasswordActivity)),
            [typeof(LoginViewModel)] = new ActivityPresenterInfo(typeof(LoginActivity), startNewTaskFlags),
            [typeof(MainTabBarViewModel)] = new ActivityPresenterInfo(typeof(MainTabBarActivity), clearBackStackFlags),
            [typeof(OutdatedAppViewModel)] = new ActivityPresenterInfo(typeof(OutdatedAppActivity), startNewTaskFlags),
            [typeof(SelectClientViewModel)] = new ActivityPresenterInfo(typeof(SelectClientActivity)),
            [typeof(SelectCountryViewModel)] = new ActivityPresenterInfo(typeof(SelectCountryActivity)),
            [typeof(SelectProjectViewModel)] = new ActivityPresenterInfo(typeof(SelectProjectActivity)),
            [typeof(SelectTagsViewModel)] = new ActivityPresenterInfo(typeof(SelectTagsActivity)),
            [typeof(SendFeedbackViewModel)] = new ActivityPresenterInfo(typeof(SendFeedbackActivity)),
            [typeof(SignupViewModel)] = new ActivityPresenterInfo(typeof(SignUpActivity), startNewTaskFlags),
            [typeof(StartTimeEntryViewModel)] = new ActivityPresenterInfo(typeof(StartTimeEntryActivity)),
            [typeof(TokenResetViewModel)] = new ActivityPresenterInfo(typeof(TokenResetActivity), startNewTaskFlags)
        };

        protected override void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            var viewModelType = viewModel.GetType();

            if (!presentableActivitiesInfos.TryGetValue(viewModelType, out var presentableInfo))
                throw new Exception($"Failed to start Activity for viewModel with type {viewModelType.Name}");
            
            var intent = new Intent(getContextFromView(sourceView), presentableInfo.ActivityType).AddFlags(presentableInfo.Flags);

            if (presentationRequiresViewModelCleaning(presentableInfo.Flags))
            {
                AndroidDependencyContainer.Instance.ViewModelCache.ClearAll();
            }

            AndroidDependencyContainer
                .Instance
                .ViewModelCache
                .Cache(viewModel);

            getContextFromView(sourceView).StartActivity(intent);
        }

        private bool presentationRequiresViewModelCleaning(ActivityFlags activityFlags)
        {
            return activityFlags == clearBackStackFlags
                   || activityFlags == startNewTaskFlags;
        }

        private Context getContextFromView(IView view)
        {
            if (view is Activity activity)
                return activity;

            if (view is Fragment fragment)
                return fragment.Activity;

            return Application.Context;
        }

        private struct ActivityPresenterInfo
        {
            public Type ActivityType { get; }
            public ActivityFlags Flags { get; }

            public ActivityPresenterInfo(Type activityType, ActivityFlags flags = 0)
            {
                ActivityType = activityType;
                Flags = flags;
            }
        }
    }
}
