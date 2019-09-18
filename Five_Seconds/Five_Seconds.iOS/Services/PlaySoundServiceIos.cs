using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVFoundation;
using Five_Seconds.iOS.Services;
using Five_Seconds.Models;
using Five_Seconds.Services;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(PlaySoundServiceIos))]
namespace Five_Seconds.iOS.Services
{
    public class PlaySoundServiceIos : IPlaySoundService
    {
        private AVAudioPlayer _audioPlayer = null;
        public Action OnFinishedPlaying { get; set; }

        public void PlayAudio(AlarmTone alarmTone, int volume)
        {
            PlayAudio(alarmTone, false, volume);
        }

        public void PlayAudio(AlarmTone alarmTone, bool isLooping, int volume)
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.FinishedPlaying -= Player_FinishedPlaying;
                _audioPlayer.Stop();
            }

            string localUrl = alarmTone.Path;
            _audioPlayer = AVAudioPlayer.FromUrl(NSUrl.FromFilename(localUrl));
            _audioPlayer.FinishedPlaying += Player_FinishedPlaying;
            _audioPlayer.Play();
        }

        public void PlayCountAudio()
        {
            throw new NotImplementedException();
        }

        public void StopAudio()
        {
            throw new NotImplementedException();
        }

        private void Player_FinishedPlaying(object sender, AVStatusEventArgs e)
        {
            OnFinishedPlaying?.Invoke();
        }

        public void Pause()
        {
            _audioPlayer?.Pause();
        }

        public void Play()
        {
            _audioPlayer?.Play();
        }
    }
}