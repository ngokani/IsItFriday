using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using IsItFriday.Tools;
using IsItFriday.Views;

namespace IsItFriday.Fragments
{
    public class MainFragment : Fragment
    {
        private string _itsFridayToastMessage;
        private string _itsNotFridayToastMessage;

        private TextView _isItFridayTextView;
        private CustomSwipeRefreshLayout _swipeRefreshLayout;

        public new MainActivity Activity => base.Activity as MainActivity;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _itsFridayToastMessage = Resources.GetString(Resource.String.itsFridayToast);
            _itsNotFridayToastMessage = Resources.GetString(Resource.String.itsNotFridayToast);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.MainFragment, container, false);
            _swipeRefreshLayout = view.FindViewById<CustomSwipeRefreshLayout>(Resource.Id.SwipeRefreshLayout);
            _isItFridayTextView = view.FindViewById<TextView>(Resource.Id.IsItFridayTextView);

            _swipeRefreshLayout.Enabled = false;
            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
            UpdateTextView();
            SetColorMode(Activity.InDarkMode);

            //_swipeRefreshLayout.Refresh += RefreshLayout_OnRefresh;
            //_swipeRefreshLayout.Touch += SwipeRefreshLayout_Touch;

            if (Activity != null)
            {
                Activity.InDarkModeChanged += Activity_InDarkModeChanged;
                Activity.MidnightTimerEnded += Activity_MidnightTimerEnded;
            }
        }

        public override void OnPause()
        {
            _swipeRefreshLayout.Touch -= SwipeRefreshLayout_Touch;
            _swipeRefreshLayout.Refresh -= RefreshLayout_OnRefresh;
            if (Activity != null)
            {
                Activity.InDarkModeChanged -= Activity_InDarkModeChanged;
                Activity.MidnightTimerEnded -= Activity_MidnightTimerEnded;
            }
            base.OnPause();
        }

        private void RefreshLayout_OnRefresh(object sender, EventArgs e)
        {
            _swipeRefreshLayout.Refreshing = false;

            string message = DateTime.Today.DayOfWeek == DayOfWeek.Friday
                ? _itsFridayToastMessage
                : _itsNotFridayToastMessage;

            UpdateTextView();
            
            Activity.CreateAndShowToast(message, ToastLength.Long);
        }

        private void SwipeRefreshLayout_Touch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Move)
            {
                _swipeRefreshLayout.Enabled = true;
            }
            else
            {
                _swipeRefreshLayout.Enabled = false;
            }
        }

        private void UpdateTextView()
        {
            if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
            {
                _isItFridayTextView.SetText(Resource.String.yes);
            }
            else
            {
                _isItFridayTextView.SetText(Resource.String.no);
            }
        }

        private void SetColorMode(bool setDarkMode)
        {
            if (_isItFridayTextView == null) return;

            if (setDarkMode)
            {
                _isItFridayTextView.SetTextColor(Color.White);
                ((ViewGroup)_isItFridayTextView.Parent).SetBackgroundColor(Color.Black);
            }
            else
            {
                _isItFridayTextView.SetTextColor(Color.Black);
                ((ViewGroup)_isItFridayTextView.Parent).SetBackgroundColor(Color.White);
            }
        }

        private void Activity_MidnightTimerEnded(object sender, EventArgs e)
        {
            UpdateTextView();
        }

        private void Activity_InDarkModeChanged(object sender, bool inDarkMode)
        {
            SetColorMode(inDarkMode);
        }
    }
}