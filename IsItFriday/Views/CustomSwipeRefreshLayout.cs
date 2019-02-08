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
        private const int MOVEMENT_XY_THRESHOLD = 1;
        private const double MOVEMENT_TIMEOUT_MS = 1500;

        public CustomSwipeRefreshLayout(Context context) 
            : base(context)
        {
        }

        public CustomSwipeRefreshLayout(Context context, IAttributeSet attrs) 
            : base(context, attrs)
        {
        }

        private float _lastX = -1;
        private float _lastY = -1;
        private double _lastMs = 0;
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            double currentMs = DateTime.Today.TimeOfDay.TotalMilliseconds;
            if (currentMs - _lastMs > MOVEMENT_TIMEOUT_MS)
            {
                //Timed out
                _lastX = -1;
                _lastY = -1;
                _lastMs = currentMs;
                return false;
            }

            float currentX = ev.RawX;
            float currentY = ev.RawY;
            var t = DateTime.Today.TimeOfDay.TotalMilliseconds;

            if (ev.ActionMasked != MotionEventActions.Down)
            {
                return base.OnInterceptTouchEvent(ev);
            }
            else
            {
                return false;
            }
        }
    }
}