using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace IsItFriday.Helpers
{
    public static class DisplayHelper
    {
        // Adapted from https://stackoverflow.com/a/17410076
        public static float ConvertDpToPixel(this Context context, int dp)
        {
            DisplayMetrics metrics = context.Resources.DisplayMetrics;
            float px = 0;
            //px = TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, resources.DisplayMetrics);
            px = dp * ((float)metrics.DensityDpi / (float)DisplayMetricsDensity.Default);
            return px;
        }

        public static float ConvertPixelToDp(this Context context, int pixel)
        {
            DisplayMetrics metrics = context.Resources.DisplayMetrics;
            float dp = pixel / ((float)metrics.DensityDpi / (float)DisplayMetricsDensity.Default);
            return dp;
        }
    }
}