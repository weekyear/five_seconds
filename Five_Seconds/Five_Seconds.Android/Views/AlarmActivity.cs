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
using Five_Seconds.Services;
using Xamarin.Forms;
using Button = Android.Widget.Button;

namespace Five_Seconds.Droid
{
    [Activity(Label = "AlarmActivity", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AlarmActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        //AlarmApp.Models.Settings _settings;

        MediaPlayer _mediaPlayer = new MediaPlayer();
        Vibrator _vibrator;
        EditText missionEditText;
        int id;

        readonly long[] _pattern =
        {
            0, 500, 500
        };

        public AlarmActivity()
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "Constructor");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "OnCreate");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AlarmActivity);
            var tellmeButton = FindViewById<Button>(Resource.Id.tellmeButton);
            tellmeButton.Click += CloseButton_Click;

            // add flags to turn screen on and appear over lock screen
            Window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            Window.AddFlags(WindowManagerFlags.DismissKeyguard);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            Window.AddFlags(WindowManagerFlags.TurnScreenOn);

            Intent intent = Intent;
            Bundle bundle = intent.Extras;

            if (bundle == null) return;

            id = (int)bundle.Get("id");
            var timeTextView = FindViewById<TextView>(Resource.Id.timeTextView);
            var missionTextView = FindViewById<TextView>(Resource.Id.missionTextView);
            missionEditText = FindViewById<EditText>(Resource.Id.missionEditText);
            var mission = App.MissionsRepo.GetMission(id);
            var alarm = App.MissionsRepo.GetAlarm(id);
            timeTextView.Text = alarm.TimeOffset.ToLocalTime().ToString(@"hh\:mm");
            missionTextView.Text = mission.Name;

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

            if (alarm.IsVibrateOn)
            {
                _vibrator = Vibrator.FromContext(this);
                _vibrator.Vibrate(VibrationEffect.CreateOneShot(500 * alarm.VibeFrequency / 10, VibrationEffect.DefaultAmplitude));
                Log.Debug(AlarmSetterAndroid.AlarmTag, "Done Create");
            }
        }

        async void CloseButton_Click(object sender, EventArgs e)
        {

            _mediaPlayer?.Stop();
            _vibrator?.Cancel();

            //removes our app from the scree and from 'recent apps' section
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                missionEditText.Text = await WaitForSpeechToText();
            }
            else
            {
                missionEditText.Text = await WaitForSpeechToText();
            }
        }

        async Task<string> WaitForSpeechToText()
        {
            var stt = DependencyService.Get<ISpeechToText>();
            return await stt.SpeechToTextAsync();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void Dispose(bool disposing)
        {

            //close mediaplayer? and vibrator?
            base.Dispose(disposing);
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
    }
}