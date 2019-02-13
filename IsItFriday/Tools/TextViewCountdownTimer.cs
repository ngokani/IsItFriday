using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace IsItFriday.Tools
{
    public class TextViewCountdownTimer : CountDownTimer
    {
        public event EventHandler TimerEnded;
        public event EventHandler<long> Tick;

        public TextViewCountdownTimer(long millisInFuture)
            : base (millisInFuture, 1000)
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