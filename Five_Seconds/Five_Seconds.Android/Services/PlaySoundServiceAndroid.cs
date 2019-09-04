﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Five_Seconds.Droid.Services;
using Five_Seconds.Models;
using Five_Seconds.Services;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(PlaySoundServiceAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class PlaySoundServiceAndroid : IPlaySoundService
    {
        static CancellationTokenSource tokenSource2 = new CancellationTokenSource();
        CancellationToken ct = tokenSource2.Token;

        MediaPlayer _mediaPlayer = new MediaPlayer();
        AssetFileDescriptor _assetFileDescriptor;
        Task playSoundTask;

        public void PlayAudio(AlarmTone alarmTone, int volume)
        {
            PlayAudio(alarmTone, false, volume);
        }

        public void PlayAudio(AlarmTone alarmTone, bool isLooping, int volume)
        {
            StopAudio();

            var isADefaultTone = AlarmTone.Tones.FirstOrDefault(x => x.Path == alarmTone.Path) != default(AlarmTone);

            if (isADefaultTone)
            {
                _assetFileDescriptor = Forms.Context.Assets.OpenFd(alarmTone.Path);
                _mediaPlayer.SetDataSource(_assetFileDescriptor.FileDescriptor, _assetFileDescriptor.StartOffset, _assetFileDescriptor.Length);
            }
            else
            {
                string alarmTonePath;
                string[] split = alarmTone.Path.Split(':');
                alarmTonePath = split[1];
                _mediaPlayer.SetDataSource(alarmTonePath);
            }

            var maxVolume = 10;
            float log1 = (float)(Math.Log(maxVolume - volume) / Math.Log(maxVolume));

            _mediaPlayer.SetVolume(1 - log1, 1 - log1);
            _mediaPlayer.Looping = isLooping;
            _mediaPlayer.Prepare();
            _mediaPlayer.Start();
        }

        public void StopAudio()
        {
            if (_mediaPlayer.IsPlaying)
                _mediaPlayer.Stop();

            _mediaPlayer.Reset();
        }
    }
}