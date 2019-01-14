using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace IsItFriday
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ISensorEventListener2
    {
        private FridayCountdownTimer _fridayCountdownTimer;
        private TextView _isItFridayTextView;
        private SensorManager _sensorManager;
        private Sensor _accelerometer;
        private bool _isThursday;
        private bool _inDarkMode = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            
            _isItFridayTextView = FindViewById<TextView>(Resource.Id.IsItFridayTextView);
            ISharedPreferences settings = ApplicationContext.GetSharedPreferences("uk.co.technikhil.isitfriday", FileCreationMode.Private);
            _inDarkMode = !settings.GetBoolean(nameof(_inDarkMode), true);
            ToggleDarkMode();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            ISharedPreferences settings = ApplicationContext.GetSharedPreferences("uk.co.technikhil.isitfriday", FileCreationMode.Private);
            ISharedPreferencesEditor editor = settings.Edit();
            editor.PutBoolean(nameof(_inDarkMode), _inDarkMode);
            editor.Apply();
            base.OnSaveInstanceState(outState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            ISharedPreferences settings = ApplicationContext.GetSharedPreferences("uk.co.technikhil.isitfriday", FileCreationMode.Private);
            _inDarkMode = !settings.GetBoolean(nameof(_inDarkMode), true);
            ToggleDarkMode();
        }

        protected override void OnResume()
        {
            base.OnResume();
            UpdateTextView();
            _isThursday = DateTime.Today.DayOfWeek == DayOfWeek.Thursday;

            //if (_isThursday && _fridayCountdownTimer == null)
            //{
            //    long timeToFridayMs = (long)(DateTime.Today.AddDays(1) - DateTime.Now).TotalMilliseconds;
            //    _fridayCountdownTimer = new FridayCountdownTimer(timeToFridayMs);
            //    _fridayCountdownTimer.TimerEnded += CountdownTimer_TimerEnded;
            //    _fridayCountdownTimer.Start();
            //}

            _sensorManager = (SensorManager)GetSystemService(Context.SensorService);
            if (_sensorManager != null)
            {
                _accelerometer = _sensorManager.GetDefaultSensor(SensorType.Accelerometer);
                if (_accelerometer != null)
                {
                    _sensorManager.RegisterListener(this, _accelerometer, SensorDelay.Game);
                }
            }
        }

        protected override void OnPause()
        {
            if (_isThursday)
            {
                _fridayCountdownTimer?.Cancel();
                _fridayCountdownTimer = null;
            }

            _sensorManager?.UnregisterListener(this);
            _sensorManager = null;
            _accelerometer = null;

            base.OnPause();
        }

        private void CountdownTimer_TimerEnded(object sender, EventArgs e)
        {
            UpdateTextView();
        }

        private void UpdateTextView()
        {
            _isItFridayTextView.Text = DateTime.Today.DayOfWeek == DayOfWeek.Friday ? "Yes" : "No";
        }

        private void ToggleDarkMode()
        {
            if (_inDarkMode)
            {
                _isItFridayTextView.SetTextColor(Color.White);
                ((ViewGroup)_isItFridayTextView.Parent).SetBackgroundColor(Color.Black);
                _inDarkMode = false;
            }
            else
            {
                _isItFridayTextView.SetTextColor(Color.Black);
                ((ViewGroup)_isItFridayTextView.Parent).SetBackgroundColor(Color.White);
                _inDarkMode = true;
            }
        }

        #region ISensorEventListener2 Implementation
        public void OnFlushCompleted(Sensor sensor)
        {
            // not used
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            // don't care
        }
        private DateTime _lastTime = DateTime.UtcNow;
        private DateTime _lastShake = DateTime.UtcNow;
        private DateTime _lastForce = DateTime.UtcNow;
        private float _lastX = 0;
        private float _lastY = 0;
        private float _lastZ = 0;
        private int _shakeCount = 0;
        private const int FORCE_THRESHOLD = 3050;
        private const int TIME_THRESHOLD = 100;
        private const int SHAKE_TIMEOUT = 500;
        private const int SHAKE_DURATION = 1500;
        private const int MAX_SHAKE_COUNT = 6;

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type != SensorType.Accelerometer)
                return;

            DateTime now = DateTime.UtcNow;
            if ((now - _lastForce).TotalMilliseconds > SHAKE_TIMEOUT)
            {
                _shakeCount = 0;
            }


            if ((now - _lastTime ).TotalMilliseconds> TIME_THRESHOLD)
            {
                float diffTime = (float)(now - _lastTime).TotalMilliseconds;
                float x = e.Values[0];
                float y = e.Values[1];
                float z = e.Values[2];

                float speed = Math.Abs(x + y + z - _lastX - _lastY - _lastZ) / diffTime * 10000;

                if (speed > FORCE_THRESHOLD)
                {
                    if (++_shakeCount >= MAX_SHAKE_COUNT && ((now - _lastShake).TotalMilliseconds > SHAKE_DURATION))
                    {
                        _shakeCount = 0;
                    }
                    else if (_shakeCount == 1)
                    {
                        ToggleDarkMode();
                    }
                    _lastForce = now;
                }
                _lastTime = now;

                _lastX = x;
                _lastY = y;
                _lastZ = z;
            }
        }
        #endregion ISensorEventListener2 Implementation
    }
}

