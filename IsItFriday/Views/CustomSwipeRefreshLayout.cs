using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace IsItFriday.Views
{
    public class CustomSwipeRefreshLayout : SwipeRefreshLayout
    {
        private const int MOVEMENT_XY_THRESHOLD = 200;
        private const double MOVEMENT_TIMEOUT_MS = 1000;
        private float _lastY = -1;
        private double _lastMs = 0;

        public CustomSwipeRefreshLayout(Context context) 
            : base(context)
        {
        }

        public CustomSwipeRefreshLayout(Context context, IAttributeSet attrs) 
            : base(context, attrs)
        {
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            Log.Debug(nameof(CustomSwipeRefreshLayout), "on touch event");

            double currentMs = DateTime.Now.TimeOfDay.TotalMilliseconds;
            if (_lastMs == 0)
            {
                _lastMs = currentMs;
            }

            bool timedOut = currentMs - _lastMs > MOVEMENT_TIMEOUT_MS;

            Log.Debug(nameof(CustomSwipeRefreshLayout), "_lastY: " + _lastY + "\nRawY: " + e.RawY);
            if (_lastY == -1)
            {
                _lastY = e.RawY;
            }

            float dY = Math.Abs(e.RawY - _lastY);
            Log.Debug(nameof(CustomSwipeRefreshLayout), "DY: " + dY);

            if (dY <= MOVEMENT_XY_THRESHOLD && timedOut)
            {
                Log.Debug(nameof(CustomSwipeRefreshLayout), "Show next activity");
                Toast.MakeText(Context, "Show next activity", ToastLength.Short).Show();
                Refreshing = false;
                return false;
            }
            //else if (timedOut)
            //{
            //    _lastY = -1;
            //}
            else
            {
                _lastY = e.RawY;
            }

            {
                _lastMs = currentMs;
            }

            Log.Debug(nameof(CustomSwipeRefreshLayout), "Swipe intercepted, resume refresh");

            return base.OnTouchEvent(e);
        }
    }
}