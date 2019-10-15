using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class SettingToneViewModel : BaseViewModel
    {
        readonly IPlaySoundService _soundService = DependencyService.Get<IPlaySoundService>();
        readonly IFileLocator _fileLocator = DependencyService.Get<IFileLocator>();

        public SettingToneViewModel(INavigation navigation, Alarm alarm) : base(navigation)
        {
            Alarm = alarm;
            ConstructCommand();
            SetSelectedTone();
        }

        private void ConstructCommand()
        {
            ToneSaveCommand = new Command(() => ToneSave());
            ClickPlayCommand = new Command(() => ClickPlay());
            PlayToneCommand = new Command(() => PlayTone());
            StopToneCommand = new Command(() => StopTone());
            AddToneCommand = new Command(() => AddTone());
        }

        public void SetSelectedTone()
        {
            for (int i = 0; i < AllAlarmTones.Count; i++)
            {
                if (AllAlarmTones[i].Name == Alarm.Tone)
                {
                    SelectedTone = AllAlarmTones[i];
                }
            }
        }

        public Command ToneSaveCommand { get; set; }
        public Command ClickPlayCommand { get; set; }
        public Command PlayToneCommand { get; set; }
        public Command StopToneCommand { get; set; }
        public Command AddToneCommand { get; set; }


        public ObservableCollection<AlarmTone> AllAlarmTones { get; set; } = new ObservableCollection<AlarmTone>(AlarmTone.Tones);

        public Alarm Alarm
        {
            get; set;
        }

        public int Volume
        {
            get { return Alarm.Volume; }
            set
            {
                if (Alarm.Volume == value) return;
                Alarm.Volume = value;
                ToneSave();
                OnPropertyChanged(nameof(Volume));
            }
        }

        public bool IsPlaying
        {
            get; set;
        }

        private AlarmTone selectedTone;
        public AlarmTone SelectedTone
        {
            get 
            {
                return selectedTone; 
            }
            set
            {
                if (selectedTone != value)
                {
                    selectedTone = value;
                    OnPropertyChanged(nameof(SelectedTone));
                }
            }
        }

        private void ClickPlay()
        {
            if (IsPlaying)
            {
                StopTone();
            }
            else
            {
                PlayTone();
            }

            IsPlaying = !IsPlaying;
        }

        private void PlayTone()
        {
            _soundService.PlayAudio(SelectedTone, true, Alarm.Volume);
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

        private void ToneSave()
        {
            Alarm.Tone = SelectedTone.Name;
            MessagingCenter.Send(this, "changeAlarmTone", Alarm);
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

            AlarmTone.Tones.Add(newTone);
            App.AlarmToneRepo.AddTone(newTone);

            AllAlarmTones.Add(newTone);

            _fileLocator.FileChosen -= ToneFileChosen;
        }

        private async Task ClosePopup()
        {
            StopTone();
            await Navigation.PopAsync(true);
        }
    }
}
