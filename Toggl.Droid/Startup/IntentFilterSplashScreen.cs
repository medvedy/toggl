using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Toggl.Core;
using Toggl.Core.UI;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.BroadcastReceivers;
using Toggl.Droid.Helper;
using Toggl.Droid.Presentation;
using static Android.Content.Intent;

namespace Toggl.Droid.Startup
{
    [Activity(
        Theme = "@style/Theme.Splash",
        NoHistory = true,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    [IntentFilter(
        new[] { "android.intent.action.VIEW", "android.intent.action.EDIT" },
        Categories = new[] { "android.intent.category.BROWSABLE", "android.intent.category.DEFAULT" },
        DataSchemes = new[] { "toggl" },
        DataHost = "*")]
    [IntentFilter(
        new[] { "android.intent.action.PROCESS_TEXT" },
        Categories = new[] { "android.intent.category.DEFAULT" },
        DataMimeType = "text/plain")]
    public class IntentFilterSplashScreen : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            var dependencyContainer = AndroidDependencyContainer.Instance;
            var app = new App<LoginViewModel, CredentialsParameter>(dependencyContainer);

            ApplicationContext.RegisterReceiver(new TimezoneChangedBroadcastReceiver(dependencyContainer.TimeService),
                new IntentFilter(ActionTimezoneChanged));
            
            var hasAppAccess = app.Initialize()
                .NavigateWhenUserDoesNotHaveFullAppAccess().GetAwaiter().GetResult();

            if (!hasAppAccess)
            {
                Finish();
                return;
            }
            
            var navigationUrl = Intent.Data?.ToString() ?? getTrackUrlFromProcessedText();
            
            if (string.IsNullOrEmpty(navigationUrl))
            {
                AndroidStartupHelper.StartMainTabBarActivity(this);
                Finish();
                return;
            }

            var timeService = dependencyContainer.TimeService;
            var interactorFactory = dependencyContainer.InteractorFactory;
            var urlHandler = dependencyContainer.UrlHandler;
            var androidUrlHandler = new AndroidUrlHandler(timeService, interactorFactory, urlHandler);
            androidUrlHandler.HandleUrlForAppStart(navigationUrl, this);
        }
        
        private string getTrackUrlFromProcessedText()
        {
            if (MarshmallowApis.AreNotAvailable)
                return null;

            var description = Intent.GetStringExtra(ExtraProcessText);
            if (string.IsNullOrWhiteSpace(description))
                return null;

            var applicationUrl = ApplicationUrls.TimeEntry.Start.WithDescription(description);
            return applicationUrl;
        }
    }
}