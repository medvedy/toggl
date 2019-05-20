using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Toggl.Core;
using Toggl.Core.Services;
using Toggl.Shared;

namespace Toggl.Droid
{
    public sealed class ApplicationLifecycleObserver : Java.Lang.Object, Application.IActivityLifecycleCallbacks, IComponentCallbacks2
    {
        private readonly IBackgroundService backgroundService;
        private bool isInBackground = true;

        public ApplicationLifecycleObserver(IBackgroundService backgroundService)
        {
            Ensure.Argument.IsNotNull(backgroundService, nameof(backgroundService));
            this.backgroundService = backgroundService;
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            if (!isInBackground)
                return;

            isInBackground = false;
            backgroundService.EnterForeground();
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
        }

        public void OnActivityStopped(Activity activity)
        {
        }

        public void OnConfigurationChanged(Configuration newConfig)
        {
        }

        public void OnLowMemory()
        {
        }

        public void OnTrimMemory(TrimMemory level)
        {
            switch (level)
            {
                case TrimMemory.RunningCritical:
                case TrimMemory.RunningLow:
                    AndroidDependencyContainer.Instance
                        .AnalyticsService
                        .ReceivedLowMemoryWarning
                        .Track(Platform.Giskard);
                    break;
                case TrimMemory.UiHidden:
                    isInBackground = true;
                    backgroundService.EnterBackground();
                    break;
            }
        }
    }
}
