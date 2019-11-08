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

        private readonly IMessageBoxService MessageBoxService;

        public static bool IsFinding;
        public SettingToneViewModel(INavigation navigation, Alarm alarm) : base(navigation)
        {
            Alarm = alarm;
            MessageBoxService = new MessageBoxService();
            ConstructCommand();
            SetSettingTones();
        }

        private void ConstructCommand()
        {
            ToneSaveCommand = new Command(() => ToneSave());
            ClickPlayCommand = new Command(() => ClickPlay());
            PlayToneCommand = new Command(() => PlayTone());
            StopToneCommand = new Command(() => StopTone());
            ChangeVolumeCommand = new Command<double>((v) => ChangeVolume(v));
            AddToneCommand = new Command(() => AddTone());
            DeleteToneCommand = new Command<string>((a) => DeleteTone(a));
        }

        public void SetSettingTones()
        {
            AllAlarmTones.Clear();

            var AlarmTones = App.AlarmToneRepo.Tones;
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
        public Command<double> ChangeVolumeCommand { get; set; }
        public Command AddToneCommand { get; set; }
        public Command<string> DeleteToneCommand { get; set; }


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
            _soundService.PlayAudio(App.AlarmToneRepo.Tones.Find(a => a.Name == Alarm.Tone), true, Alarm.Volume);
        }

        private void StopTone()
        {
            _soundService.StopAudio();
        }
        
        private void ChangeVolume(double volume)
        {
            var _volume = (int)volume;
            _soundService.ChangeVolume(_volume);
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

            App.AlarmToneRepo.Tones.Add(newTone);
            App.AlarmToneRepo.SaveTone(newTone);

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
            foreach(var settingTone in AllAlarmTones.ToList())
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

        public void ConfirmDeleteTone(SettingTone settingTone)
        {
            var toneName = settingTone.Name;

            var alarmTone = App.AlarmToneRepo.Tones.Find(a => a.Name == toneName);

            if (alarmTone.IsCustomTone)
            {
                MessageBoxService.ShowConfirm($"알람음 삭제", $"'{toneName}'을 정말 삭제하시겠습니까?", null, () => DeleteTone(settingTone.Name));
            }
            else
            {
                MessageBoxService.ShowAlert("알람음 삭제", $"{toneName}은 삭제하실 수 없습니다.", null);
            }

        }

        private void DeleteTone(string selectedTone)
        {
            var settingTone = AllAlarmTones.ToList().Find(a => a.Name == selectedTone);
            var alarmTone = App.AlarmToneRepo.Tones.Find(a => a.Name == selectedTone);

            if (settingTone.IsSelected)
            {
                ClickTone(AllAlarmTones[0]);
            }

            App.AlarmToneRepo.Tones.Remove(alarmTone);
            App.AlarmToneRepo.DeleteTone(alarmTone);
            AllAlarmTones.Remove(settingTone);
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
