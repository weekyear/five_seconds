using System;
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

namespace Five_Seconds.Droid
{
    [Activity(Label = "AlarmActivity", NoHistory = true, Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class TellMeActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const int VOICE = 10;

        public TellMeActivity()
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "Constructor");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CheckIsExistMic();

            SetContentView(Resource.Layout.TellMeActivity);
            var tellmeButton = FindViewById<Button>(Resource.Id.tellmeButton);
            tellmeButton.Click += TellmeButton_Click;

            // add flags to turn screen on and appear over lock screen
            //Window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            //Window.AddFlags(WindowManagerFlags.DismissKeyguard);
            //Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            //Window.AddFlags(WindowManagerFlags.TurnScreenOn);

            Intent intent = Intent;
            Bundle bundle = intent.Extras;

            if (bundle == null) return;

            var id = (int)bundle.Get("id");
            var timeTextView = FindViewById<TextView>(Resource.Id.timeTextView);
            var missionTextView = FindViewById<TextView>(Resource.Id.missionTextView);
            var mission = App.MissionsRepo.GetMission(id);
            timeTextView.Text = "5";
            missionTextView.Text = mission.Name;

            // 벨소리 늘리거나 커스텀 벨소리 넣으려면 AlarmApp example솔루션 열어서 확인
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
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

        private void CheckIsExistMic()
        {
            string rec = PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                var alert = new AlertDialog.Builder(Application.Context);
                alert.SetTitle("You don't seem to have a microphone to record with");
                alert.SetPositiveButton("OK", (sender, e) =>
                {
                    return;
                });
                alert.Show();
            }
        }

        void TellmeButton_Click(object sender, EventArgs e)
        {
            //removes our app from the scree and from 'recent apps' section
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Finish();
            }
            else
            {
                Finish();
            }

            //if (_settings.IsVibrateOn)
            //    _vibrator.Cancel();

            Java.Lang.JavaSystem.Exit(0);
        }

        protected override void Dispose(bool disposing)
        {

            //close mediaplayer? and vibrator?
            base.Dispose(disposing);
        }
    }
}