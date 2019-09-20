using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Speech;
using Android.Util;
using Android.Views;
using Android.Widget;
using Five_Seconds.Droid.Services;
using Five_Seconds.Models;
using Five_Seconds.Services;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Button = Android.Widget.Button;

namespace Five_Seconds.Droid
{
    [Activity(Label = "AlarmActivity", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AlarmActivity : Activity
    {
        IPlaySoundService _soundService = new PlaySoundServiceAndroid();
        Vibrator _vibrator;

        private LinearLayout alarmLayout;
        private Button tellmeButton;
        public TextView countTextView;
        private TextView alarmTextView;
        private EditText alarmEditText;
        private CountDown countDown;

        private int id;
        private string name;
        private string toneName;
        private bool isAlarmOn;
        private bool isVibrateOn;
        private bool isCountOn;
        private bool isRepeating;
        private int alarmVolume;

        public AlarmActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle bundle = Intent.Extras;
            id = (int)bundle.Get("id");
            name = (string)bundle.Get("name");
            toneName = (string)bundle.Get("toneName");
            isAlarmOn = (bool)bundle.Get("isAlarmOn");
            isVibrateOn = (bool)bundle.Get("isVibrateOn");
            isCountOn = (bool)bundle.Get("isCountOn");
            isRepeating = (bool)bundle.Get("isRepeating");
            alarmVolume = (int)bundle.Get("alarmVolume");

            if (id == -1)
            {
                SetContentView(Resource.Layout.AlarmActivity);
                SetControlsForCountActivity();
                ShowCountActivity();
                return;
            }

            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);

            SetMediaPlayer();
            SetVibrator();

            SetContentView(Resource.Layout.AlarmActivity);
            SetControls();

            AddWindowManagerFlags();

            Alarm alarm;

            var alarmsRepo = App.AlarmsRepo;

            if (alarmsRepo != null)
            {
                alarm = App.AlarmsRepo.GetAlarm(id);
                alarm.Days = App.AlarmsRepo.GetDaysOfWeek(alarm.DaysId);

                if (!isRepeating)
                {
                    alarm.IsActive = false;
                    App.Service.SaveAlarmAtLocal(alarm);
                }
                else
                {
                    AlarmController.SetNextAlarm(alarm);
                }

                App.Service.SendChangeAlarmsMessage();
            }

            if (bundle == null) return;
        }

        private void SetControls()
        {
            alarmLayout = FindViewById<LinearLayout>(Resource.Id.alarmLayout);
            tellmeButton = FindViewById<Button>(Resource.Id.tellmeButton);
            countTextView = FindViewById<TextView>(Resource.Id.countTextView);
            alarmTextView = FindViewById<TextView>(Resource.Id.alarmTextView);
            alarmEditText = FindViewById<EditText>(Resource.Id.alarmEditText);

            tellmeButton.Click += TellmeButton_Click;
            countTextView.Text = "5.00 초";
            alarmTextView.Text = name;

            alarmEditText.Enabled = false;
        }

        private void SetControlsForCountActivity()
        {
            alarmLayout = FindViewById<LinearLayout>(Resource.Id.alarmLayout);
            tellmeButton = FindViewById<Button>(Resource.Id.tellmeButton);
            countTextView = FindViewById<TextView>(Resource.Id.countTextView);
            alarmTextView = FindViewById<TextView>(Resource.Id.alarmTextView);
            alarmEditText = FindViewById<EditText>(Resource.Id.alarmEditText);

            countTextView.Text = "5.00 초";
        }

        private void AddWindowManagerFlags()
        {
            // add flags to turn screen on and appear over lock screen
            Window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            Window.AddFlags(WindowManagerFlags.DismissKeyguard);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            Window.AddFlags(WindowManagerFlags.TurnScreenOn);
        }

        

        async void TellmeButton_Click(object sender, EventArgs e)
        {
            _soundService?.StopAudio();

            if (!alarmEditText.Enabled)
            {
                alarmEditText.Text = await WaitForSpeechToText();
                alarmEditText.Enabled = true;
                tellmeButton.Text = "5초 카운트!";
            }

            var editText = alarmEditText.Text.Replace(" ", "");
            var textView = alarmTextView.Text.Replace(" ", "");

            if (editText == textView)
            {
                var toastService = new ToastServiceAndroid();

                toastService.Show("이제 5초를 셉니다!");

                ShowCountActivity();

            }
            else
            {
                ShowAlertDoNotMatchText();
            }
        }

        private void ShowCountActivity()
        {
            _vibrator?.Cancel();

            HideAllViewExceptForCountText();

            if (isCountOn)
            {
                _soundService.PlayCountAudio();
            }

            SetCountDown();
        }

        async Task<string> WaitForSpeechToText()
        {
            var stt = new SpeechToText_Android();
            return await stt.SpeechToTextAsync();
        }

        private void ShowAlertDoNotMatchText()
        {
            AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            AlertDialog alert = dialog.Create();
            alert.SetTitle("미션 내용 불일치");
            alert.SetMessage("미션 내용을 정확히 기입하여주세요");
            alert.SetButton("확인", (c, ev) =>
            {
                alert.Dispose();
            });
            alert.Show();
        }

        private void SetMediaPlayer()
        {
            if (isAlarmOn)
            {
                AlarmTone alarmTone = AlarmTone.Tones.Find(a => a.Name == toneName);
                _soundService.PlayAudio(alarmTone, true, alarmVolume);
            }
        }

        private void SetVibrator()
        {
            if (isVibrateOn)
            {
                _vibrator = Vibrator.FromContext(this);
                long[] mVibratePattern = new long[] { 0, 400, 1000, 600, 1000, 800, 1000, 1000 };
                VibrationEffect effect = VibrationEffect.CreateWaveform(mVibratePattern, 0);
                _vibrator.Vibrate(effect);
            }
        }

        private void HideAllViewExceptForCountText()
        {
            alarmLayout.Visibility = ViewStates.Invisible;
            countTextView.Visibility = ViewStates.Visible;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            var VOICE = 10;
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == VOICE)
            {
                if (resultCode == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        var textInput = matches[0];
                        if (textInput.Length > 500)
                            textInput = textInput.Substring(0, 500);
                        SpeechToText_Android.SpeechText = textInput;
                    }
                }
                SpeechToText_Android.autoEvent.Set();
            }
        }

        private async void SetCountDown()
        {
            await Task.Delay(330);
            countDown = new CountDown(5000, 10, this);
            countDown.Start();
        }

        public override void OnBackPressed()
        {
        }

        private class CountDown : CountDownTimer
        {
            public long CountDownInterval { get; }
            public long MillisInFuture { get; }
            public Activity Activity { get; set; }

            public CountDown(long millisInFuture, long countDownInterval, Activity activity) : base(millisInFuture, countDownInterval)
            {
                Activity = activity;
                MillisInFuture = millisInFuture;
                CountDownInterval = countDownInterval;
            }

            public override void OnFinish()
            {
                Activity.FinishAndRemoveTask();
            }

            public override void OnTick(long millisUntilFinished)
            {
                var alarmActivity = Activity as AlarmActivity;
                var countTextView = alarmActivity.countTextView;

                double count = (double)millisUntilFinished / 1000;
                var stringFormat = string.Format("{0:f2}", count);
                countTextView.Text = stringFormat + "초";
            }
        }
    }

    
}