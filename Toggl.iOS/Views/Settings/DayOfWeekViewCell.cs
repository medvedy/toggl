﻿using Foundation;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Cells;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Settings
{
    public partial class DayOfWeekViewCell : BaseTableViewCell<SelectableBeginningOfWeekViewModel>
    {
        public static readonly string Identifier = nameof(DayOfWeekViewCell);
        public static readonly NSString Key = new NSString(nameof(DayOfWeekViewCell));
        public static readonly UINib Nib;

        static DayOfWeekViewCell()
        {
            Nib = UINib.FromName(nameof(DayOfWeekViewCell), NSBundle.MainBundle);
        }

        protected DayOfWeekViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        protected override void UpdateView()
        {
            DayOfWeekLabel.Text = Item.BeginningOfWeek.ToLocalizedString();
            SelectedImageView.Hidden = !Item.Selected;
        }
    }
}
