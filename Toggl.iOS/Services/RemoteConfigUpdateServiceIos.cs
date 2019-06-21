using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Firebase.RemoteConfig;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Storage.Settings;
using static Toggl.Core.Services.RemoteConfigKeys;

namespace Toggl.iOS.Services
{
    public class RemoteConfigUpdateServiceIos : IRemoteConfigUpdateService
    {
        private const string remoteConfigDefaultsFileName = "RemoteConfigDefaults";
        
        private bool isRunning;
        private IKeyValueStorage keyValueStorage;
        private readonly object updateLock = new object();
        private readonly ISubject<Unit> remoteConfigUpdatedSubject = new BehaviorSubject<Unit>(Unit.Default);
        
        public IObservable<Unit> RemoteConfigChanged { get; }

        public RemoteConfigUpdateServiceIos(IKeyValueStorage keyValueStorage)
        {
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));
            this.keyValueStorage = keyValueStorage;

            RemoteConfigChanged = remoteConfigUpdatedSubject.AsObservable();
            var remoteConfig = RemoteConfig.SharedInstance;
            remoteConfig.SetDefaults(plistFileName: remoteConfigDefaultsFileName);
        }
        
        public void FetchAndStoreRemoteConfigData()
        {
            lock (updateLock)
            {
                if (isRunning) return;
                isRunning = true;
            }
            
            var remoteConfig = RemoteConfig.SharedInstance;
            remoteConfig.Fetch((status, error) =>
            {
                if (error == null)
                    remoteConfig.ActivateFetched();
                    
                var ratingViewConfiguration = extractRatingViewConfiguration(remoteConfig);
                var pushNotificationsConfiguration = extractPushNotificationsConfiguration(remoteConfig);
                
                keyValueStorage.SetInt(RatingViewDelayParameter, ratingViewConfiguration.DayCount);
                keyValueStorage.SetString(RatingViewTriggerParameter, ratingViewConfiguration.Criterion.ToString());
                keyValueStorage.SetBool(RegisterPushNotificationsTokenWithServerParameter, pushNotificationsConfiguration.RegisterPushNotificationsTokenWithServer);
                keyValueStorage.SetBool(HandlePushNotificationsParameter, pushNotificationsConfiguration.HandlePushNotifications);
                
                remoteConfigUpdatedSubject.OnNext(Unit.Default);

                lock (updateLock)
                {
                    isRunning = false;
                }
            });
        }
        
        private RatingViewConfiguration extractRatingViewConfiguration(RemoteConfig remoteConfig) 
            => new RatingViewConfiguration(
                remoteConfig[RatingViewDelayParameter].NumberValue.Int32Value,
                remoteConfig[RatingViewTriggerParameter].StringValue.ToRatingViewCriterion());

        private PushNotificationsConfiguration extractPushNotificationsConfiguration(RemoteConfig remoteConfig)
            => new PushNotificationsConfiguration(
                remoteConfig[RegisterPushNotificationsTokenWithServerParameter].BoolValue,
                remoteConfig[HandlePushNotificationsParameter].BoolValue); 
    }
}
