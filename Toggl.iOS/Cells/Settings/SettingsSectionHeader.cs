using System;

using Foundation;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Cells.Settings
{
    public partial class SettingsSectionHeader : BaseTableHeaderFooterView<string>
    {
        public static readonly string Identifier = new NSString("SettingsSectionHeader");
        public static readonly UINib Nib;

        static SettingsSectionHeader()
        {
            Nib = UINib.FromName("SettingsSectionHeader", NSBundle.MainBundle);
        }

        protected SettingsSectionHeader(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            TitleLabel.TextColor = Colors.Settings.SectionHeaderText.ToNativeColor();
            ContentView.BackgroundColor = Colors.Settings.Background.ToNativeColor();
            BottomSeparator.BackgroundColor = Colors.Settings.SeparatorColor.ToNativeColor();
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Item;
        }
    }
}

