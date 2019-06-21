﻿using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Shared;

namespace Toggl.Core.Services
{
    public sealed class BackgroundService : IBackgroundService
    {
        private readonly ITimeService timeService;
        private readonly IAnalyticsService analyticsService;
        private readonly IUpdateRemoteConfigCacheService updateRemoteConfigCacheService;

        private DateTimeOffset? lastEnteredBackground { get; set; }
        private ISubject<TimeSpan> appBecameActiveSubject { get; }

        public IObservable<TimeSpan> AppResumedFromBackground { get; }

        public BackgroundService(ITimeService timeService, IAnalyticsService analyticsService, IUpdateRemoteConfigCacheService updateRemoteConfigCacheService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(updateRemoteConfigCacheService, nameof(updateRemoteConfigCacheService));

            this.timeService = timeService;
            this.analyticsService = analyticsService;
            this.updateRemoteConfigCacheService = updateRemoteConfigCacheService;

            appBecameActiveSubject = new Subject<TimeSpan>();
            lastEnteredBackground = null;

            AppResumedFromBackground = appBecameActiveSubject.AsObservable();
        }

        public void EnterBackground()
        {
            analyticsService.AppSentToBackground.Track();
            lastEnteredBackground = timeService.CurrentDateTime;
        }

        public void EnterForeground()
        {
            var timeSinceLastRemoteConfigFetch = updateRemoteConfigCacheService.TimeSpanSinceLastFetch();
            if (!timeSinceLastRemoteConfigFetch.HasValue || timeSinceLastRemoteConfigFetch.Value > RemoteConfigConstants.RemoteConfigExpiration)
            {
                Task.Run(() => updateRemoteConfigCacheService.FetchAndStoreRemoteConfigData())
                    .ConfigureAwait(false);
            }

            if (lastEnteredBackground.HasValue == false)
                return;

            var timeInBackground = timeService.CurrentDateTime - lastEnteredBackground.Value;
            lastEnteredBackground = null;
            appBecameActiveSubject.OnNext(timeInBackground);
            analyticsService.AppDidEnterForeground.Track();
        }
    }
}
