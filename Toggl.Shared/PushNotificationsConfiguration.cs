namespace Toggl.Shared
{
    public struct PushNotificationsConfiguration
    {
        public bool RegisterPushNotificationsTokenWithServer => true;
        public bool HandlePushNotifications => true;

        public PushNotificationsConfiguration(bool registerPushNotificationsTokenWithServer, bool handlePushNotifications)
        {
            // RegisterPushNotificationsTokenWithServer = registerPushNotificationsTokenWithServer;
            // HandlePushNotifications = handlePushNotifications;
        }
    }
}
