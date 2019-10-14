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
            //SetAllAlarmTones();
        }

        private void ConstructCommand()
        {
            ToneSaveCommand = new Command(() => ToneSave());
            ClickPlayCommand = new Command(() => ClickPlay());
            PlayToneCommand = new Command(() => PlayTone());
            StopToneCommand = new Command(() => StopTone());
            AddToneCommand = new Command(() => AddTone());
        }

        public void SetAllAlarmTones()
        {
            var AlarmTones = AlarmTone.Tones;
            foreach (var alarmTone in AlarmTones)
            {
                var isSelected = false;
                if (alarmTone.Name == Alarm.Tone) 
                { 
                    isSelected = true;
                }

                var settingTone = new SettingTone { AlarmTone = alarmTone, IsSelected = isSelected };
                AllAlarmTones.Add(settingTone);
            }

            foreach (var settingTone in AllAlarmTones)
            {
                if (settingTone.AlarmTone.Name == Alarm.Tone)
                {
                    SelectedTone = settingTone;
                }
            }


        }

        public Command ToneSaveCommand { get; set; }
        public Command ClickPlayCommand { get; set; }
        public Command PlayToneCommand { get; set; }
        public Command StopToneCommand { get; set; }
        public Command AddToneCommand { get; set; }


        public ObservableCollection<SettingTone> AllAlarmTones { get; set; } = new ObservableCollection<SettingTone>();

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
                OnPropertyChanged(nameof(Volume));
            }
        }

        public bool IsPlaying
        {
            get; set;
        }

        private SettingTone selectedTone;
        public SettingTone SelectedTone
        {
            get 
            {
                if (selectedTone == null)
                {
                    foreach (var tone in AllAlarmTones)
                    {
                        if (tone.AlarmTone.Name == Alarm.Tone)
                        {
                            selectedTone = tone;
                        }
                    }
                }
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
            _soundService.PlayAudio(SelectedTone.AlarmTone, true, Alarm.Volume);
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
            Alarm.Tone = SelectedTone.AlarmTone.Name;
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

            var newSettingTone = new SettingTone
            {
                AlarmTone = newTone,
                IsSelected = true
            };

            AllAlarmTones.Add(newSettingTone);

            _fileLocator.FileChosen -= ToneFileChosen;
        }

        private async Task ClosePopup()
        {
            StopTone();
            await Navigation.PopAsync(true);
        }

        public void ChangeIsSelected()
        {
            foreach (var settingTone in AllAlarmTones)
            {
                if (settingTone.AlarmTone.Name == SelectedTone.AlarmTone.Name)
                {
                    settingTone.IsSelected = true;
                }
                else
                {
                    settingTone.IsSelected = false;
                }
            }
        }

        public class SettingTone : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public AlarmTone AlarmTone { get; set; }

            private bool isSelected;
            public bool IsSelected 
            {
                get { return isSelected; } 
                set
                {
                    if (isSelected == value) return;
                    isSelected = value;
                }
            }
        }
    }
}
