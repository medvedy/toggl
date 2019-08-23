using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Views;

namespace Toggl.Droid.Fragments
{
    public class ReportsCalendarFragment : ReactiveDialogFragment<ReportsCalendarViewModel>
    {
        private ReportsCalendarView calendarView;

        public ReportsCalendarFragment() { }

        public ReportsCalendarFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.ReportsCalendar, null);
            InitializeViews(view);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            calendarView.SetupWith(ViewModel);
        }

        public override void OnResume()
        {
            base.OnResume();

            var (widthPixels, _, _) = Activity.GetMetrics(Context);
            var width = widthPixels - 24.DpToPixels(Context);
            Dialog.Window.SetLayout(width, ViewGroup.LayoutParams.WrapContent);
        }

        protected override void InitializeViews(View view)
        {
            calendarView = view.FindViewById<ReportsCalendarView>(Resource.Id.ReportsFragmentCalendarView);
        }
    }
}
