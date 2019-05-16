using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Web;
using Toggl.Core.Analytics;
using Toggl.Core.DTOs;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Extensions.RxAction;

namespace Toggl.Core.UI.Services
{
    public sealed class UrlHandler : IUrlHandler
    {
        private readonly ITimeService timeService;
        private readonly IInteractorFactory interactorFactory;
        private readonly INavigationService navigationService;
        private readonly IPresenter viewPresenter;

        public UrlHandler(ITimeService timeService, IInteractorFactory interactorFactory, INavigationService navigationService, IPresenter viewPresenter)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(viewPresenter, nameof(viewPresenter));

            this.timeService = timeService;
            this.interactorFactory = interactorFactory;
            this.navigationService = navigationService;
            this.viewPresenter = viewPresenter;
        }

        public async Task<bool> Handle(Uri uri)
        {
            var path = uri.AbsolutePath;
            var args = uri.GetQueryParams();

            switch (path)
            {
                case ApplicationUrls.TimeEntry.Start.Path:
                    return await handleTimeEntryStart(args);
                case ApplicationUrls.TimeEntry.ContinueLast.Path:
                    return await handleContinueLast();
                case ApplicationUrls.TimeEntry.Stop.Path:
                    return await handleTimeEntryStop(args);
                case ApplicationUrls.TimeEntry.Create.Path:
                    return await handleTimeEntryCreate(args);
                case ApplicationUrls.TimeEntry.Update.Path:
                    return await handleTimeEntryUpdate(args);
                case ApplicationUrls.TimeEntry.New.Path:
                    return handleTimeEntryNew(args);
                case ApplicationUrls.TimeEntry.Edit.Path:
                    return handleTimeEntryEdit(args);
                case ApplicationUrls.Reports.Path:
                    return await handleReports(args);
                case ApplicationUrls.Calendar.Path:
                    return await handleCalendar(args);
                default:
                    return false;
            }
        }

        #region Handle paths

        private async Task<bool> handleTimeEntryStart(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/timeEntry/start?workspaceId=1&startTime="2019-04-18T09:35:47Z"&description=Toggl&projectId=1&taskId=1&tags=[1,2,3]&billable=true&source=Siri
            var workspaceId = args.GetValueAsLong(ApplicationUrls.TimeEntry.WorkspaceId);
            var startTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StartTime) ?? timeService.CurrentDateTime;
            var description = args.GetValueAsString(ApplicationUrls.TimeEntry.Description) ?? string.Empty;
            var projectId = args.GetValueAsLong(ApplicationUrls.TimeEntry.ProjectId);
            var taskId = args.GetValueAsLong(ApplicationUrls.TimeEntry.TaskId);
            var tags = args.GetValueAsLongs(ApplicationUrls.TimeEntry.Tags);
            var isBillable = args.GetValueAsBool(ApplicationUrls.TimeEntry.Billable) ?? false;
            var source = args.GetValueAsEnumCase(ApplicationUrls.TimeEntry.Source, TimeEntryStartOrigin.Deeplink);

            if (!workspaceId.HasValue)
            {
                var defaultWorkspace = await interactorFactory.GetDefaultWorkspace().Execute();
                workspaceId = defaultWorkspace.Id;
            }

            var prototype = description.AsTimeEntryPrototype(startTime, workspaceId.Value, null, projectId, taskId, tags, isBillable);
            await interactorFactory.CreateTimeEntry(prototype, source).Execute();

