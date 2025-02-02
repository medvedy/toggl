using Android.Views;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.ViewHolders;

namespace Toggl.Droid.Adapters
{
    public class SelectDurationFormatRecyclerAdapter : BaseRecyclerAdapter<SelectableDurationFormatViewModel>
    {
        private const int selectableDurationFormatViewType = 1;

        public override int GetItemViewType(int position)
            => selectableDurationFormatViewType;

        protected override BaseRecyclerViewHolder<SelectableDurationFormatViewModel> CreateViewHolder(ViewGroup parent, LayoutInflater inflater, int viewType)
        {
            switch (viewType)
            {
                case selectableDurationFormatViewType:
                    var inflatedView = inflater.Inflate(Resource.Layout.SelectDurationFormatFragmentCell, parent, false);
                    return new SelectDurationFormatViewHolder(inflatedView);
                default:
                    throw new Exception("Unsupported view type");
            }
        }
    }
}
