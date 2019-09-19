using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class SettingToneViewModel : BaseViewModel
    {
        IPlaySoundService _soundService = DependencyService.Get<IPlaySoundService>();
        IFileLocator _fileLocator = DependencyService.Get<IFileLocator>();

        public SettingToneViewModel(INavigation navigation, Mission mission) : base(navigation)
        {
            Mission = mission;
            ConstructCommand();
        }

        private void ConstructCommand()
        {
            ToneSaveCommand = new Command<AlarmTone>(async (a) => await ToneSave(a));
            ClickPlayCommand = new Command<AlarmTone>((a) => ClickPlay(a));
            PlayToneCommand = new Command<AlarmTone>((a) => PlayTone(a));
            StopToneCommand = new Command(() => StopTone());
            AddToneCommand = new Command(() => AddTone());
        }

        public Command ToneSaveCommand { get; set; }
        public Command ClickPlayCommand { get; set; }
        public Command PlayToneCommand { get; set; }
        public Command StopToneCommand { get; set; }
        public Command AddToneCommand { get; set; }


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

        public bool IsPlaying
        {
            get; set;
        }

        private void ClickPlay(AlarmTone tone)
        {
            if (tone.IsPlaying)
            {
                StopTone();
                tone.IsPlaying = false;
            }
            else
            {
                PlayTone(tone);
                PauseAllToneExceptSeletedTone(tone);
            }
        }

        private void PlayTone(AlarmTone tone)
        {
            _soundService.PlayAudio(tone, true, Mission.Alarm.Volume);
        }

        private void StopTone()
        {
            _soundService.StopAudio();
        }

        private void AddTone()
        {
            _fileLocator.FileChosen += ToneFileChosen;
            _fileLocator.OpenFileLocator();
        }

        private async Task ToneSave(AlarmTone tone)
        {
            Mission.Alarm.Tone = tone.Name;
            await ClosePopup();
        }

        void ToneFileChosen(string path)
        {
            var filename = Path.GetFileNameWithoutExtension(path);

            var newTone = new AlarmTone
            {
                Name = filename,
                Path = path,
                IsCustomTone = true
            };
            AllAlarmTones.Add(newTone);
            AlarmTone.Tones.Add(newTone);
            App.AlarmToneRepo.AddTone(newTone);

            _fileLocator.FileChosen -= ToneFileChosen;
        }

        private async Task ClosePopup()
        {
            StopTone();
            await Navigation.PopAsync(true);
        }

        private void PauseAllToneExceptSeletedTone(AlarmTone selectedTone)
        {
            foreach (var tone in AllAlarmTones)
            {
                if (tone.Path == selectedTone.Path)
                {
                    tone.IsPlaying = true;
                }
                else
                {
                    tone.IsPlaying = false;
                }
            }
        }
    }
}
