using System;
using Android.OS;

namespace IsItFriday.Tools
{
    public class CountdownTimerImpl : CountDownTimer
    {
        public event EventHandler TimerEnded;
        public event EventHandler<long> Tick;

        public CountdownTimerImpl(long millisInFuture, long countdownInterval)
            : base (millisInFuture, countdownInterval)
        {
        }

        public override void OnFinish()
        {
            TimerEnded?.Invoke(this, EventArgs.Empty);
        }

        public override void OnTick(long millisUntilFinished)
        {
            Tick?.Invoke(this, millisUntilFinished);
        }
    }
}