using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Toggl.Core.UI.Navigation;
using Toggl.Core;
using Toggl.Core.UI;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
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
    public class ExternalIntentsHandlerActivity : AppCompatActivity, IView
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            var dependencyContainer = AndroidDependencyContainer.Instance;
            var app = new App<LoginViewModel, CredentialsParameter>(dependencyContainer);

            ApplicationContext.RegisterReceiver(new TimezoneChangedBroadcastReceiver(dependencyContainer.TimeService),
                new IntentFilter(ActionTimezoneChanged));
            
            var hasAppAccess = app.Initialize()
                .CheckIfUserHasFullAppAccess().GetAwaiter().GetResult();

            if (!hasAppAccess)
            {
                Finish();
                return;
            }
            
            var navigationUrl = Intent.Data?.ToString() ?? getTrackUrlFromProcessedText();
            
            if (string.IsNullOrEmpty(navigationUrl))
            {
                AndroidDependencyContainer.Instance
                    .NavigationService
                    .Navigate<MainTabBarViewModel>(this)
                    .ContinueWith(_ => Finish());
                return;
            }

            var androidHandler = (AndroidUrlHandler)AndroidDependencyContainer.Instance.UrlHandler;
            androidHandler.HandleUrlForAppStart(navigationUrl, this);
        }
        
        private string getTrackUrlFromProcessedText()
        {
            if (MarshmallowApis.AreNotAvailable)
                return null;

            var description = Intent.GetStringExtra(ExtraProcessText);
            if (string.IsNullOrWhiteSpace(description))
                return null;

            var applicationUrl = ApplicationUrls.Track.Default(description);
            return applicationUrl;
        }

        public IObservable<bool> Confirm(string title, string message, string confirmButtonText, string dismissButtonText)
            => throw new InvalidOperationException("You shouldn't be doing this from a splash screen");

        public IObservable<Unit> Alert(string title, string message, string buttonTitle)
            => throw new InvalidOperationException("You shouldn't be doing this from a splash screen");

        public IObservable<bool> ConfirmDestructiveAction(ActionType type, params object[] formatArguments)
            => throw new InvalidOperationException("You shouldn't be doing this from a splash screen");

        public IObservable<T> Select<T>(string title, IEnumerable<SelectOption<T>> options, int initialSelectionIndex)
            => throw new InvalidOperationException("You shouldn't be doing this from a splash screen");

        public IObservable<bool> RequestCalendarAuthorization(bool force = false)
            => throw new InvalidOperationException("You shouldn't be doing this from a splash screen");

        public IObservable<bool> RequestNotificationAuthorization(bool force = false)
            => throw new InvalidOperationException("You shouldn't be doing this from a splash screen");

        public void OpenAppSettings()
            => throw new InvalidOperationException("You shouldn't be doing this from a splash screen");

        public IObservable<string> GetGoogleToken()
            => throw new InvalidOperationException("You shouldn't be doing this from a splash screen");

        public Task Close()
            => throw new InvalidOperationException("You shouldn't be doing this from a splash screen");
    }
}