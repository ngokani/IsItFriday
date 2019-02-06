using Android.OS;
using System;

namespace IsItFriday.Tools
{
    public class MidnightTimer : CountDownTimer
    {
        public event EventHandler TimerEnded;

        public MidnightTimer(long millisInFuture)
            : base(millisInFuture, countDownInterval: millisInFuture) // We don't care about the intervals
        {
        }

        public override void OnFinish()
        {
            TimerEnded?.Invoke(this, EventArgs.Empty);
        }

        public override void OnTick(long millisUntilFinished)
        {
            // do nothing
        }
    }
}