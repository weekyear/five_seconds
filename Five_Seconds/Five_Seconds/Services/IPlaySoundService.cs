using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Services
{
    public interface IPlaySoundService
    {
        void PlayAudio(AlarmTone alarmTone, int volume);
        void PlayAudio(AlarmTone alarmTone, bool isLooping, int volume);
        void PlayCountAudio();
        void StopAudio();
    }
}
