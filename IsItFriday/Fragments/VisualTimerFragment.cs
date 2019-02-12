using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace IsItFriday.Fragments
{
    public class VisualTimerFragment : Fragment
    {
        private MainActivity _mainActivity;
        private RelativeLayout _mainLayout;
        private TextView _tickingTextView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _mainLayout = new RelativeLayout(Context);
            _mainLayout.SetBackgroundColor(Color.White);

            _tickingTextView = new TextView(Context);
            _mainLayout.AddView(_tickingTextView);

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
        }

        public override void OnPause()
        {
            if (_mainActivity != null)
            {
                _mainActivity.InDarkModeChanged -= Activity_InDarkModeChanged;
            }

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

        private void Activity_InDarkModeChanged(object sender, bool inDarkMode)
        {
            SetColorMode(inDarkMode);
        }
    }
}