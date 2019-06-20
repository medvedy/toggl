using System;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Core.Services;
using Toggl.Networking;
using Toggl.Networking.Network;

namespace Toggl.Core.UI
{
    public abstract class UIDependencyContainer : DependencyContainer
    {
        private readonly Lazy<IDeeplinkParser> deeplinkParser;
        private readonly Lazy<IBrowserService> browserService;
        private readonly Lazy<ViewModelLoader> viewModelLoader;
        private readonly Lazy<INavigationService> navigationService;
        private readonly Lazy<IPermissionsChecker> permissionsService;

        public IDeeplinkParser DeeplinkParser => deeplinkParser.Value;
        public IBrowserService BrowserService => browserService.Value;
        public ViewModelLoader ViewModelLoader => viewModelLoader.Value;
        public INavigationService NavigationService => navigationService.Value;
        public IPermissionsChecker PermissionsChecker => permissionsService.Value;

        public static UIDependencyContainer Instance { get; protected set; }

        protected UIDependencyContainer(ApiEnvironment apiEnvironment, UserAgent userAgent)
            : base(apiEnvironment, userAgent)
        {
            deeplinkParser = new Lazy<IDeeplinkParser>(createDeeplinkParser);
            browserService = new Lazy<IBrowserService>(CreateBrowserService);
            viewModelLoader = new Lazy<ViewModelLoader>(CreateViewModelLoader);
            navigationService = new Lazy<INavigationService>(CreateNavigationService);
            permissionsService = new Lazy<IPermissionsChecker>(CreatePermissionsChecker);
        }

        private IDeeplinkParser createDeeplinkParser()
            => new DeeplinkParser();

        protected abstract IBrowserService CreateBrowserService();
        protected abstract INavigationService CreateNavigationService();
        protected abstract IPermissionsChecker CreatePermissionsChecker();

        protected virtual ViewModelLoader CreateViewModelLoader() => new ViewModelLoader(this);
        protected override IErrorHandlingService CreateErrorHandlingService()
            => new ErrorHandlingService(NavigationService, AccessRestrictionStorage);

        protected override IRemoteConfigService CreateRemoteConfigService()
            => new RemoteConfigService(KeyValueStorage);
    }
}
