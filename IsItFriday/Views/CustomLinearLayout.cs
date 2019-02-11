using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using IsItFriday.Tools;
using static Android.Views.View;

namespace IsItFriday.Views
{
    public class CustomLinearLayout : LinearLayout
    {
        private const int MOVEMENT_XY_THRESHOLD = 200;
        private const double MOVEMENT_TIMEOUT_MS = 1500;
        private float _lastYPos = -1;
        private double _lastMs = 0;

        public Action InterceptedAction { get; set; }

        public CustomLinearLayout(Context context) 
            : base(context)
        {
        }

        public CustomLinearLayout(Context context, IAttributeSet attrs) 
            : base(context, attrs)
        {
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            Log.Debug(nameof(CustomLinearLayout), "---------------BEGIN---------------\non touch event: " + ev.ActionMasked);

            if (InterceptedAction == null)
            {
                Log.Debug(nameof(CustomLinearLayout), "----------------NULL ACTION----------------");
                Log.Debug(nameof(CustomLinearLayout), "----------------END----------------");
                return false;
            }

            if (ev.ActionMasked == MotionEventActions.Up)
            {
                _lastYPos = -1;
                _lastMs = 0;

                Log.Debug(nameof(CustomLinearLayout), "----------------END----------------");
                if (InterceptedAction != null)
                {
                    RemoveCallbacks(InterceptedAction);
                }
                return false;
            }

            Log.Debug(nameof(CustomLinearLayout), "_lastY: " + _lastYPos + "\nRawY: " + ev.RawY);

            double currentMs = DateTime.Now.TimeOfDay.TotalMilliseconds;
            if (_lastMs == 0)
            {
                _lastMs = currentMs;
            }

            if (ev.ActionMasked == MotionEventActions.Move)
            {
                float dY = Math.Abs(ev.RawY - _lastYPos);

                bool shouldNavigateToNextActivity = currentMs - _lastMs > MOVEMENT_TIMEOUT_MS;
                bool withinThreshold = dY <= MOVEMENT_XY_THRESHOLD;
                Log.Debug(nameof(CustomLinearLayout), $"dY: {dY}\nwithin threshold: {withinThreshold}");

                //Log.Debug(nameof(CustomLinearLayout), "Should navigate to next: " + shouldNavigateToNextActivity);

                if (withinThreshold && shouldNavigateToNextActivity)
                {
                    Log.Debug(nameof(CustomLinearLayout), "Show next activity");
                    Log.Debug(nameof(CustomLinearLayout), "----------------END----------------");

                    _lastYPos = -1;
                    _lastMs = 0;
                    if (InterceptedAction != null)
                    {
                        PostDelayed(InterceptedAction, Convert.ToInt64(MOVEMENT_TIMEOUT_MS));
                        return true;
                    }
                }
                else /*if (!withinThreshold)*/
                {
                    _lastMs = currentMs;

                    Log.Debug(nameof(CustomLinearLayout), $"DY too large, reset {nameof(_lastMs)}");
                }
                //else
                //{
                //    Log.Debug(nameof(CustomLinearLayout), $"Resuming refresh. Should navigate: {shouldNavigateToNextActivity}");
                //    Log.Debug(nameof(CustomLinearLayout), $"{nameof(withinThreshold)}: {withinThreshold} - {nameof(shouldNavigateToNextActivity)}: {shouldNavigateToNextActivity}");
                //}
                _lastYPos = ev.RawY;
            }
            else if (ev.ActionMasked == MotionEventActions.Down)
            {
                _lastMs = currentMs;
                _lastYPos = ev.RawY;

                //if (InterceptedAction != null)
                //{
                //    PostDelayed(InterceptedAction, Convert.ToInt64(MOVEMENT_TIMEOUT_MS));
                //    Log.Debug(nameof(CustomLinearLayout), "----------------END----------------");
                //    return true;
                //}
            }

            if(InterceptedAction != null)
            {
                RemoveCallbacks(InterceptedAction);
            }

            Log.Debug(nameof(CustomLinearLayout), "----------------END----------------");
            return false;
        }
    }
}