using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using IsItFriday.Fragments;
using IsItFriday.Tools;
using IsItFriday.Views;
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

        private const int NOTIFICATION_ID = 7561;
        private const string NOTIFICATION_CHANNEL_ID = "friday_notification";

        private bool _timerFragmentAddedToBackStack;
        private Vibrator _vibrator;
        private CustomLinearLayout _rootView;
        private CountdownTimerImpl _midnightTimer;
        private SensorManager _sensorManager;
        private Sensor _accelerometer;

        public event EventHandler MidnightTimerEnded;
        public event EventHandler<bool> InDarkModeChanged;

        public bool IsThursdayOrFriday => DateTime.Today.DayOfWeek == DayOfWeek.Thursday || DateTime.Today.DayOfWeek == DayOfWeek.Friday;

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

        public Toast CurrentToast { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainActivity);
            _rootView = FindViewById<CustomLinearLayout>(Resource.Id.RootView);
            _vibrator = GetSystemService(Context.VibratorService) as Vibrator;

            ISharedPreferences settings = ApplicationContext.GetSharedPreferences(PackageName, FileCreationMode.Private);

            if (settings.Contains(nameof(InDarkMode)))
            {
                InDarkMode = settings.GetBoolean(nameof(InDarkMode), false);
            }
            else if (settings.Contains(nameof(_inDarkMode))) // To be removed in the next version
            {
                InDarkMode = settings.GetBoolean(nameof(_inDarkMode), false);
            }

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
            transaction.Commit();
        }

        protected override void OnResume()
        {
            base.OnResume();
            CreateAndStartMidnightTimerIfNeeded();

            if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
            {
                CountdownTimerImpl newTimer = new CountdownTimerImpl(10000, 10000);
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

            _rootView.Touch += RootView_Touch;
        }

        private void RootView_Touch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.ActionMasked == MotionEventActions.Up)
            {
                SupportFragmentManager.PopBackStack(nameof(VisualTimerFragment), Android.Support.V4.App.FragmentManager.PopBackStackInclusive);
                _timerFragmentAddedToBackStack = false;
                e.Handled = false;
            }
            else if (!_timerFragmentAddedToBackStack)
            {
                FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
                transaction.Replace(Resource.Id.FragmentContainer, new VisualTimerFragment());
                transaction.AddToBackStack(nameof(VisualTimerFragment));
                transaction.Commit();
                Vibrate();
                e.Handled = true;
                _timerFragmentAddedToBackStack = true;
            }
        }

        protected override void OnPause()
        {
            _sensorManager?.UnregisterListener(this);
            _sensorManager = null;
            _accelerometer = null;

            CurrentToast?.Cancel();
            CurrentToast = null;

            _midnightTimer?.Cancel();
            _midnightTimer = null;

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

        public void CreateAndShowToast(string message, ToastLength toastLength)
        {
            Toast toast = Toast.MakeText(this, message, toastLength);
            CurrentToast?.Cancel();

            CurrentToast = toast;
            toast.Show();
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
                .SetContentTitle(Resources.GetString(Resource.String.fridayNotificationTitle))
                .SetContentText(Resources.GetString(Resource.String.fridayNotificationContent))
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

        public void Vibrate()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Vibrate) != Permission.Granted)
            {
                return;
            }

            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                _vibrator?.Vibrate(VibrationEffect.CreateOneShot(200, VibrationEffect.DefaultAmplitude));
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete
                _vibrator?.Vibrate(200);
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }

        /// <summary>
        /// If it's Thursday or Friday, we need to notify ourselves when we get to midnight
        /// </summary>
        private void CreateAndStartMidnightTimerIfNeeded()
        {
            if (IsThursdayOrFriday && _midnightTimer == null)
            {
                long timeToTomorrow = (long)(DateTime.Today.AddDays(1) - DateTime.Now).TotalMilliseconds;
                _midnightTimer = new CountdownTimerImpl(timeToTomorrow, timeToTomorrow);
                _midnightTimer.TimerEnded += CountdownTimer_TimerEnded;
                _midnightTimer.Start();
            }
        }

        private void CountdownTimer_TimerEnded(object sender, EventArgs e)
        {
            _midnightTimer.TimerEnded -= CountdownTimer_TimerEnded;
            _midnightTimer = null;

            // Timer may have ended, but today could be Friday.
            // May need to start another timer.
            CreateAndStartMidnightTimerIfNeeded();

            MidnightTimerEnded?.Invoke(sender, e);
        }

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

                float speed = Java.Lang.Math.Abs(x + y + z - _lastX - _lastY - _lastZ) / diffTime * 10000;

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

