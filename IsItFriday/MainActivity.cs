using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace IsItFriday
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            TextView isItFridayTextView = FindViewById<TextView>(Resource.Id.IsItFridayTextView);
            isItFridayTextView.Text = DateTime.Today.DayOfWeek == DayOfWeek.Friday ? "Yes" : "No";
        }
	}
}

