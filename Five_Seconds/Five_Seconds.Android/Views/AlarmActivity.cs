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
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Plugin.CurrentActivity;
using Xamarin.Essentials;
using Xamarin.Forms;
using Button = Android.Widget.Button;

namespace Five_Seconds.Droid
{
    [Activity(Label = "AlarmActivity", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AlarmActivity : Activity
    {
        IPlaySoundService _soundService = new PlaySoundServiceAndroid();
        Vibrator _vibrator;

        private LinearLayout missionLayout;
        private Button tellmeButton;
        public TextView countTextView;
        private TextView missionTextView;
        private EditText missionEditText;
        private CountDown countDown;

        private Alarm alarm;

        int id;

        public AlarmActivity()
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "Constructor");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Console.WriteLine("OnCreate_AlarmActivity");
            base.OnCreate(savedInstanceState);

            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);

            Bundle bundle = Intent.Extras;
            id = (int)bundle.Get("id");

            AlarmController.SetNextAlarm(id);
            Console.WriteLine("AlarmController_AlarmActivity");

            var mission = AlarmController.AlarmMissionNow;
            Console.WriteLine(mission.Name);

            alarm = mission.Alarm;
            Console.WriteLine(alarm.Time);

            SetMediaPlayer(alarm);
            Console.WriteLine("SetMediaPlayer_AlarmActivity");
            SetVibrator(alarm);
            Console.WriteLine("SetVibrator_AlarmActivity");

            SetContentView(Resource.Layout.AlarmActivity);
            SetControls(mission);

            AddWindowManagerFlags();

            if (bundle == null) return;


            if (!mission.IsActive)
            {
                App.Service.DeleteMission(mission);
                Console.WriteLine("App.DeleteMission_AlarmActivity");
            }
            else
            {
                var AlarmNotification = new AlarmNotificationAndroid();
                AlarmNotification.UpdateNotification();
            }
        }

        private void SetControls(Mission mission)
        {
            missionLayout = FindViewById<LinearLayout>(Resource.Id.missionLayout);
            tellmeButton = FindViewById<Button>(Resource.Id.tellmeButton);
            countTextView = FindViewById<TextView>(Resource.Id.countTextView);
            missionTextView = FindViewById<TextView>(Resource.Id.missionTextView);
            missionEditText = FindViewById<EditText>(Resource.Id.missionEditText);

            tellmeButton.Click += TellmeButton_Click;
            countTextView.Text = "5.00 초";
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

        

        async void TellmeButton_Click(object sender, EventArgs e)
        {
            _soundService?.StopAudio();

            if (!missionEditText.Enabled)
            {
                missionEditText.Text = await WaitForSpeechToText();
                missionEditText.Enabled = true;
                tellmeButton.Text = "5초 카운트!";
            }

            var editText = missionEditText.Text.Replace(" ", "");
            var textView = missionTextView.Text.Replace(" ", "");

            if (editText == textView)
            {
                _vibrator?.Cancel();

                HideAllViewExceptForCountText();

                SetCountDown();

                if (alarm.IsCountOn)
                {
                    _soundService.PlayCountAudio();
                }
            }
            else
            {
                ShowAlertDoNotMatchText();
            }
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

        private void SetMediaPlayer(Alarm alarm)
        {
            if (alarm.IsAlarmOn)
            {
                AlarmTone alarmTone = AlarmTone.Tones.Find(a => a.Name == alarm.Tone);
                _soundService.PlayAudio(alarmTone, true, alarm.Volume);
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
            }
        }

        private void HideAllViewExceptForCountText()
        {
            missionLayout.Visibility = ViewStates.Invisible;
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

        private void SetCountDown()
        {
            countDown = new CountDown(5300, 10, this);
            countDown.Start();
        }

        public override void OnBackPressed()
        {
        }
    }

    public class CountDown : CountDownTimer
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