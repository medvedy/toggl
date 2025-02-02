﻿using Android.Views;
using System;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Droid.ViewHolders;

namespace Toggl.Droid.Adapters
{
    public sealed class SelectDateFormatRecyclerAdapter : BaseRecyclerAdapter<SelectableDateFormatViewModel>
    {
        private const int selectableDateFormatViewType = 1;

        public override int GetItemViewType(int position)
            => selectableDateFormatViewType;

        protected override BaseRecyclerViewHolder<SelectableDateFormatViewModel> CreateViewHolder(ViewGroup parent, LayoutInflater inflater, int viewType)
        {
            switch (viewType)
            {
                case selectableDateFormatViewType:
                    var inflatedView = inflater.Inflate(Resource.Layout.SelectDateFormatFragmentCell, parent, false);
                    return new SelectDateFormatViewHolder(inflatedView);
                default:
                    throw new Exception("unsupported view type");
            }
        }
    }
}
