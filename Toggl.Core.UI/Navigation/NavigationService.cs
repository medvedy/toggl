using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.Shared;

namespace Toggl.Core.UI.Navigation
{
    public sealed class NavigationService : INavigationService
    {
        private readonly CompositePresenter presenter;
        private readonly IAnalyticsService analyticsService;
        private readonly ViewModelLoader viewModelLocator;

        public NavigationService(
            CompositePresenter presenter,
            ViewModelLoader viewModelLocator,
            IAnalyticsService analyticsService)
        {
            Ensure.Argument.IsNotNull(presenter, nameof(presenter));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(viewModelLocator, nameof(viewModelLocator));

            this.presenter = presenter;
            this.analyticsService = analyticsService;
            this.viewModelLocator = viewModelLocator;
        }

        public async Task<TOutput> Navigate<TViewModel, TInput, TOutput>(TInput payload, IView sourceView = null)
            where TViewModel : ViewModel<TInput, TOutput>
        {
            var viewModel = viewModelLocator.Load<TViewModel>();

            await viewModel.Initialize(payload).ConfigureAwait(false);
            await presenter.Present(viewModel, sourceView).ConfigureAwait(false);

            analyticsService.CurrentPage.Track(typeof(TViewModel));

            return await viewModel.Result;
        }
    }
}
