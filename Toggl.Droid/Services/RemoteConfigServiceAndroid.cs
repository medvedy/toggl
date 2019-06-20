using Toggl.Core.Extensions;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Storage.Settings;
using GmsTask = Android.Gms.Tasks.Task;

namespace Toggl.Droid.Services
{
    public class RemoteConfigServiceAndroid : IRemoteConfigService
    {
        private readonly IKeyValueStorage keyValueStorage;

        public RemoteConfigServiceAndroid(IKeyValueStorage keyValueStorage)
        {
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));
            this.keyValueStorage = keyValueStorage;
        }

        public RatingViewConfiguration GetRatingViewConfiguration() 
            => keyValueStorage.ReadStoredRatingViewConfiguration();

        public PushNotificationsConfiguration GetPushNotificationsConfiguration()
            => keyValueStorage.ReadStoredPushNotificationsConfiguration();
    }
}
