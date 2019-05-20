using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core;
using Toggl.Core.Analytics;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.Services;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Extensions.RxAction;

namespace Toggl.Droid.Presentation
{
    public class AndroidUrlHandler : IUrlHandler
    {
        private readonly IUrlHandler urlHandler;
        private readonly ITimeService timeService;
        private readonly IInteractorFactory interactorFactory;
        private readonly INavigationService navigationService;
        private readonly IPresenter viewPresenter;

        public AndroidUrlHandler(ITimeService timeService, IInteractorFactory interactorFactory, INavigationService navigationService, IPresenter viewPresenter)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(viewPresenter, nameof(viewPresenter));
            
            this.timeService = timeService;
            this.interactorFactory = interactorFactory;
            this.navigationService = navigationService;
            this.viewPresenter = viewPresenter;
            urlHandler = new UrlHandler(timeService, interactorFactory, navigationService, viewPresenter);
        }
        
        public async Task<bool> Handle(Uri uri)
        {
            var path = uri.AbsolutePath;
            var args = uri.GetQueryParams();

            switch (path)
            {
                case ApplicationUrls.TimeEntry.New.Path:
                    return handleTimeEntryNew(args);
                case ApplicationUrls.TimeEntry.Edit.Path:
                    return handleTimeEntryEdit(args);
                case ApplicationUrls.Reports.Path:
                    return await handleReports(args);
                case ApplicationUrls.Calendar.Path:
                    return await handleCalendar(args);
                default:
                    return await urlHandler.Handle(uri)
                        .ContinueWith(_ => navigationService.Navigate<MainTabBarViewModel>(null))
                        .ContinueWith(_ => true);
            }
        }

        private bool handleTimeEntryNew(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/timeEntry/new?workspaceId=1&startTime="2019-04-18T09:35:47Z"&stopTime="2019-04-18T09:45:47Z"&duration=600&description="Toggl"&projectId=1&tags=[]
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
            navigate<StartTimeEntryViewModel, StartTimeEntryParameters>(startTimeEntryParameters);
            return true;
        }

        internal void HandleUrlForAppStart(string navigationUrl, Activity splashScreen)
        {
            splashScreen.Finish();
        }

        private bool handleTimeEntryEdit(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/timeEntry/edit?timeEntryId=1
            var timeEntryId = args.GetValueAsLong(ApplicationUrls.TimeEntry.TimeEntryId);
            if (timeEntryId.HasValue)
            {
                navigate<EditTimeEntryViewModel, long[]>(new[] { timeEntryId.Value });
                return true;
            }

            return false;
        }

        private async Task<bool> handleReports(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/reports?workspaceId=1&startDate="2019-04-18T09:35:47Z"&endDate="2019-04-18T09:45:47Z"
            var workspaceId = args.GetValueAsLong(ApplicationUrls.Reports.WorkspaceId);
            var startDate = args.GetValueAsDateTimeOffset(ApplicationUrls.Reports.StartDate);
            var endDate = args.GetValueAsDateTimeOffset(ApplicationUrls.Reports.EndDate);

            var change = new ShowReportsPresentationChange(workspaceId, startDate, endDate);
            viewPresenter.ChangePresentation(change);
            return true;
        }

        private async Task<bool> handleCalendar(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/calendar?eventId=1
            var calendarItemId = args.GetValueAsString(ApplicationUrls.Calendar.EventId);

            if (calendarItemId == null)
            {
                var change = new ShowCalendarPresentationChange();
                viewPresenter.ChangePresentation(change);
                return true;
            }

            var calendarEvent = await interactorFactory.GetCalendarItemWithId(calendarItemId).Execute();

            var defaultWorkspace = await interactorFactory.GetDefaultWorkspace().Execute();

            await interactorFactory
                .CreateTimeEntry(calendarEvent.AsTimeEntryPrototype(defaultWorkspace.Id), TimeEntryStartOrigin.CalendarEvent)
                .Execute();

            return true;
        }
        
        private Task navigate<TViewModel, TNavigationInput>(TNavigationInput payload)
            where TViewModel : ViewModel<TNavigationInput, Unit>
            => navigationService.Navigate<TViewModel, TNavigationInput, Unit>(payload, null);
    }
}
