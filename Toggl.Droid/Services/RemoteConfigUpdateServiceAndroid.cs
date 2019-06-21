using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Gms.Tasks;
using Firebase.RemoteConfig;
using Toggl.Core;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Storage.Settings;
using static Toggl.Core.Services.RemoteConfigKeys;
using GmsTask = Android.Gms.Tasks.Task;

namespace Toggl.Droid.Services
{
    public class RemoteConfigUpdateServiceAndroid : IRemoteConfigUpdateService
    {
        private bool isRunning;
        private readonly ITimeService timeService;
        private readonly IKeyValueStorage keyValueStorage;
        private readonly object updateLock = new object();
        private readonly ISubject<Unit> remoteConfigUpdatedSubject = new BehaviorSubject<Unit>(Unit.Default);
        
        public IObservable<Unit> RemoteConfigChanged { get; }

        [Conditional("DEBUG")]
        private void enableDeveloperModeInDebugModel(FirebaseRemoteConfig remoteConfig)
        {
            var settings = new FirebaseRemoteConfigSettings
                    .Builder()
                .SetDeveloperModeEnabled(true)
                .Build();

            remoteConfig.SetConfigSettings(settings);
        }

        public RemoteConfigUpdateServiceAndroid(IKeyValueStorage keyValueStorage, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));
            this.timeService = timeService;
            this.keyValueStorage = keyValueStorage;

            RemoteConfigChanged = remoteConfigUpdatedSubject.AsObservable();
        }
        
        public void FetchAndStoreRemoteConfigData()
        {
            lock (updateLock)
            {
                if (isRunning) return;
                isRunning = true;
            }
            
            var remoteConfig = FirebaseRemoteConfig.Instance;
            enableDeveloperModeInDebugModel(remoteConfig);
            remoteConfig.SetDefaults(Resource.Xml.RemoteConfigDefaults);

            remoteConfig.Fetch(error =>
            {
                if (error == null)
                    remoteConfig.ActivateFetched();

                var ratingViewConfiguration = extractRatingViewConfiguration(remoteConfig);
                var pushNotificationsConfiguration = extractPushNotificationsConfiguration(remoteConfig);
                
                keyValueStorage.SetInt(RatingViewDelayParameter, ratingViewConfiguration.DayCount);
                keyValueStorage.SetString(RatingViewTriggerParameter, ratingViewConfiguration.Criterion.ToString());
                keyValueStorage.SetBool(RegisterPushNotificationsTokenWithServerParameter, pushNotificationsConfiguration.RegisterPushNotificationsTokenWithServer);
                keyValueStorage.SetBool(HandlePushNotificationsParameter, pushNotificationsConfiguration.HandlePushNotifications);
                
                keyValueStorage.SetDateTimeOffset(LastFetchAtKey, timeService.CurrentDateTime);
                
                remoteConfigUpdatedSubject.OnNext(Unit.Default);

                lock (updateLock)
                {
                    isRunning = false;
                }
            });
        }

        public TimeSpan TimeSpanSinceLastFetch()
        {
            var now = timeService.CurrentDateTime;
            return now.Subtract(keyValueStorage.GetDateTimeOffset(LastFetchAtKey) ?? default(DateTimeOffset));
        }

        private RatingViewConfiguration extractRatingViewConfiguration(FirebaseRemoteConfig remoteConfig)
            => new RatingViewConfiguration(
                (int) remoteConfig.GetValue(RatingViewDelayParameter).AsLong(),
                remoteConfig.GetString(RatingViewTriggerParameter).ToRatingViewCriterion());

        private PushNotificationsConfiguration extractPushNotificationsConfiguration(FirebaseRemoteConfig remoteConfig)
            => new PushNotificationsConfiguration(
                remoteConfig.GetBoolean(RegisterPushNotificationsTokenWithServerParameter),
                remoteConfig.GetBoolean(HandlePushNotificationsParameter));
    }

    public class RemoteConfigCompletionHandler : Java.Lang.Object, IOnCompleteListener
    {
        private readonly Action<Exception> action;
        
        public RemoteConfigCompletionHandler(Action<Exception> action)
        {
            this.action = action;
        }

        public void OnComplete(GmsTask task)
        {
            action(task.IsSuccessful ? null : task.Exception);
        }
    }

    public static class FirebaseExtensions
    {
        public static void Fetch(this FirebaseRemoteConfig remoteConfig, Action<Exception> action)
        {
            remoteConfig.Fetch().AddOnCompleteListener(new RemoteConfigCompletionHandler(action));
        }
    }
}
