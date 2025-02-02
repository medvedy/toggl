using CoreAnimation;
using CoreGraphics;
using System;
using Toggl.iOS.Extensions;
using UIKit;
using Math = Toggl.Shared.Math;

namespace Toggl.iOS.Views.EditDuration.Shapes.Caps
{
    public abstract class Cap : CAShapeLayer
    {
        private readonly CGColor capColor
            = Core.UI.Helper.Colors.EditDuration.Wheel.Cap.ToNativeColor().CGColor;

        // The sizes are relative to the radius of the wheel.
        // The radius of the wheel in the design document is 128 points.
        private readonly nfloat outerRadius = 16.5f / 128f;

        private readonly nfloat innerRadius = 14.05f / 128f;

        public CGColor Color
        {
            set => FillColor = value;
        }

        protected Cap(
            CGImage icon,
            Func<nfloat, nfloat> scale,
            nfloat iconHeight,
            nfloat iconWidth)
        {
            var center = new CGPoint(0, 0);

            var outerPath = new UIBezierPath();
            outerPath.AddArc(center, scale(outerRadius), 0, (nfloat)Math.FullCircle, false);

            Path = outerPath.CGPath;

            var innerPath = new UIBezierPath();
            innerPath.AddArc(center, scale(innerRadius), 0, (nfloat)Math.FullCircle, false);

            var circleLayer = new CAShapeLayer { Path = innerPath.CGPath, FillColor = capColor };

            var imageFrame = new CGRect(
                x: center.X - scale(iconWidth) / 2f,
                y: center.Y - scale(iconHeight) / 2f,
                width: scale(iconWidth),
                height: scale(iconHeight));

            var imageLayer = new CALayer { Contents = icon, Frame = imageFrame };
            circleLayer.AddSublayer(imageLayer);

            AddSublayer(circleLayer);
        }
    }
}
