using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;

namespace Toggl.Droid.Fragments
{
    public sealed class SelectDateTimeFragment : ReactiveDialogFragment<SelectDateTimeViewModel>
    {
        public SelectDateTimeFragment()
        {
        }

        public SelectDateTimeFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            switch (ViewModel.Mode)
            {
                case DateTimePickerMode.Date:
                    return showDate();
                case DateTimePickerMode.Time:
                    return showTime();
                default:
                    throw new NotSupportedException();
            }
        }

        private DatePickerDialog showDate()
        {
            var localTime = ViewModel.CurrentDateTime.Value.ToLocalTime();

            var dialog = new DatePickerDialog(
                Activity, Resource.Style.TogglDialog, onDateSet,
                localTime.Year, localTime.Month - 1, localTime.Day);

            dialog.DatePicker.MinDate = ViewModel.MinDate.ToUnixTimeMilliseconds();
            dialog.DatePicker.MaxDate = ViewModel.MaxDate.ToUnixTimeMilliseconds();

            return dialog;
        }

        private TimePickerDialog showTime()
        {
            var localTime = ViewModel.CurrentDateTime.Value.ToLocalTime();

            var dialog = new TimePickerDialog(
                Activity, onTimeSet,
                localTime.Hour, localTime.Minute, ViewModel.Is24HoursFormat);

            return dialog;
        }

        private void onDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            var localTime = ViewModel.CurrentDateTime.Value.ToLocalTime();

            ViewModel.CurrentDateTime.Accept(
                new DateTimeOffset(e.Year, e.Month + 1, e.DayOfMonth,
                    localTime.Hour, localTime.Minute, localTime.Second,
                    localTime.Offset));

            ViewModel.Save.Execute();
        }

        private void onTimeSet(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            var localTime = ViewModel.CurrentDateTime.Value.ToLocalTime();

            ViewModel.CurrentDateTime.Accept(
                new DateTimeOffset(localTime.Year, localTime.Month, localTime.Day,
                    e.HourOfDay, e.Minute, localTime.Second,
                    localTime.Offset));            
        }

        protected override void InitializeViews(View view)
        {
        }
    }
}
