using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Xamarin.Essentials;

namespace Five_Seconds.Droid.Services
{
    public class CountDown : CountDownTimer
    {
        public long CountDownInterval { get; }
        public long MillisInFuture { get; }
        private long countForTTS = 60;
        private long periodForTTS = 12;
        public AlarmActivity Activity { get; }

        private bool IsFiveCountType { get; }
        private bool HasWakeUpText { get; }
        private string WakeUpText { get; }

        private Handler Handler => new Handler();

        public CountDown(long millisInFuture, long countDownInterval, Activity activity, bool isFiveCountType) : base(millisInFuture, countDownInterval)
        {
            Activity = activity as AlarmActivity;
            MillisInFuture = millisInFuture;
            CountDownInterval = countDownInterval;
            IsFiveCountType = isFiveCountType;
            HasWakeUpText = Activity.HasWakeUpText;
            if (HasWakeUpText)
            {
                countForTTS = countForTTS - periodForTTS;
                WakeUpText = Activity.WakeUpText;
                SpeakNowDefaultSettings();
            }
        }

        public override async void OnFinish()
        {
            if (IsFiveCountType)
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
            Action DelayedAction = () => NotifyFaieldAlarmAndFinish();
            var timeSpan = new TimeSpan(0, 1, 0);
            Handler.PostDelayed(DelayedAction, (long)timeSpan.TotalMilliseconds);
        }

        private void NotifyFaieldAlarmAndFinish()
        {
            NotificationAndroid.NotifyFailedAlarm(Activity.alarm, Activity.AlarmTimeNow);
            Activity.FinishAndRemoveTask();
        }

        public async override void OnTick(long millisUntilFinished)
        {
            var alarmActivity = Activity as AlarmActivity;
            double count = (double)millisUntilFinished / 1000;

            if (IsFiveCountType)
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

                if (HasWakeUpText && count < countForTTS)
                {
                    countForTTS = countForTTS - periodForTTS;
                    await SpeakNowDefaultSettings();
                }

                timeOutTextView.Text = stringFormat;
            }
        }

        private async Task SpeakNowDefaultSettings()
        {
            var locales = await TextToSpeech.GetLocalesAsync();

            var locale = locales.FirstOrDefault();

            var settings = new SpeechOptions()
            {
                Volume = .75f,
                Pitch = 1.0f,
                Locale = locale
            };

            await TextToSpeech.SpeakAsync(WakeUpText, settings);

            // This method will block until utterance finishes.
        }
    }
}