using System;
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
        private readonly IRemoteConfigUpdateService remoteConfigUpdateService;

        private DateTimeOffset? lastEnteredBackground { get; set; }
        private ISubject<TimeSpan> appBecameActiveSubject { get; }

        public IObservable<TimeSpan> AppResumedFromBackground { get; }

        public BackgroundService(ITimeService timeService, IAnalyticsService analyticsService, IRemoteConfigUpdateService remoteConfigUpdateService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(remoteConfigUpdateService, nameof(remoteConfigUpdateService));

            this.timeService = timeService;
            this.analyticsService = analyticsService;
            this.remoteConfigUpdateService = remoteConfigUpdateService;

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
            var timeSinceLastRemoteConfigFetch = remoteConfigUpdateService.TimeSpanSinceLastFetch();
            if (!timeSinceLastRemoteConfigFetch.HasValue || timeSinceLastRemoteConfigFetch.Value > RemoteConfigConstants.RemoteConfigExpiration)
            {
                Task.Run(() => remoteConfigUpdateService.FetchAndStoreRemoteConfigData())
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
