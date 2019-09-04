using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class SettingToneViewModel : BaseViewModel
    {
        IPlaySoundService _soundService = DependencyService.Get<IPlaySoundService>();

        public SettingToneViewModel(INavigation navigation, Mission mission) : base(navigation)
        {
            Mission = mission;
            ConstructCommand();
        }

        private void ConstructCommand()
        {
            CloseCommand = new Command(async () => await ClosePopup());
            ToneSaveCommand = new Command<AlarmTone>(async (a) => await ToneSave(a));
            PlayToneCommand = new Command<AlarmTone>((a) => PlayTone(a));
            StopToneCommand = new Command(() => StopTone());
        }

        public Command CloseCommand { get; set; }
        public Command ToneSaveCommand { get; set; }
        public Command PlayToneCommand { get; set; }
        public Command StopToneCommand { get; set; }


        public ObservableCollection<AlarmTone> AllAlarmTones { get; set; } = new ObservableCollection<AlarmTone>(AlarmTone.Tones);

        public Mission Mission
        {
            get; set;
        }

        public int Volume
        {
            get { return Mission.Alarm.Volume; }
            set
            {
                if (Mission.Alarm.Volume == value) return;
                Mission.Alarm.Volume = value;
                OnPropertyChanged(nameof(Volume));
            }
        }

        private void PlayTone(AlarmTone tone)
        {
            _soundService.PlayAudio(tone, Mission.Alarm.Volume);
        }

        private void StopTone()
        {
            _soundService.StopAudio();
        }

        private async Task ToneSave(AlarmTone tone)
        {
            Mission.Alarm.Tone = tone.Name;
            await ClosePopup();
        }
        private async Task ClosePopup()
        {
            StopTone();
            await Navigation.PopAsync(true);
        }
    }
}
