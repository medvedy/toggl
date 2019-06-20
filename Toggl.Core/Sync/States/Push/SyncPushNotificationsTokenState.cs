using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Interactors;
using Toggl.Core.Interactors.PushNotifications;
using Toggl.Core.Services;
using Toggl.Networking;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;
using Toggl.Core.Extensions;

namespace Toggl.Core.Sync.States.Push
{
    public sealed class SyncPushNotificationsTokenState : ISyncState
    {
        private readonly IKeyValueStorage keyValueStorage;
        private readonly ITogglApi togglApi;
        private readonly IPushNotificationsTokenService pushNotificationsTokenService;
        private readonly ITimeService timeService;
        private readonly IRemoteConfigService remoteConfigService;

        public StateResult Done { get; } = new StateResult();

        public SyncPushNotificationsTokenState(
            IKeyValueStorage keyValueStorage,
            ITogglApi togglApi,
            IPushNotificationsTokenService pushNotificationsTokenService,
            ITimeService timeService,
            IRemoteConfigService remoteConfigService)
        {
            Ensure.Argument.IsNotNull(keyValueStorage, nameof(keyValueStorage));
            Ensure.Argument.IsNotNull(togglApi, nameof(togglApi));
            Ensure.Argument.IsNotNull(pushNotificationsTokenService, nameof(pushNotificationsTokenService));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(remoteConfigService, nameof(remoteConfigService));

            this.keyValueStorage = keyValueStorage;
            this.togglApi = togglApi;
            this.pushNotificationsTokenService = pushNotificationsTokenService;
            this.timeService = timeService;
            this.remoteConfigService = remoteConfigService;
        }

        public IObservable<ITransition> Start()
            => handlePushNotificationTokenSubscription().SelectValue(Done.Transition());

        private IObservable<Unit> handlePushNotificationTokenSubscription()
            => remoteConfigService.GetPushNotificationsConfiguration().RegisterPushNotificationsTokenWithServer
                ? createSubscriptionInteractor().Execute()
                : createUnsubscriptionInteractor().Execute();

        private IInteractor<IObservable<Unit>> createSubscriptionInteractor()
            => new SubscribeToPushNotificationsInteractor(keyValueStorage, togglApi, pushNotificationsTokenService, timeService);

        private IInteractor<IObservable<Unit>> createUnsubscriptionInteractor()
            => new UnsubscribeFromPushNotificationsInteractor(pushNotificationsTokenService, keyValueStorage, togglApi);
    }
}
