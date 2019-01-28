using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace IsItFriday
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ISensorEventListener2
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

        private bool _isThursdayOrFriday;
        private bool _inDarkMode;
        private string _packageName;
        private string _itsFridayToastMessage;
        private string _itsNotFridayToastMessage;

        private NextDayCountdownTimer _nextDayCountdownTimer;
        private TextView _isItFridayTextView;
        private SensorManager _sensorManager;
        private Sensor _accelerometer;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private Toast _currentToast;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            _packageName = ApplicationContext.PackageName;

            _itsFridayToastMessage = Resources.GetString(Resource.String.itsFridayToast);
            _itsNotFridayToastMessage = Resources.GetString(Resource.String.itsNotFridayToast);

            _swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.SwipeRefreshLayout);
            _isItFridayTextView = FindViewById<TextView>(Resource.Id.IsItFridayTextView);
            ISharedPreferences settings = ApplicationContext.GetSharedPreferences(_packageName, FileCreationMode.Private);
            _inDarkMode = !settings.GetBoolean(nameof(_inDarkMode), true);
            ToggleDarkMode();

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
        }

        protected override void OnResume()
        {
            base.OnResume();
            UpdateTextView();

            _swipeRefreshLayout.Refresh += RefreshLayout_OnRefresh;

            _isThursdayOrFriday = DateTime.Today.DayOfWeek == DayOfWeek.Thursday || DateTime.Today.DayOfWeek == DayOfWeek.Friday;
            if (_isThursdayOrFriday && _nextDayCountdownTimer == null)
            {
                long timeToTomorrow = (long)(DateTime.Today.AddDays(1) - DateTime.Now).TotalMilliseconds;
                _nextDayCountdownTimer = new NextDayCountdownTimer(timeToTomorrow);
                _nextDayCountdownTimer.TimerEnded += CountdownTimer_TimerEnded;
                _nextDayCountdownTimer.Start();
            }

            if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
            {
                NextDayCountdownTimer newTimer = new NextDayCountdownTimer(10000);
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
        }

        protected override void OnPause()
        {
            if (_isThursdayOrFriday)
            {
                _nextDayCountdownTimer?.Cancel();
                _nextDayCountdownTimer = null;
            }

            _sensorManager?.UnregisterListener(this);
            _sensorManager = null;
            _accelerometer = null;

            _currentToast?.Cancel();

            _swipeRefreshLayout.Refresh -= RefreshLayout_OnRefresh;
            base.OnPause();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            ISharedPreferences settings = ApplicationContext.GetSharedPreferences(_packageName, FileCreationMode.Private);
            ISharedPreferencesEditor editor = settings.Edit();
            editor.PutBoolean(nameof(_inDarkMode), _inDarkMode);
            editor.Apply();
            base.OnSaveInstanceState(outState);
        }

        private void RefreshLayout_OnRefresh(object sender, EventArgs e)
        {
            _swipeRefreshLayout.Refreshing = false;

            string message = DateTime.Today.DayOfWeek == DayOfWeek.Friday
                ? _itsFridayToastMessage
                : _itsNotFridayToastMessage;

            _currentToast?.Cancel();
            _currentToast = Toast.MakeText(this, message, ToastLength.Long);
            _currentToast.Show();
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

        private void CountdownTimer_TimerEnded(object sender, EventArgs e) => UpdateTextView();

        private void CreateNotification()
        {
            string id = "kfVsfOSbJY0";
            var resultIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("vnd.youtube:" + id));

            PendingIntent pendingIntent = PendingIntent.GetActivity(this, NOTIFICATION_ID, resultIntent, PendingIntentFlags.UpdateCurrent);
            NotificationCompat.Builder notifcationBuilder = new NotificationCompat.Builder(this, CHANNEL_ID)
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
            }
        }

        private static readonly int NOTIFICATION_ID = 7561;
        private static readonly string CHANNEL_ID = "friday_notification";

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                string name = GetString(Resource.String.channelName);
                string description = GetString(Resource.String.channelDescription);
                NotificationChannel channel = new NotificationChannel(CHANNEL_ID, name, NotificationImportance.Default);
                channel.Description = description;

                NotificationManager notificationManager = (NotificationManager)GetSystemService(Activity.NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
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


            if ((now - _lastTime).TotalMilliseconds> TIME_THRESHOLD)
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

