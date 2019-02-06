using System;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using IsItFriday.Fragments;
using IsItFriday.Tools;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace IsItFriday
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : FragmentActivity, ISensorEventListener2
    {
        #region ISensorEventListener2 global variables
        private const int FORCE_THRESHOLD = 3050;
        private const int TIME_THRESHOLD = 100;
        private const int SHAKE_TIMEOUT = 500;
        private const int SHAKE_DURATION = 1500;
        private const int MAX_SHAKE_COUNT = 6;
        private DateTime _lastTime = DateTime.UtcNow;
        private DateTime _lastShake = DateTime.UtcNow;
        private DateTime _lastForce = DateTime.UtcNow;
        private float _lastX = 0;
        private float _lastY = 0;
        private float _lastZ = 0;
        private int _shakeCount = 0;
        #endregion ISensorEventListener2 global variables

        private static readonly int NOTIFICATION_ID = 7561;
        private static readonly string NOTIFICATION_CHANNEL_ID = "friday_notification";

        private MidnightTimer _nextDayCountdownTimer;
        private SensorManager _sensorManager;
        private Sensor _accelerometer;

        public event EventHandler MidnightTimerEnded;
        public event EventHandler<bool> InDarkModeChanged;
        public event EventHandler<bool> IsThursdayOrFridayChanged;

        private bool _isThursdayOrFriday;
        public bool IsThursdayOrFriday
        {
            get => _isThursdayOrFriday;
            private set
            {
                if (_isThursdayOrFriday != value)
                {
                    _isThursdayOrFriday = value;
                    IsThursdayOrFridayChanged?.Invoke(this, _isThursdayOrFriday);
                }
            }
        }

        private bool _inDarkMode;
        public bool InDarkMode
        {
            get => _inDarkMode;
            private set
            {
                if (_inDarkMode != value)
                {
                    _inDarkMode = value;
                    InDarkModeChanged?.Invoke(this, _inDarkMode);
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainActivity);

            ISharedPreferences settings = ApplicationContext.GetSharedPreferences(PackageName, FileCreationMode.Private);

            // To be remove in the next version
            InDarkMode = settings.GetBoolean("_inDarkMode", false);
            InDarkMode = InDarkMode || settings.GetBoolean(nameof(InDarkMode), false);
            CreateNotificationChannel();
        }

        protected override void OnStart()
        {
            base.OnStart();
            _sensorManager = (SensorManager)GetSystemService(Context.SensorService);
            if (_sensorManager != null)
            {
                _accelerometer = _sensorManager.GetDefaultSensor(SensorType.Accelerometer);
            }

            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.FragmentContainer, new MainFragment());
            transaction.AddToBackStack(null);
            transaction.Commit();
        }

        protected override void OnResume()
        {
            base.OnResume();

            IsThursdayOrFriday = DateTime.Today.DayOfWeek == DayOfWeek.Thursday || DateTime.Today.DayOfWeek == DayOfWeek.Friday;
            if (IsThursdayOrFriday && _nextDayCountdownTimer == null)
            {
                long timeToTomorrow = (long)(DateTime.Today.AddDays(1) - DateTime.Now).TotalMilliseconds;
                _nextDayCountdownTimer = new MidnightTimer(timeToTomorrow);
                _nextDayCountdownTimer.TimerEnded += CountdownTimer_TimerEnded;
                _nextDayCountdownTimer.Start();
            }

            if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
            {
                MidnightTimer newTimer = new MidnightTimer(10000);
                newTimer.TimerEnded += (s, e) =>
                {
                    CreateNotification();
                };

                newTimer.Start();
            }

            if (_accelerometer != null)
            {
                _sensorManager.RegisterListener(this, _accelerometer, SensorDelay.Game);
            }

            SupportFragmentManager.BackStackChanged += SupportFragmentManager_BackStackChanged;
        }

        protected override void OnPause()
        {
            _sensorManager?.UnregisterListener(this);
            _sensorManager = null;
            _accelerometer = null;

            if (IsThursdayOrFriday)
            {
                _nextDayCountdownTimer?.Cancel();
                _nextDayCountdownTimer = null;
            }

            SupportFragmentManager.BackStackChanged -= SupportFragmentManager_BackStackChanged;
            base.OnPause();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            ISharedPreferences settings = ApplicationContext.GetSharedPreferences(PackageName, FileCreationMode.Private);
            ISharedPreferencesEditor editor = settings.Edit();
            editor.PutBoolean(nameof(InDarkMode), InDarkMode);
            editor.Apply();
            base.OnSaveInstanceState(outState);
        }

        private void CountdownTimer_TimerEnded(object sender, EventArgs e)
        {
            MidnightTimerEnded?.Invoke(sender, e);
        }

        private void SupportFragmentManager_BackStackChanged(object sender, EventArgs e)
        {
            Toast.MakeText(this, "PANIC! Something changed on the backstack!!", ToastLength.Short).Show();
        }

        private void CreateNotification()
        {
            string id = "kfVsfOSbJY0";
            var resultIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("vnd.youtube:" + id));

            PendingIntent pendingIntent = PendingIntent.GetActivity(this, NOTIFICATION_ID, resultIntent, PendingIntentFlags.UpdateCurrent);
            NotificationCompat.Builder notifcationBuilder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID)
                .SetSmallIcon(Resource.Drawable.ic_stat_iif)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetContentTitle("It's Friday Friday Friday....")
                .SetContentText("Today is Friday! Tap here for a treat")
                .SetPriority(NotificationCompat.PriorityDefault);

            var notificationManager = NotificationManagerCompat.From(this);
            try
            {
                notificationManager.Notify(NOTIFICATION_ID, notifcationBuilder.Build());
            }
            finally
            {
                // do nothing
            }
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                string name = GetString(Resource.String.channelName);
                string description = GetString(Resource.String.channelDescription);
                NotificationChannel channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, name, NotificationImportance.Default);
                channel.Description = description;

                NotificationManager notificationManager = (NotificationManager)GetSystemService(Activity.NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }

        private void ToggleDarkMode() => InDarkMode = !InDarkMode;

        #region ISensorEventListener2 Implementation
        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type != SensorType.Accelerometer)
                return;

            DateTime now = DateTime.UtcNow;
            if ((now - _lastForce).TotalMilliseconds > SHAKE_TIMEOUT)
            {
                _shakeCount = 0;
            }


            if ((now - _lastTime).TotalMilliseconds > TIME_THRESHOLD)
            {
                float diffTime = (float)(now - _lastTime).TotalMilliseconds;
                float x = e.Values[0];
                float y = e.Values[1];
                float z = e.Values[2];

                float speed = System.Math.Abs(x + y + z - _lastX - _lastY - _lastZ) / diffTime * 10000;

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

        public void OnFlushCompleted(Sensor sensor) { }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy) { }
        #endregion ISensorEventListener2 Implementation
    }
}

