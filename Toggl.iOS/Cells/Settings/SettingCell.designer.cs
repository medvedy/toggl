// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.Cells.Settings
{
	[Register ("SettingCell")]
	partial class SettingCell
	{
		[Outlet]
		UIKit.UIView BottomSeparator { get; set; }

		[Outlet]
		UIKit.UILabel DetailLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint RightConstraint { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		[Outlet]
		UIKit.UIView TopSeparator { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DetailLabel != null) {
				DetailLabel.Dispose ();
				DetailLabel = null;
			}

			if (RightConstraint != null) {
				RightConstraint.Dispose ();
				RightConstraint = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TopSeparator != null) {
				TopSeparator.Dispose ();
				TopSeparator = null;
			}

			if (BottomSeparator != null) {
				BottomSeparator.Dispose ();
				BottomSeparator = null;
			}
		}
	}
}
