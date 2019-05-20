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
        public static void ClearSubViewModelsState()
        {
            var mainTabBarViewModel = loadMainTabBarViewModel();
            
            AndroidDependencyContainer
                .Instance
                .ViewModelCache
                .ClearAll();
            
            AndroidDependencyContainer
                .Instance
                .ViewModelCache
                .Cache(mainTabBarViewModel);
        }

        public static void StartMainTabBarActivity(Activity activity)
        {
            AndroidDependencyContainer
                .Instance
                .ViewModelCache
                .Cache(locateMainTabBarViewModel());

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