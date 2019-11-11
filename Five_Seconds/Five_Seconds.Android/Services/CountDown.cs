using System;
using System.Collections.Generic;
using System.Globalization;
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

        private Handler Handler => new Handler();

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
                if (Activity.CanShowReview)
                {
                    await Task.Delay(1000);
                    Activity.ShowReviewDialog();
                }
                else
                {
                    Activity.FinishAndRemoveTask();
                }
            }
            else
            {
                await Task.Delay(300);
                Activity.ShowResultDialog();
                if (!Activity.IsSuccess)
                {
                    CloseActivityWhenFailedAndTimeOut();
                }
            }
        }

        private void CloseActivityWhenFailedAndTimeOut()
        {
            // 10분 경과
            Action DelayedAction = () => Activity.FinishAndRemoveTask();
            // 1분 경과
            var timeSpan = new TimeSpan(0, 1, 0);
            Handler.PostDelayed(DelayedAction, (long)timeSpan.TotalMilliseconds);
        }

        public override void OnTick(long millisUntilFinished)
        {
            var alarmActivity = Activity as AlarmActivity;
            double count = (double)millisUntilFinished / 1000;

            if (IsCountDown)
            {
                var countTextView = alarmActivity.countTextView;

                var stringFormat = string.Format("{0:f1}", count);
                countTextView.Text = stringFormat;
            }
            else
            {
                var timeOutTextView = alarmActivity.timeOutTextView;

                string stringFormat;

                switch (CultureInfo.CurrentCulture.Name)
                {
                    case "ko-KR":
                        stringFormat = $"{count:00}초 이내";
                        break;
                    case "en-US":
                        stringFormat = $"Within {count:00} sec";
                        break;
                    default:
                        stringFormat = $"Within {count:00} sec";
                        break;
                }

                timeOutTextView.Text = stringFormat;
            }
        }
    }
}