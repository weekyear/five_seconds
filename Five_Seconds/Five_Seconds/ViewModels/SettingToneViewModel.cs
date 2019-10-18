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
            SetSettingTones();
        }

        private void ConstructCommand()
        {
            ToneSaveCommand = new Command(() => ToneSave());
            ClickPlayCommand = new Command(() => ClickPlay());
            PlayToneCommand = new Command(() => PlayTone());
            StopToneCommand = new Command(() => StopTone());
            AddToneCommand = new Command(() => AddTone());
        }

        public void SetSettingTones()
        {
            AllAlarmTones.Clear();

            var AlarmTones = AlarmTone.Tones;
            foreach(var alarmTone in AlarmTones)
            {
                bool isSelected = false;
                if (alarmTone.Name == Alarm.Tone)
                {
                    isSelected = true;
                }
                AllAlarmTones.Add(new SettingTone(alarmTone.Name, isSelected));
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
                ToneSave();
                OnPropertyChanged(nameof(Volume));
            }
        }

        public string SelectedToneName
        {
            get { return Alarm.Name; }
            set
            {
                if (Alarm.Name == value) return;
                Alarm.Name = value;
                ToneSave();
                OnPropertyChanged(nameof(SelectedToneName));
            }
        }

        public bool IsPlaying
        {
            get; set;
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
            _soundService.PlayAudio(AlarmTone.Tones.Find(a => a.Name == Alarm.Tone), true, Alarm.Volume);
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

            AllAlarmTones.Add(new SettingTone(newTone.Name, false));

            _fileLocator.FileChosen -= ToneFileChosen;
        }

        public void ClickTone(SettingTone selectedTone)
        {
            Alarm.Tone = selectedTone.Name;

            if (IsPlaying)
            {
                StopTone();
                PlayTone();
            }

            SetIsSelected(selectedTone);
        }

        public void SetIsSelected(SettingTone selectedTone)
        {
            foreach(var settingTone in AllAlarmTones)
            {
                if (settingTone.Name == selectedTone.Name)
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
            public SettingTone(string name, bool isSelected)
            {
                Name = name;
                IsSelected = isSelected;
            }
            public string Name { get; set; }
            public bool IsSelected { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
