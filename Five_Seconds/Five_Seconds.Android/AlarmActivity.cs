using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Five_Seconds.Droid.Services;
using Five_Seconds.Models;
using Five_Seconds.Repository;

namespace Five_Seconds.Droid
{
    [Activity(Label = "AlarmActivity", NoHistory = true, Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AlarmActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        IAlarmRepository _alarmRepo = App.AlarmRepo;
        Alarm _alarm;
        //AlarmApp.Models.Settings _settings;

        MediaPlayer _mediaPlayer = new MediaPlayer();
        Vibrator _vibrator;
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
            var closeButton = FindViewById<Button>(Resource.Id.closeButton);
            closeButton.Click += CloseButton_Click;

            // add flags to turn screen on and appear over lock screen
            Window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            Window.AddFlags(WindowManagerFlags.DismissKeyguard);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            Window.AddFlags(WindowManagerFlags.TurnScreenOn);

            Intent intent = Intent;
            Bundle bundle = intent.Extras;

            if (bundle == null) return;

            var id = (int)bundle.Get("id");
            var timeTextView = FindViewById<TextView>(Resource.Id.timeTextView);
            var nameTextView = FindViewById<TextView>(Resource.Id.nameTextView);
            _alarm = _alarmRepo.GetAlarm(id);
            timeTextView.Text = _alarm.TimeOffset.ToLocalTime().ToString(@"hh\:mm");
            nameTextView.Text = _alarm.Name;

            // 벨소리 늘리거나 커스텀 벨소리 넣게할라면 AlarmApp 솔루션 열어서 확인
            string alarmTonePath = "rocket.m4a";
            AssetFileDescriptor assetFileDescriptor = Assets.OpenFd(alarmTonePath);
            _mediaPlayer.SetDataSource(assetFileDescriptor.FileDescriptor, assetFileDescriptor.StartOffset, assetFileDescriptor.Length);

            _mediaPlayer.Looping = true;
            _mediaPlayer.Prepare();
            _mediaPlayer.Start();

            //if (_alarm.IsVibrateOn) return;


            _vibrator = Vibrator.FromContext(this);
            _vibrator.Vibrate(VibrationEffect.CreateOneShot(500, VibrationEffect.DefaultAmplitude));
            Log.Debug(AlarmSetterAndroid.AlarmTag, "Done Create");
        }

        void CloseButton_Click(object sender, EventArgs e)
        {
            //removes our app from the scree and from 'recent apps' section
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                FinishAndRemoveTask();
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