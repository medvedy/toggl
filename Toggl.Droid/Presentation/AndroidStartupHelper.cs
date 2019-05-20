using System.Reactive;
using Android.App;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;

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

        private static MainTabBarViewModel loadMainTabBarViewModel()
        {
            var vmCache = AndroidDependencyContainer.Instance.ViewModelCache;
            var cachedViewModel = vmCache.Get<MainTabBarViewModel>();
            if (cachedViewModel != null)
                return cachedViewModel;

            var viewModelLoader = new ViewModelLoader(AndroidDependencyContainer.Instance);
            var viewModel = viewModelLoader.Load<Unit, Unit>(typeof(MainTabBarViewModel), Unit.Default).GetAwaiter().GetResult();

            return (MainTabBarViewModel) viewModel;
        }
    }
}