            return true;
        }

        private async Task<bool> handleContinueLast()
        {
            // e.g: toggl://tracker/timeEntry/continue
            await interactorFactory.ContinueMostRecentTimeEntry().Execute();
            return true;
        }

        private async Task<bool> handleTimeEntryStop(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/timeEntry/stop?stopTime="2019-04-18T09:45:47Z"&source=Siri
            var stopTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StopTime) ?? timeService.CurrentDateTime;
            var source = args.GetValueAsEnumCase(ApplicationUrls.TimeEntry.Source, TimeEntryStopOrigin.Deeplink);

            await interactorFactory.StopTimeEntry(stopTime, source).Execute();

            return true;
        }

        private async Task<bool> handleTimeEntryCreate(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/timeEntry/create?workspaceId=1&startTime="2019-04-18T09:35:47Z"&stopTime="2019-04-18T09:45:47Z"&duration=600&description="Toggl"&projectId=1&taskId=1&tags=[]&billable=true&source=Siri
            var workspaceId = args.GetValueAsLong(ApplicationUrls.TimeEntry.WorkspaceId);
            var startTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StartTime);
            var stopTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StopTime);
            var duration = args.GetValueAsTimeSpan(ApplicationUrls.TimeEntry.Duration);
            var description = args.GetValueAsString(ApplicationUrls.TimeEntry.Description);
            var projectId = args.GetValueAsLong(ApplicationUrls.TimeEntry.ProjectId);
            var taskId = args.GetValueAsLong(ApplicationUrls.TimeEntry.TaskId);
            var tags = args.GetValueAsLongs(ApplicationUrls.TimeEntry.Tags);
            var isBillable = args.GetValueAsBool(ApplicationUrls.TimeEntry.Billable) ?? false;
            var source = args.GetValueAsEnumCase(ApplicationUrls.TimeEntry.Source, TimeEntryStartOrigin.Deeplink);

            if (!workspaceId.HasValue)
            {
                var defaultWorkspace = await interactorFactory.GetDefaultWorkspace().Execute();
                workspaceId = defaultWorkspace.Id;
            }

            if (!startTime.HasValue)
                return false;

            if (!duration.HasValue && !stopTime.HasValue)
                return false;

            if (!duration.HasValue)
            {
                duration = stopTime.Value - startTime.Value;
            }

            var prototype = description.AsTimeEntryPrototype(startTime.Value, workspaceId.Value, duration, projectId, taskId, tags, isBillable);
            await interactorFactory.CreateTimeEntry(prototype, source).Execute();

            return true;
        }

        private async Task<bool> handleTimeEntryUpdate(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/timeEntry/update?timeEntryId=1workspaceId=1&startTime="2019-04-18T09:35:47Z"&stopTime="2019-04-18T09:45:47Z"&description="Toggl"&projectId=1&taskId=1&tags=[]&billable=true&source=Siri
            var timeEntryId = args.GetValueAsLong(ApplicationUrls.TimeEntry.TimeEntryId);
            var workspaceId = args.GetValueAsLong(ApplicationUrls.TimeEntry.WorkspaceId);
            var startTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StartTime) ?? timeService.CurrentDateTime;
            var stopTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StopTime);
            var description = args.GetValueAsString(ApplicationUrls.TimeEntry.Description);
            var projectId = args.GetValueAsLong(ApplicationUrls.TimeEntry.ProjectId);
            var taskId = args.GetValueAsLong(ApplicationUrls.TimeEntry.TaskId);
            var tags = args.GetValueAsLongs(ApplicationUrls.TimeEntry.Tags);
            var isBillable = args.GetValueAsBool(ApplicationUrls.TimeEntry.Billable) ?? false;

            if (!timeEntryId.HasValue)
                return false;

            var timeEntryToUpdate = await interactorFactory.GetTimeEntryById(timeEntryId.Value).Execute();

            if (!workspaceId.HasValue)
            {
                var defaultWorkspace = await interactorFactory.GetDefaultWorkspace().Execute();
                workspaceId = defaultWorkspace.Id;
            }

            var newWorkspaceId = args.ContainsKey(ApplicationUrls.TimeEntry.WorkspaceId)
                ? workspaceId.Value
                : timeEntryToUpdate.WorkspaceId;

            var newStartTime = args.ContainsKey(ApplicationUrls.TimeEntry.StartTime)
                ? startTime
                : timeEntryToUpdate.Start;

            var newStopTime = args.ContainsKey(ApplicationUrls.TimeEntry.StopTime)
                ? stopTime
                : timeEntryToUpdate.StopTime();

            var newDescription = args.ContainsKey(ApplicationUrls.TimeEntry.Description)
                ? description
                : timeEntryToUpdate.Description;

            var newProjectId = args.ContainsKey(ApplicationUrls.TimeEntry.ProjectId)
                ? projectId
                : timeEntryToUpdate.ProjectId;

            var newTaskId = args.ContainsKey(ApplicationUrls.TimeEntry.TaskId)
                ? taskId
                : timeEntryToUpdate.TaskId;

            var newTags = args.ContainsKey(ApplicationUrls.TimeEntry.Tags)
                ? tags
                : timeEntryToUpdate.TagIds;

            var newIsBillable = args.ContainsKey(ApplicationUrls.TimeEntry.Billable)
                ? isBillable
                : timeEntryToUpdate.Billable;

            var editTimeEntryDto = new EditTimeEntryDto
            {
                Id = timeEntryId.Value,
                WorkspaceId = newWorkspaceId,
                StartTime = newStartTime,
                StopTime = newStopTime,
                Description = newDescription,
                ProjectId = newProjectId,
                TaskId = newTaskId,
                TagIds = newTags,
                Billable = newIsBillable,
            };

            await interactorFactory.UpdateTimeEntry(editTimeEntryDto).Execute();

            return true;
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

        #endregion

        private Task navigate<TViewModel, TNavigationInput>(TNavigationInput payload)
            where TViewModel : ViewModel<TNavigationInput, Unit>
            => navigationService.Navigate<TViewModel, TNavigationInput, Unit>(payload, null);
    }
}
