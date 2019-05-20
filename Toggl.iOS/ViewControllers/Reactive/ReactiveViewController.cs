using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public abstract partial class ReactiveViewController<TViewModel> : UIViewController, IView
        where TViewModel : IViewModel
    {
        private bool hasFinished;

        public CompositeDisposable DisposeBag { get; private set; } = new CompositeDisposable();

        public TViewModel ViewModel { get; set; }

        protected ReactiveViewController(TViewModel viewModel) : this(viewModel, null)
        {
            ViewModel = viewModel;
        }

        protected ReactiveViewController(TViewModel viewModel, string nibName) : base(nibName, null)
        {
            ViewModel = viewModel;
        }

        protected ReactiveViewController(IntPtr handle)
            : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ViewModel?.AttachView(this);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ViewModel?.ViewAppearing();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ViewModel?.ViewAppeared();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ViewModel?.ViewDisappearing();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            ViewModel?.ViewDisappeared();
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            // When this is removed from the parent controller (e.g: a pop action in a navigation controller)
            // then the view is destroyed and the view model needs to be cancelled
            if (parent == null)
            {
                if (!hasFinished)
                {
                    ViewModel?.Cancel();
                }

                ViewModel?.DetachView();
                ViewModel?.ViewDestroyed();
            }
        }

        public override void DismissViewController(bool animated, Action completionHandler)
        {
            base.DismissViewController(animated, completionHandler);

            if (!IsBeingDismissed)
                return;

            ViewModel?.DetachView();
            ViewModel?.ViewDestroyed();
        }

        public Task Close()
        {
            hasFinished = true;
            UIApplication.SharedApplication.InvokeOnMainThread(this.Dismiss);
            return Task.CompletedTask;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            DisposeBag?.Dispose();
        }
    }
}
