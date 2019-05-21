using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Toggl.Core.Services;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.Droid.BroadcastReceivers;
using Toggl.Droid.Presentation;
using static Android.Content.Intent;

namespace Toggl.Droid
{
    [Activity(Label = "Toggl for Devs",
              MainLauncher = true,
              Icon = "@mipmap/ic_launcher",
              Theme = "@style/Theme.Splash",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class SplashScreen : AppCompatActivity
    {
        public SplashScreen()
            : base()
        {
#if !USE_PRODUCTION_API
            System.Net.ServicePointManager.ServerCertificateValidationCallback
                  += (sender, certificate, chain, sslPolicyErrors) => true;
#endif
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var dependencyContainer = AndroidDependencyContainer.Instance;
            var app = new App<LoginViewModel, CredentialsParameter>(dependencyContainer);

            ApplicationContext.RegisterReceiver(new TimezoneChangedBroadcastReceiver(dependencyContainer.TimeService),
                new IntentFilter(ActionTimezoneChanged));

            createApplicationLifecycleObserver(dependencyContainer.BackgroundService);

            var hasAppAccess = app.Initialize()
                .NavigateWhenUserDoesNotHaveFullAppAccess().GetAwaiter().GetResult();

            if (!hasAppAccess)
            {
                Finish();
                return;
            }

            AndroidStartupHelper.StartMainTabBarActivity(this);
            Finish();
        }

        private void createApplicationLifecycleObserver(IBackgroundService backgroundService)
        {
            var appLifecycleObserver = new ApplicationLifecycleObserver(backgroundService);
            Application.RegisterActivityLifecycleCallbacks(appLifecycleObserver);
            Application.RegisterComponentCallbacks(appLifecycleObserver);
        }
    }
}
