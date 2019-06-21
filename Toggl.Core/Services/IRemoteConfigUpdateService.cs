using System;
using System.Reactive;

namespace Toggl.Core.Services
{
    public interface IRemoteConfigUpdateService
    {
        IObservable<Unit> RemoteConfigChanged { get; }
        void FetchAndStoreRemoteConfigData();
        TimeSpan TimeSpanSinceLastFetch();
    }

    public static class RemoteConfigKeys
    {
        public static string RatingViewDelayParameter = "day_count";
        public static string RatingViewTriggerParameter = "criterion";
        public static string RegisterPushNotificationsTokenWithServerParameter = "register_push_notifications_token_with_server";
        public static string HandlePushNotificationsParameter = "handle_push_notifications";

        public static string LastFetchAtKey = "LastFetchAtKey";
    }

    public static class RemoteConfigConstants
    {
        public static readonly TimeSpan RemoteConfigExpiration = TimeSpan.FromHours(12.5f);
    }
}
