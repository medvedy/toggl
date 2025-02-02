using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.ViewModels;

namespace Toggl.Droid.ViewHolders
{
    public class SelectWorkspaceViewHolder : BaseRecyclerViewHolder<SelectableWorkspaceViewModel>
    {
        public static SelectWorkspaceViewHolder Create(View itemView)
            => new SelectWorkspaceViewHolder(itemView);

        private TextView workspaceNameTextView;
        private RadioButton selectWorkspaceFragmentCellRadioButton;

        public SelectWorkspaceViewHolder(View itemView)
            : base(itemView)
        {
        }

        public SelectWorkspaceViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            workspaceNameTextView = ItemView.FindViewById<TextView>(Resource.Id.SelectWorkspaceFragmentCellTextView);
            selectWorkspaceFragmentCellRadioButton = ItemView.FindViewById<RadioButton>(Resource.Id.SelectWorkspaceFragmentCellRadioButton);
        }

        protected override void UpdateView()
        {
            workspaceNameTextView.Text = Item.WorkspaceName;
            selectWorkspaceFragmentCellRadioButton.Checked = Item.Selected;
        }
    }
}