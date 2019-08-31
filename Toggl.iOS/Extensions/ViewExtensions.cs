using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class ViewExtensions
    {
        public static void Shake(this UIView view, double duration = 0.05, int repeatCount = 5, double shakeThreshold = 4.0)
        {
            var animation = CABasicAnimation.FromKeyPath("position");
            animation.Duration = duration;
            animation.RepeatCount = repeatCount;
            animation.AutoReverses = true;
            animation.From =
                NSValue.FromCGPoint(new CGPoint(view.Center.X - shakeThreshold, view.Center.Y));

            animation.To = NSValue.FromCGPoint(new CGPoint(view.Center.X + shakeThreshold, view.Center.Y));
            view.Layer.AddAnimation(animation, "shake");
        }

        public static void ConstrainInView(this UIView self, UIView view)
        {
            self.TopAnchor.ConstraintEqualTo(view.TopAnchor).Active = true;
            self.BottomAnchor.ConstraintEqualTo(view.BottomAnchor).Active = true;
            self.LeadingAnchor.ConstraintEqualTo(view.LeadingAnchor).Active = true;
            self.TrailingAnchor.ConstraintEqualTo(view.TrailingAnchor).Active = true;
        }

        public static void ConstrainToViewSides(this UIView self, UIView view)
        {
            self.LeadingAnchor.ConstraintEqualTo(view.LeadingAnchor).Active = true;
            self.TrailingAnchor.ConstraintEqualTo(view.TrailingAnchor).Active = true;
        }

        public static UIView InsertSeparator(this UIView self, float leftInset = 0)
        {
            var separatorHeight = 1 / UIScreen.MainScreen.Scale;
            var separator = new UIView();
            separator.BackgroundColor = ColorAssets.Table.Separator;
            self.AddSubview(separator);

            separator.TranslatesAutoresizingMaskIntoConstraints = false;
            separator.BottomAnchor.ConstraintEqualTo(self.BottomAnchor).Active = true;
            separator.LeftAnchor.ConstraintEqualTo(self.LeftAnchor, leftInset).Active = true;
            separator.RightAnchor.ConstraintEqualTo(self.RightAnchor).Active = true;
            separator.HeightAnchor.ConstraintEqualTo(separatorHeight).Active = true;

            return separator;
        }
    }
}
