using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.Droid.Extensions;
using static Toggl.Core.Helper.Colors;

namespace Toggl.Droid.ViewHelpers
{
    public class TimeEntryViewData
    {
        public LogItemViewModel ViewModel { get; }
        public ISpannable ProjectTaskClientText { get; }
        public ViewStates ProjectArchivedIconVisibility { get; }
        public Color ProjectArchivedIconTintColor { get; }
        public ViewStates ProjectTaskClientVisibility { get; }
        public ViewStates HasTagsIconVisibility { get; }
        public ViewStates BillableIconVisibility { get; }
        public ViewStates ContinueButtonVisibility { get; }
        public ViewStates ErrorNeedsSyncVisibility { get; }
        public ViewStates ErrorImageViewVisibility { get; }
        public ViewStates ContinueImageVisibility { get; }
        public ViewStates AddDescriptionLabelVisibility { get; }
        public ViewStates DescriptionVisibility { get; }

        public TimeEntryViewData(LogItemViewModel viewModel)
        {
            ViewModel = viewModel;

            var spannableString = new SpannableStringBuilder();
            if (viewModel.HasProject)
            {
                spannableString.Append(viewModel.ProjectName, new ForegroundColorSpan(Color.ParseColor(viewModel.ProjectColor)), SpanTypes.ExclusiveInclusive);
                ProjectArchivedIconTintColor = Color.ParseColor(viewModel.ProjectColor);
                ProjectArchivedIconVisibility = (!viewModel.IsActive).ToVisibility();

                if (!string.IsNullOrEmpty(viewModel.TaskName))
                {
                    spannableString.Append($": {viewModel.TaskName}");
                }

                if (!string.IsNullOrEmpty(viewModel.ClientName))
                {
                    spannableString.Append($" {viewModel.ClientName}", new ForegroundColorSpan(Color.ParseColor(ClientNameColor)), SpanTypes.ExclusiveExclusive);
                }

                ProjectTaskClientText = spannableString;
                ProjectTaskClientVisibility = ViewStates.Visible;
            }
            else
            {
                ProjectTaskClientText = new SpannableString(string.Empty);
                ProjectTaskClientVisibility = ViewStates.Gone;
                ProjectArchivedIconVisibility = ViewStates.Gone;
            }
            DescriptionVisibility = viewModel.HasDescription.ToVisibility();
            AddDescriptionLabelVisibility = (!viewModel.HasDescription).ToVisibility();
            ContinueImageVisibility = viewModel.CanContinue.ToVisibility();
            ErrorImageViewVisibility = (!viewModel.CanContinue).ToVisibility();
            ErrorNeedsSyncVisibility = viewModel.NeedsSync.ToVisibility();
            ContinueButtonVisibility = viewModel.CanContinue.ToVisibility();
            BillableIconVisibility = viewModel.IsBillable.ToVisibility();
            HasTagsIconVisibility = viewModel.HasTags.ToVisibility();
        }
    }
}
