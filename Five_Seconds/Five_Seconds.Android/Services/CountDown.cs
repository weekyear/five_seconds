﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Five_Seconds.Droid.Services
{
    public class CountDown : CountDownTimer
    {
        public long CountDownInterval { get; }
        public long MillisInFuture { get; }
        public AlarmActivity Activity { get; set; }

        private bool IsCountDown { get; set; }

        public CountDown(long millisInFuture, long countDownInterval, Activity activity, bool isCountDown) : base(millisInFuture, countDownInterval)
        {
            Activity = activity as AlarmActivity;
            MillisInFuture = millisInFuture;
            CountDownInterval = countDownInterval;
            IsCountDown = isCountDown;
        }

        public override async void OnFinish()
        {
            if (IsCountDown)
            {
                await Task.Delay(300);
                Activity.IsFinished = true;
                Activity.FinishAndRemoveTask();
            }
            else
            {
                Activity.ShowFeedbackDialog();
            }
        }

        public override void OnTick(long millisUntilFinished)
        {
            var alarmActivity = Activity as AlarmActivity;
            double count = (double)millisUntilFinished / 1000;

            if (IsCountDown)
            {
                var countTextView = alarmActivity.countTextView;

                var stringFormat = string.Format("{0:f2}", count);
                countTextView.Text = stringFormat;
            }
            else
            {
                var timeOutTextView = alarmActivity.timeOutTextView;

                var stringFormat = $"{count:00}초 이내";
                timeOutTextView.Text = stringFormat;
            }
        }
    }
}