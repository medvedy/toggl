using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Tests.UI.Exceptions;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Toggl.Tests.UI.Extensions
{
    public static partial class EditTimeEntryExtensions
    {
        public static void AssertEditViewIsVisible(
            this IApp app,
            string description = null,
            string projectName = null,
            string[] tags = null,
            string startTime = null,
            string stopTime = null,
            bool? isStopped = null,
            string duration = null,
            string startDate = null,
            bool? isBillable = null)
        {
            app.WaitForElement(EditTimeEntry.View);

            List<string> wrongFields = new List<string>();
            if (description != null && !app.DescriptionIsCorrect(description))
                wrongFields.Add("description");

            if (projectName != null && !app.ProjectNameIsCorrect(projectName))
                wrongFields.Add("project name");

            if (tags != null && !app.TagsAreCorrect(tags))
                wrongFields.Add("tags");

            if (startTime != null && !app.StartTimeIsCorrect(startTime))
                wrongFields.Add("start time");

            if (stopTime != null && !app.StopTimeIsCorrect(startTime))
                wrongFields.Add("stop time");

            if (isStopped != null && !app.StoppedStateIsCorrect((bool)isStopped))
                wrongFields.Add("stopped state");

            if (duration != null && !app.DurationIsCorrect(duration))
                wrongFields.Add("duration");

            if (startDate != null && !app.StartDateIsCorrect(startDate))
                wrongFields.Add("start date");

            if (isBillable != null && !app.BillableStateIsCorrect((bool)isBillable))
                wrongFields.Add("billable state");

            if (wrongFields.Any())
                throw new EditViewAssertionException($"Edit view is open, but {string.Join(", ", wrongFields)} {(wrongFields.Count > 1 ? "are" : "is")} wrong.");
        }

        public static bool DescriptionIsCorrect(this IApp app, string description)
            => app.Query(x => x.Marked(EditTimeEntry.EditDescription).Text(description)).Any();

        public static bool ProjectNameIsCorrect(this IApp app, string projectName)
            => app.Query(x => x.Marked(EditTimeEntry.EditProject).Descendant().Contains(projectName)).Any();

        public static bool StartTimeIsCorrect(this IApp app, string startTime)
            => app.Query(x => x.Marked(EditTimeEntry.StartTime).Text(startTime)).Any();

        public static bool StopTimeIsCorrect(this IApp app, string stopTime)
            => app.Query(x => x.Marked(EditTimeEntry.StopTime).Text(stopTime)).Any();

        public static bool StoppedStateIsCorrect(this IApp app, bool isStopped)
            => isStopped == app.Query(x => x.Marked(EditTimeEntry.StopButton)).Any();

        public static bool DurationIsCorrect(this IApp app, string duration)
            => app.Query(x => x.Marked(EditTimeEntry.Duration).Text(duration)).Any();

        public static bool StartDateIsCorrect(this IApp app, string startDate)
            => app.Query(x => x.Marked(EditTimeEntry.StartDate).Text(startDate)).Any();
    }
}
