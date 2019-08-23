using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Adapters;
using Toggl.Droid.Extensions;
using Toggl.Droid.ViewHolders;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public partial class SelectWorkspaceFragment : ReactiveDialogFragment<SelectWorkspaceViewModel>
    {
        public SelectWorkspaceFragment() { }

        public SelectWorkspaceFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.SelectWorkspaceFragment, container, false);
            InitializeViews(view);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            var adapter = new SimpleAdapter<SelectableWorkspaceViewModel>(
                Resource.Layout.SelectWorkspaceFragmentCell,
                SelectWorkspaceViewHolder.Create
            );

            adapter.ItemTapObservable
                .Subscribe(ViewModel.SelectWorkspace.Inputs)
                .DisposedBy(DisposeBag);

            adapter.Items = ViewModel.Workspaces;

            recyclerView.SetAdapter(adapter);
            recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
        }

        public override void OnResume()
        {
            base.OnResume();
            Dialog.Window.SetDefaultDialogLayout(Activity, Context, ViewGroup.LayoutParams.WrapContent);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            DisposeBag.Dispose();
        }
    }
}
