using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace IsItFriday.Fragments
{
    public class VisualTimerFragment : Fragment
    {
        public const string TAG = nameof(VisualTimerFragment) + "TAG";

        private RelativeLayout _mainLayout;
        private TextView _tickingTextView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _mainLayout = new RelativeLayout(Context);
            _mainLayout.SetBackgroundColor(Color.White);
            _tickingTextView = new TextView(Context);
            _mainLayout.AddView(_tickingTextView);
            _tickingTextView.Gravity = GravityFlags.Center;
            _tickingTextView.Text = "hello world";
            _tickingTextView.SetTextColor(Color.Blue);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);s

            return _mainLayout;
        }


    }
}