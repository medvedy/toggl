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
using static Android.Content.Intent;

namespace Toggl.Droid
{
    [Activity(Label = "Toggl for Devs",
              MainLauncher = true,
              Icon = "@mipmap/ic_launcher",
              Theme = "@style/Theme.Splash",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class SplashScreen : AppCompatActivity, IView
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
                .CheckIfUserHasFullAppAccess().GetAwaiter().GetResult();

            if (!hasAppAccess)
            {
                Finish();
                return;
            }

            AndroidDependencyContainer.Instance
                .NavigationService
                .Navigate<MainTabBarViewModel>(this)
                .ContinueWith(_ => Finish());
            return;
        }

        private void createApplicationLifecycleObserver(IBackgroundService backgroundService)
        {
            var appLifecycleObserver = new ApplicationLifecycleObserver(backgroundService);
            Application.RegisterActivityLifecycleCallbacks(appLifecycleObserver);
            Application.RegisterComponentCallbacks(appLifecycleObserver);
        }

        public IObservable<bool> Confirm(string title, string message, string confirmButtonText, string dismissButtonText)
        {
            throw new InvalidOperationException("You shouldn't be doing this from a splash screen");
        }

        public IObservable<Unit> Alert(string title, string message, string buttonTitle)
        {
            throw new InvalidOperationException("You shouldn't be doing this from a splash screen");
        }

        public IObservable<bool> ConfirmDestructiveAction(ActionType type, params object[] formatArguments)
        {
            throw new InvalidOperationException("You shouldn't be doing this from a splash screen");
        }

        public IObservable<T> Select<T>(string title, IEnumerable<SelectOption<T>> options, int initialSelectionIndex)
        {
            throw new InvalidOperationException("You shouldn't be doing this from a splash screen");
        }

        public IObservable<bool> RequestCalendarAuthorization(bool force = false)
        {
            throw new InvalidOperationException("You shouldn't be doing this from a splash screen");
        }

        public IObservable<bool> RequestNotificationAuthorization(bool force = false)
        {
            throw new InvalidOperationException("You shouldn't be doing this from a splash screen");
        }

        public void OpenAppSettings()
        {
            throw new InvalidOperationException("You shouldn't be doing this from a splash screen");
        }

        public IObservable<string> GetGoogleToken()
        {
            throw new InvalidOperationException("You shouldn't be doing this from a splash screen");
        }

        public Task Close()
        {
            throw new InvalidOperationException("You shouldn't be doing this from a splash screen");
        }
    }
}
