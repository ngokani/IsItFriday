using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using IsItFriday.Tools;
using System;

namespace IsItFriday.Fragments
{
    public class VisualTimerFragment : Fragment
    {
        private MainActivity _mainActivity;
        private RelativeLayout _mainLayout;
        private TextView _tickingTextView;
        private TextViewCountdownTimer _countdownTimer;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _mainLayout = new RelativeLayout(Context);
            _mainLayout.SetBackgroundColor(Color.White);

            _tickingTextView = new TextView(Context);
            _mainLayout.AddView(_tickingTextView);

            if (_tickingTextView.LayoutParameters is RelativeLayout.LayoutParams textViewLayoutParams)
            {
                textViewLayoutParams.AddRule(LayoutRules.CenterInParent);
            }

            _tickingTextView.Text = "hello world";
            _tickingTextView.SetTextSize(Android.Util.ComplexUnitType.Sp, 20);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return _mainLayout;
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            _mainActivity = context as MainActivity;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (_mainActivity != null)
            {
                SetColorMode(_mainActivity.InDarkMode);
                _mainActivity.InDarkModeChanged += Activity_InDarkModeChanged;
            }

            CreateTimerIfNeeded();
            _countdownTimer.Tick += Timer_Tick;
            _countdownTimer.Start();
        }

        private void Timer_Tick(object sender, long millisToFriday)
        {
            _tickingTextView.Text = TimeSpan.FromMilliseconds(millisToFriday).ToString(@"hh\:mm\:ss");
        }

        public override void OnPause()
        {
            if (_mainActivity != null)
            {
                _mainActivity.InDarkModeChanged -= Activity_InDarkModeChanged;
            }

            _countdownTimer?.Cancel();
            _countdownTimer.Tick -= Timer_Tick;
            _countdownTimer = null;

            base.OnPause();
        }

        private void SetColorMode(bool inDarkMode)
        {
            if (inDarkMode)
            {

            }
            else
            {

            }
        }

        private void CreateTimerIfNeeded()
        {
            if (_countdownTimer != null)
                return;

            int daysToFriday = 0;
            if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
            {
                daysToFriday = 7;
            }
            else
            {
                daysToFriday = DateTime.Today.DayOfWeek - DayOfWeek.Friday;
            }

            long timeToFriday = (long)(DateTime.Today.AddDays(daysToFriday) - DateTime.Now).TotalMilliseconds;

            _countdownTimer = new TextViewCountdownTimer(timeToFriday);
        }

        private void Activity_InDarkModeChanged(object sender, bool inDarkMode)
        {
            SetColorMode(inDarkMode);
        }
    }
}