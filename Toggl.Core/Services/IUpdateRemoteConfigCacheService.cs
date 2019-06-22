using System;
using System.Reactive;

namespace Toggl.Core.Services
{
    public interface IUpdateRemoteConfigCacheService
    {
        IObservable<Unit> RemoteConfigChanged { get; }
        void FetchAndStoreRemoteConfigData();
        bool NeedsToUpdateStoredRemoteConfigData();
    }

    public static class RemoteConfigKeys
    {
        public static string RatingViewDelayParameter = "day_count";
        public static string RatingViewTriggerParameter = "criterion";
        public static string RegisterPushNotificationsTokenWithServerParameter = "register_push_notifications_token_with_server";
        public static string HandlePushNotificationsParameter = "handle_push_notifications";
        public static string LastFetchAtKey = "LastFetchAtKey";
    }
}
