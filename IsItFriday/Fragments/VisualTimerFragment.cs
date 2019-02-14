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

        private LinearLayout _mainLayout;
        private CountdownTimerImpl _countdownTimer;

        private TextView _daysTextView;
        private TextView _hoursTextView;
        private TextView _minutesTextView;
        private TextView _secondsTextView;

        private TextView _daysLabel;
        private TextView _hoursLabel;
        private TextView _minutesLabel;
        private TextView _secondsLabel;

        private TextView _untilFridayLabel;

        private Typeface _typeface;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _typeface = Typeface.CreateFromAsset(Context.Assets, "fonts/LiquidCrystal.otf");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _mainLayout = (LinearLayout)inflater.Inflate(Resource.Layout.VisualTimerFragment, container, false);

            _daysTextView = _mainLayout.FindViewById<TextView>(Resource.Id.DaysTextView);
            _hoursTextView = _mainLayout.FindViewById<TextView>(Resource.Id.HoursTextView);
            _minutesTextView = _mainLayout.FindViewById<TextView>(Resource.Id.MinutesTextView);
            _secondsTextView = _mainLayout.FindViewById<TextView>(Resource.Id.SecondsTextView);

            _daysLabel = _mainLayout.FindViewById<TextView>(Resource.Id.DaysLabel);
            _hoursLabel = _mainLayout.FindViewById<TextView>(Resource.Id.HoursLabel);
            _minutesLabel = _mainLayout.FindViewById<TextView>(Resource.Id.MinutesLabel);
            _secondsLabel = _mainLayout.FindViewById<TextView>(Resource.Id.SecondsLabel);

            _daysTextView.Typeface = _typeface;
            _daysLabel.Typeface = _typeface;

            _untilFridayLabel = _mainLayout.FindViewById<TextView>(Resource.Id.UntilFridayLabel);

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

            UpdateUntilFridayLabel();
            CreateAndStartTimerIfNeeded();
        }

        private void Timer_Tick(object sender, long millisToFriday)
        {
            TimeSpan timeSpanToFriday = TimeSpan.FromMilliseconds(millisToFriday);

            if (timeSpanToFriday.Days == 1)
            {
                _daysLabel.SetText(Resource.String.day);
            }
            else
            {
                _daysLabel.SetText(Resource.String.days);
            }

            if (timeSpanToFriday.Hours == 1)
            {
                _hoursLabel.SetText(Resource.String.hour);
            }
            else
            {
                _hoursLabel.SetText(Resource.String.hours);
            }

            if (timeSpanToFriday.Minutes == 1)
            {
                _minutesLabel.SetText(Resource.String.minute);
            }
            else
            {
                _minutesLabel.SetText(Resource.String.minutes);
            }

            if (timeSpanToFriday.Seconds == 1)
            {
                _secondsLabel.SetText(Resource.String.second);
            }
            else
            {
                _secondsLabel.SetText(Resource.String.seconds);
            }

            _daysTextView.Text = timeSpanToFriday.ToString(@"dd");
            _hoursTextView.Text = timeSpanToFriday.ToString(@"hh");
            _minutesTextView.Text = timeSpanToFriday.ToString(@"mm");
            _secondsTextView.Text = timeSpanToFriday.ToString(@"ss");
        }


        private void Timer_TimerEnded(object sender, EventArgs e)
        {
            UpdateUntilFridayLabel();
        }

        public override void OnPause()
        {
            if (_mainActivity != null)
            {
                _mainActivity.InDarkModeChanged -= Activity_InDarkModeChanged;
            }

            if (_countdownTimer != null)
            {
                _countdownTimer.Cancel();
                _countdownTimer.Tick -= Timer_Tick;
                _countdownTimer.TimerEnded -= Timer_TimerEnded;
            }
            _countdownTimer = null;

            base.OnPause();
        }

        private void SetColorMode(bool inDarkMode)
        {
            if (inDarkMode)
            {
                _mainLayout.SetBackgroundColor(Color.Black);

                _daysTextView.SetTextColor(Color.White);
                _hoursTextView.SetTextColor(Color.White);
                _minutesTextView.SetTextColor(Color.White);
                _secondsTextView.SetTextColor(Color.White);

                _daysLabel.SetTextColor(Color.White);
                _hoursLabel.SetTextColor(Color.White);
                _minutesLabel.SetTextColor(Color.White);
                _secondsLabel.SetTextColor(Color.White);

                _untilFridayLabel.SetTextColor(Color.White);
            }
            else
            {
                _mainLayout.SetBackgroundColor(Color.White);

                _daysTextView.SetTextColor(Color.Black);
                _hoursTextView.SetTextColor(Color.Black);
                _minutesTextView.SetTextColor(Color.Black);
                _secondsTextView.SetTextColor(Color.Black);

                _daysLabel.SetTextColor(Color.Black);
                _hoursLabel.SetTextColor(Color.Black);
                _minutesLabel.SetTextColor(Color.Black);
                _secondsLabel.SetTextColor(Color.Black);

                _untilFridayLabel.SetTextColor(Color.Black);
            }
        }

        private void UpdateUntilFridayLabel()
        {
            if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
            {
                _untilFridayLabel.SetText(Resource.String.untilFridayEnds);
            }
            else
            {
                _untilFridayLabel.SetText(Resource.String.untilFriday);
            }
        }

        private void CreateAndStartTimerIfNeeded()
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
                daysToFriday = DayOfWeek.Friday - DateTime.Today.DayOfWeek;
            }

            if (daysToFriday == 0) // ie it's Friday
            {
                daysToFriday = 1;
            }

            long timeToFriday = (long)(DateTime.Today.AddDays(daysToFriday) - DateTime.Now).TotalMilliseconds;

            _countdownTimer = new CountdownTimerImpl(timeToFriday, 1000);
            _countdownTimer.Tick += Timer_Tick;
            _countdownTimer.TimerEnded += Timer_TimerEnded;
            _countdownTimer.Start();
        }

        private void Activity_InDarkModeChanged(object sender, bool inDarkMode)
        {
            SetColorMode(inDarkMode);
        }
    }
}