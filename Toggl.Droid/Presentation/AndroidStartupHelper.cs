using System.Reactive;
using Android.App;
using Android.Content;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Activities;

namespace Toggl.Droid.Presentation
{
    public static class AndroidStartupHelper
    {
        public static void EnsureCleanRootViewModelState()
        {
            var mainTabBarViewModel = loadMainTabBarViewModel();
            var instanceViewModelCache = AndroidDependencyContainer.Instance.ViewModelCache;
            
            instanceViewModelCache.Clear<MainTabBarViewModel>();
            instanceViewModelCache.ClearAll();
            instanceViewModelCache.Cache(mainTabBarViewModel);
        }

        public static void StartMainTabBarActivity(Activity activity)
        {
            EnsureCleanRootViewModelState();

            var intent = new Intent(activity, typeof(MainTabBarActivity))
                .AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            
            activity.StartActivity(intent);
        }

        private static MainTabBarViewModel loadMainTabBarViewModel()
        {
            var vmCache = AndroidDependencyContainer.Instance.ViewModelCache;
            var cachedViewModel = vmCache.Get<MainTabBarViewModel>();
            return cachedViewModel ?? locateMainTabBarViewModel();
        }

        private static MainTabBarViewModel locateMainTabBarViewModel()
        {
            var viewModelLoader = new ViewModelLoader(AndroidDependencyContainer.Instance);
            var viewModel = viewModelLoader.Load<Unit, Unit>(typeof(MainTabBarViewModel), Unit.Default).GetAwaiter().GetResult();

            return (MainTabBarViewModel) viewModel;
        }
    }
}