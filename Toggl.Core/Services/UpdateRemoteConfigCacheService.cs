using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Shared;
using Toggl.Storage.Settings;
using static Toggl.Core.Services.RemoteConfigKeys;

namespace Toggl.Core.Services
{
    public sealed class UpdateRemoteConfigCacheService : IUpdateRemoteConfigCacheService
    {
        private readonly object updateLock = new object();
        private readonly ITimeService timeService;
        private readonly IKeyValueStorage keyValueStorage;
        private readonly IFetchRemoteConfigService fetchRemoteConfigService;
        private readonly ISubject<Unit> remoteConfigUpdatedSubject = new BehaviorSubject<Unit>(Unit.Default);
        private bool isRunning;

        public IObservable<Unit> RemoteConfigChanged => remoteConfigUpdatedSubject.AsObservable();

        public UpdateRemoteConfigCacheService(ITimeService timeService, IKeyValueStorage keyValueStorage, IFetchRemoteConfigService fetchRemoteConfigService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));
            Ensure.Argument.IsNotNull(fetchRemoteConfigService, nameof(fetchRemoteConfigService));

            this.timeService = timeService;
            this.keyValueStorage = keyValueStorage;
            this.fetchRemoteConfigService = fetchRemoteConfigService;
        }

        public TimeSpan? TimeSpanSinceLastFetch()
        {
            var lastFetchAt = keyValueStorage.GetDateTimeOffset(LastFetchAtKey);
            if (!lastFetchAt.HasValue) return null;

            var now = timeService.CurrentDateTime;
            return now.Subtract(lastFetchAt.Value);
        }

        public void FetchAndStoreRemoteConfigData()
        {
            lock (updateLock)
            {
                if (isRunning) return;
                isRunning = true;
            }

            fetchRemoteConfigService.FetchRemoteConfigData(onFetchSucceeded, onFetchFailed);
        }

        private void onFetchSucceeded()
        {
            var ratingViewConfiguration = fetchRemoteConfigService.ExtractRatingViewConfigurationFromRemoteConfig();
            var pushNotificationsConfiguration = fetchRemoteConfigService.ExtractPushNotificationsConfigurationFromRemoteConfig();

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
        }

        private void onFetchFailed(Exception exception)
        {
            lock (updateLock)
            {
                isRunning = false;
            }
        }
    }
}