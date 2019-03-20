using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;

namespace IsItFriday.Views
{
    public class CustomSwipeRefreshLayout : SwipeRefreshLayout
    {
        public event EventHandler LongPress;
        private double _deltaYThreshold;
        private double _deltaXThreshold;
        private float _lastXPos = -1;
        private float _lastYPos = -1;


        public CustomSwipeRefreshLayout(Context context)
            : base(context)
        {
        }

        public CustomSwipeRefreshLayout(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            Activity currentActivity = GetActivity();

            if (currentActivity != null)
            {
                DisplayMetrics displayMetrics = new DisplayMetrics();
                currentActivity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
                _deltaXThreshold = displayMetrics.WidthPixels * 0.08;
                _deltaYThreshold = displayMetrics.HeightPixels * 0.08;
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.ActionMasked == MotionEventActions.Down)
            {
                _lastXPos = e.RawX;
                _lastYPos = e.RawY;

                PostDelayed(OnLongPressed, ViewConfiguration.LongPressTimeout);
            }
            else if (e.ActionMasked == MotionEventActions.Up)
            {
                RemoveCallbacks(OnLongPressed);
            }
            else if (e.ActionMasked == MotionEventActions.Move)
            {
                float dX = Java.Lang.Math.Abs(e.RawX - _lastXPos);
                float dY = Java.Lang.Math.Abs(e.RawY - _lastYPos);

                bool withinThreshold = dX <= _deltaXThreshold && dY <= _deltaYThreshold;

                if (!withinThreshold)
                {
                    RemoveCallbacks(OnLongPressed);
                    Toast.MakeText(Context, "out of threshold", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(Context, "Within threshold", ToastLength.Short).Show();
                }
            }

            return base.OnTouchEvent(e);
        }

        private void OnLongPressed()
        {
            LongPress?.Invoke(this, EventArgs.Empty);
            Random r = new Random();
            Color c = new Color(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
            this.SetBackgroundColor(c);
        }

        private Activity GetActivity()
        {
            Context context = Context;
            while (context is ContextWrapper contextWrapper)
            {
                if (contextWrapper is Activity activity)
                {
                    return activity;
                }

                context = contextWrapper.BaseContext;
            }

            return null;
        }
    }
}