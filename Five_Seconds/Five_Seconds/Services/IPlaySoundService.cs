using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Services
{
    public interface IPlaySoundService
    {
        void PlayAudio(AlarmTone alarmTone);
        void PlayAudio(AlarmTone alarmTone, bool isLooping);
        void StopAudio();
    }
}
