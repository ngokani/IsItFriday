using System;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Math = Java.Lang.Math;

namespace IsItFriday.Views
{
    public class CustomLinearLayout : LinearLayout
    {
        private const double MOVEMENT_TIMEOUT_MS = 1500;

        private WeakReference<Activity> _currentActivityWeakReference;
        private double _deltaYThreshold;
        private double _lastMs = -1;
        private float _lastYPos = -1;
        private bool _childHandlingEvent = false;

        public CustomLinearLayout(Context context) 
            : base(context)
        {
        }

        public CustomLinearLayout(Context context, IAttributeSet attrs) 
            : base(context, attrs)
        {
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            Activity currentActivity = null;
            if (_currentActivityWeakReference?.TryGetTarget(out currentActivity) != true)
            {
                currentActivity = GetActivity();
                _currentActivityWeakReference = new WeakReference<Activity>(currentActivity);
            }

            if (currentActivity != null)
            {
                DisplayMetrics displayMetrics = new DisplayMetrics();
                currentActivity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
                _deltaYThreshold = displayMetrics.HeightPixels * 0.1;
            }
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (ev.ActionMasked == MotionEventActions.Up)
            {
                // Quick escape
                _childHandlingEvent = false;
                return false;
            }

            double currentMs = DateTime.Now.TimeOfDay.TotalMilliseconds;

            if (ev.ActionMasked == MotionEventActions.Move)
            {
                float dY = Math.Abs(ev.RawY - _lastYPos);

                bool withinThreshold = dY <= _deltaYThreshold;
                bool movementTimedOut = currentMs - _lastMs > MOVEMENT_TIMEOUT_MS;

                if (withinThreshold && movementTimedOut && !_childHandlingEvent)
                {
                    _lastYPos = -1;
                    _lastMs = -1;
                    return true;
                }
                else if (!withinThreshold)
                {
                    _lastMs = currentMs;
                    _lastYPos = ev.RawY;
                    _childHandlingEvent = true;
                }
            }
            else if (ev.ActionMasked == MotionEventActions.Down)
            {
                _lastMs = currentMs;
                _lastYPos = ev.RawY;
            }

            return false;
        }

        

        private Activity GetActivity()
        {
            Context context = Context;
            while (context is ContextWrapper contextWrapper)
            {
                if(contextWrapper is Activity activity)
                {
                    return activity;
                }

                context = contextWrapper.BaseContext;
            }

            return null;
        }
    }
}