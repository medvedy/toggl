﻿using System;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Toggl.Core.UI.ViewModels;
using static Toggl.Core.Helper.Color;

namespace Toggl.Droid.Extensions
{
    public static class TimeEntryExtensions
    {
        public static ISpannable ToProjectTaskClient(bool hasProject, string project, string projectColor, string task, string client)
        {
            if (!hasProject)
                return new SpannableString(string.Empty);

            var spannableString = new SpannableStringBuilder();

            spannableString.Append(
                project,
                new ForegroundColorSpan(Color.ParseColor(projectColor)),
                SpanTypes.ExclusiveInclusive);

            if (!string.IsNullOrEmpty(task))
            {
                spannableString.Append($": {task}");
            }

            if (!string.IsNullOrEmpty(client))
            {
                spannableString.Append(
                    $" {client}",
                    new ForegroundColorSpan(Color.ParseColor(ClientNameColor)),
                    SpanTypes.ExclusiveExclusive);
            }

            return spannableString;
        }
    }
}
