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
        public CustomSwipeRefreshLayout(Context context) 
            : base(context)
        {
        }

        public CustomSwipeRefreshLayout(Context context, IAttributeSet attrs) 
            : base(context, attrs)
        {
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
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