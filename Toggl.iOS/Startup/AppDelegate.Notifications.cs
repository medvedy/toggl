using System;
using System.Reactive;
using System.Reactive.Linq;
using Firebase.CloudMessaging;
using Foundation;
using Toggl.Core.Extensions;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS
{
    public partial class AppDelegate
    {
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Messaging.SharedInstance.ApnsToken = deviceToken;
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
        }

        [Export("messaging:didReceiveRegistrationToken:")]
        public void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
        {
            var dependencyContainer = IosDependencyContainer.Instance;
            var interactorFactory = dependencyContainer.InteractorFactory;

            if (!dependencyContainer.UserAccessManager.CheckIfLoggedIn())
                return;

            var shouldBeSubscribedToPushNotifications = dependencyContainer
                .RemoteConfigService
                .GetPushNotificationsConfiguration()
                .RegisterPushNotificationsTokenWithServer;

            if (!shouldBeSubscribedToPushNotifications) return;

            interactorFactory.SubscribeToPushNotifications()
                .Execute()
                .Take(1)
                .Subscribe();
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            var dependencyContainer = IosDependencyContainer.Instance;
            var interactorFactory = dependencyContainer.InteractorFactory;

            if (!dependencyContainer.UserAccessManager.CheckIfLoggedIn())
            {
                completionHandler(UIBackgroundFetchResult.NoData);
                return;
            }

            var shouldHandlePushNotifications = dependencyContainer
                .RemoteConfigService
                .GetPushNotificationsConfiguration()
                .HandlePushNotifications;

            if (!shouldHandlePushNotifications)
            {
                completionHandler(UIBackgroundFetchResult.NoData);
                return;
            }

            var syncInteractor = application.ApplicationState == UIApplicationState.Active
                ? interactorFactory.RunPushNotificationInitiatedSyncInForeground()
                : interactorFactory.RunPushNotificationInitiatedSyncInBackground();

            syncInteractor.Execute()
                .Select(mapToNativeOutcomes)
                .Subscribe(completionHandler);
        }
    }
}
