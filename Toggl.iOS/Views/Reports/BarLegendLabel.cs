﻿using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Reports
{
    internal sealed class BarLegendLabel : UILabel
    {
        public BarLegendLabel(string dayInitial, string shortDate)
        {
            Text = $"{dayInitial}\n{shortDate}";
            TextAlignment = UITextAlignment.Center;
            Lines = 2;
            TextColor = Colors.Reports.BarChart.Legend.ToNativeColor();
            Font = UIFont.SystemFontOfSize(12);
            AdjustsFontSizeToFitWidth = true;
        }
    }
}
