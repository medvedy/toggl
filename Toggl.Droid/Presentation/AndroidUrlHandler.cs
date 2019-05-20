using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Toggl.Core;
using Toggl.Core.Analytics;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.Services;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Activities;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;

namespace Toggl.Droid.Presentation
{
    public class AndroidUrlHandler
    {
        private readonly IUrlHandler urlHandler;
        private readonly ITimeService timeService;
        private readonly IInteractorFactory interactorFactory;
        
        public AndroidUrlHandler(ITimeService timeService, IInteractorFactory interactorFactory, IUrlHandler urlHandler)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(urlHandler, nameof(urlHandler));

            this.timeService = timeService;
            this.interactorFactory = interactorFactory;
            this.urlHandler = urlHandler;
        }

        public void HandleUrlForAppStart(string navigationUrl, Activity activity)
        {
            var handled = handle(new Uri(navigationUrl), activity).GetAwaiter().GetResult();
            if (!handled) 
                AndroidStartupHelper.StartMainTabBarActivity(activity);
            
            activity.Finish();
        }
        
        private async Task<bool> handle(Uri uri, Activity activity)
        {
            var path = uri.AbsolutePath;
            var args = uri.GetQueryParams();

            switch (path)
            {
                case ApplicationUrls.TimeEntry.New.Path:
                    return handleTimeEntryNew(args, activity);
                case ApplicationUrls.TimeEntry.Edit.Path:
                    return handleTimeEntryEdit(args, activity);
                case ApplicationUrls.Reports.Path:
                    return await handleReports(args, activity);
                case ApplicationUrls.Calendar.Path:
                    return await handleCalendar(args, activity);
                default:
                    return await urlHandler.Handle(uri)
                        .ContinueWith(_ => AndroidStartupHelper.StartMainTabBarActivity(activity))
                        .ContinueWith(_ => true);
            }
        }

        private bool handleTimeEntryNew(Dictionary<string, string> args, Activity activity)
        {
            // e.g: toggl://tracker/timeEntry/new?startTime="2019-04-18T09:35:47Z"&stopTime="2019-04-18T09:45:47Z"&duration=600&description="Toggl"&projectId=1&tags=[]
            var workspaceId = args.GetValueAsLong(ApplicationUrls.TimeEntry.WorkspaceId);
            var startTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StartTime) ?? timeService.CurrentDateTime;
            var stopTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StopTime);
            var duration = args.GetValueAsTimeSpan(ApplicationUrls.TimeEntry.Duration) ?? startTime - stopTime;
            var description = args.GetValueAsString(ApplicationUrls.TimeEntry.Description);
            var projectId = args.GetValueAsLong(ApplicationUrls.TimeEntry.ProjectId);
            var tags = args.GetValueAsLongs(ApplicationUrls.TimeEntry.Tags);

            var startTimeEntryParameters = new StartTimeEntryParameters(
                startTime,
                string.Empty,
                duration,
                workspaceId,
                description,
                projectId,
                tags
            );
            
            AndroidStartupHelper.ClearSubViewModelsState();

            var viewModelLoader = new ViewModelLoader(AndroidDependencyContainer.Instance);
            var viewModel = viewModelLoader.Load<StartTimeEntryParameters, Unit>(typeof(StartTimeEntryViewModel), startTimeEntryParameters).GetAwaiter().GetResult();
            var vmCache = AndroidDependencyContainer.Instance.ViewModelCache;
            vmCache.Cache(viewModel);

            startActivityWithHierarchy(activity, typeof(StartTimeEntryActivity));
            
            return true;
        }

        private bool handleTimeEntryEdit(Dictionary<string, string> args, Activity activity)
        {
            // e.g: toggl://tracker/timeEntry/edit?timeEntryId=1
            var timeEntryId = args.GetValueAsLong(ApplicationUrls.TimeEntry.TimeEntryId);
            if (timeEntryId.HasValue)
            {
                AndroidStartupHelper.ClearSubViewModelsState();

                var viewModelLoader = new ViewModelLoader(AndroidDependencyContainer.Instance);
                var vmCache = AndroidDependencyContainer.Instance.ViewModelCache;
                var viewModel = viewModelLoader.Load<long[], Unit>(typeof(EditTimeEntryViewModel), new[] { timeEntryId.Value }).GetAwaiter().GetResult();
                vmCache.Cache(viewModel);

                startActivityWithHierarchy(activity, typeof(EditTimeEntryActivity));
                
                return true;
            }

            return false;
        }

        private void startActivityWithHierarchy(Activity activity, Type activityType)
        {
            var intent = new Intent(activity, activityType);
            TaskStackBuilder.Create(activity)
                .AddNextIntent(new Intent(activity, typeof(MainTabBarActivity)).AddFlags(ActivityFlags.SingleTop))
                .AddNextIntent(intent)
                .StartActivities();
        }

        private async Task<bool> handleReports(Dictionary<string, string> args, Activity activity)
        {
            // e.g: toggl://tracker/reports?workspaceId=1&startDate="2019-04-18T09:35:47Z"&endDate="2019-04-18T09:45:47Z"
            var workspaceId = args.GetValueAsLong(ApplicationUrls.Reports.WorkspaceId);
            var startDate = args.GetValueAsDateTimeOffset(ApplicationUrls.Reports.StartDate);
            var endDate = args.GetValueAsDateTimeOffset(ApplicationUrls.Reports.EndDate);

            navigateToReports(activity, workspaceId, startDate, endDate);
            return true;
        }

        private async Task<bool> handleCalendar(Dictionary<string, string> args, Activity activity)
        {
            // e.g: toggl://tracker/calendar?eventId=1
            var calendarItemId = args.GetValueAsString(ApplicationUrls.Calendar.EventId);

            if (calendarItemId == null)
            {
                navigateToCalendar(activity);
                return true;
            }

            var calendarEvent = await interactorFactory.GetCalendarItemWithId(calendarItemId).Execute();
            var defaultWorkspace = await interactorFactory.GetDefaultWorkspace().Execute();

            await interactorFactory
                .CreateTimeEntry(calendarEvent.AsTimeEntryPrototype(defaultWorkspace.Id), TimeEntryStartOrigin.CalendarEvent)
                .Execute();

            return true;
        }
        
        private void navigateToReports(Activity activity, long? workspaceId, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var bundle = Bundle.Empty;
            bundle.PutInt(MainTabBarActivity.StartingTabExtra, Resource.Id.MainTabReportsItem);
            bundle.PutLong(MainTabBarActivity.WorkspaceIdExtra, workspaceId ?? 0L);
            bundle.PutLong(MainTabBarActivity.StartDateExtra, startDate?.ToUnixTimeSeconds() ?? 0L);
            bundle.PutLong(MainTabBarActivity.EndDateExtra, endDate?.ToUnixTimeSeconds() ?? 0L);
            navigateToMainTabBarWithExtras(activity, bundle);
        }

        private void navigateToCalendar(Activity activity)
        {
            var bundle = Bundle.Empty;
            bundle.PutInt(MainTabBarActivity.StartingTabExtra, Resource.Id.MainTabCalendarItem);
            navigateToMainTabBarWithExtras(activity, bundle);
        }
        
        private void navigateToMainTabBarWithExtras(Activity activity, Bundle extras)
        {
            AndroidStartupHelper.ClearSubViewModelsState();

            var intent = new Intent(activity, typeof(MainTabBarActivity))
                .AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop)
                .PutExtras(extras);
            
            activity.StartActivity(intent);
        }
    }
}
