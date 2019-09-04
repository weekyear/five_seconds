using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Media;
using Android.OS;
using Android.Speech;
using Android.Util;
using Android.Views;
using Android.Widget;
using Five_Seconds.Droid.Services;
using Five_Seconds.Models;
using Five_Seconds.Services;
using Xamarin.Forms;
using Button = Android.Widget.Button;

namespace Five_Seconds.Droid
{
    [Activity(Label = "AlarmActivity", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AlarmActivity : Activity
    {
        MediaPlayer _mediaPlayer = new MediaPlayer();
        Vibrator _vibrator;

        private Button tellmeButton;
        public TextView timeTextView;
        private TextView missionTextView;
        private EditText missionEditText;
        private CountDown countDown;

        int id;

        public AlarmActivity()
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "Constructor");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "OnCreate");
            base.OnCreate(savedInstanceState);

            Intent intent = Intent;
            Bundle bundle = intent.Extras;
            id = (int)bundle.Get("id");

            var mission = GetMissionById(id);
            var alarm = GetAlarmById(id);

            SetContentView(Resource.Layout.AlarmActivity);
            SetControls(mission, alarm);

            AddWindowManagerFlags();

            if (bundle == null) return;


            SetMediaPlayer(alarm);

            SetVibrator(alarm);
        }

        private Mission GetMissionById(int id)
        {
            var mission = App.MissionsRepo.GetMission(id);

            return mission;
        }

        private Alarm GetAlarmById(int id)
        {
            var alarm = App.MissionsRepo.GetAlarm(id);

            return alarm;
        }

        private void SetControls(Mission mission, Alarm alarm)
        {
            tellmeButton = FindViewById<Button>(Resource.Id.tellmeButton);
            timeTextView = FindViewById<TextView>(Resource.Id.timeTextView);
            missionTextView = FindViewById<TextView>(Resource.Id.missionTextView);
            missionEditText = FindViewById<EditText>(Resource.Id.missionEditText);

            tellmeButton.Click += TellmeButton_Click;
            timeTextView.Text = "5.00 초";
            missionTextView.Text = mission.Name;

            missionEditText.Enabled = false;
        }

        private void AddWindowManagerFlags()
        {
            // add flags to turn screen on and appear over lock screen
            Window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            Window.AddFlags(WindowManagerFlags.DismissKeyguard);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            Window.AddFlags(WindowManagerFlags.TurnScreenOn);
        }

        private void SetMediaPlayer(Alarm alarm)
        {
            // 벨소리 늘리거나 커스텀 벨소리 넣으려면 AlarmApp example솔루션 열어서 확인
            string alarmTonePath = $"{alarm.Tone.ToLower()}.mp3";
            AssetFileDescriptor assetFileDescriptor = Assets.OpenFd(alarmTonePath);
            _mediaPlayer.SetDataSource(assetFileDescriptor.FileDescriptor, assetFileDescriptor.StartOffset, assetFileDescriptor.Length);
            var maxVolume = 10;
            float log1 = (float)(Math.Log(maxVolume - alarm.Volume) / Math.Log(maxVolume));

            if (alarm.IsAlarmOn)
            {
                _mediaPlayer.SetVolume(1 - log1, 1 - log1);
                _mediaPlayer.Looping = true;
                _mediaPlayer.Prepare();
                _mediaPlayer.Start();
            }
        }

        private void SetVibrator(Alarm alarm)
        {
            if (alarm.IsVibrateOn)
            {
                _vibrator = Vibrator.FromContext(this);
                long[] mVibratePattern = new long[] { 0, 400, 1000, 600, 1000, 800, 1000, 1000 };
                VibrationEffect effect = VibrationEffect.CreateWaveform(mVibratePattern, 0);
                _vibrator.Vibrate(effect);
                Log.Debug(AlarmSetterAndroid.AlarmTag, "Done Create");
            }
        }

        async void TellmeButton_Click(object sender, EventArgs e)
        {
            _mediaPlayer?.Stop();
            _vibrator?.Cancel();

            if (!missionEditText.Enabled)
            {
                SetCountDown();
                missionEditText.Text = await WaitForSpeechToText();
                StopCountDown();
                missionEditText.Enabled = true;
                tellmeButton.Text = "5초의 법칙 시작!";
            }

            var editText = missionEditText.Text.Replace(" ", "");
            var textView = missionTextView.Text.Replace(" ", "");

            if (editText == textView)
            {
                ShowAlertSuccessOfFailed();
            }
            else
            {
                ShowAlertDoNotMatchText();
            }
        }

        async Task<string> WaitForSpeechToText()
        {
            var stt = DependencyService.Get<ISpeechToText>();
            return await stt.SpeechToTextAsync();
        }

        private void ShowAlertDoNotMatchText()
        {
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            AlertDialog alert = dialog.Create();
            alert.SetTitle("미션 내용 불일치");
            alert.SetMessage("미션 내용을 정확히 기입하여주세요");
            alert.SetButton("확인", (c, ev) =>
            {
                alert.Dispose();
            });
            alert.Show();
        }

        private void ShowAlertSuccessOfFailed()
        {
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            AlertDialog alert = dialog.Create();
            alert.SetTitle("미션 성공");
            alert.SetMessage("미션 성공했습니다~~");
            alert.SetButton("확인", (c, ev) =>
            {
                alert.Dispose();
                FinishAndRemoveTask();
            });
            alert.Show();
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

        private void SetCountDown()
        {
            countDown = new CountDown(6000, 10, this);
            countDown.Start();
        }

        private void StopCountDown()
        {
            countDown.Cancel();
        }
    }

    public class CountDown : CountDownTimer
    {
        private Activity _activity;
        private long _millisInFuture;
        private long _countDownInterval;

        public CountDown(long millisInFuture, long countDownInterval, Activity activity) : base(millisInFuture, countDownInterval)
        {
            _activity = activity;
            _millisInFuture = millisInFuture;
            _countDownInterval = countDownInterval;
        }

        public override void OnFinish()
        {
            
        }

        public override void OnTick(long millisUntilFinished)
        {
            var alarmActivity = _activity as AlarmActivity;
            var timeTextView = alarmActivity.timeTextView;

            double count = (double)millisUntilFinished / 1000;
            var stringFormat = string.Format("{0:f2}", count);
            timeTextView.Text = stringFormat + "초";
        }
    }
}