using Android.OS;
using System;

namespace IsItFriday
{
    public class NextDayCountdownTimer : CountDownTimer
    {
        public event EventHandler TimerEnded;

        public NextDayCountdownTimer(long millisInFuture)
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