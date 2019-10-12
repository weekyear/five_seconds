using Five_Seconds.CustomControls;
using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Five_Seconds.Views;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class AlarmViewModel : BaseViewModel
    {
        public AlarmViewModel(INavigation navigation) : base(navigation)
        {
            Alarm = new Alarm();

            SubscribeMessage();

            ConstructCommand();
        }

        public AlarmViewModel(INavigation navigation, Alarm alarm) : base(navigation)
        {
            Alarm = new Alarm(alarm);

            SubscribeMessage();

            ConstructCommand();
        }

        private void SubscribeMessage()
        {
            MessagingCenter.Subscribe<DaysOfWeekSelectionView>(this, "dayOfWeek_Clicked", (sender) =>
            {
                Date = SetMinimumDate();
                OnPropertyChanged(nameof(DateString));
            });
        }

        public DateTime SetMinimumDate()
        {
            if (Time.Subtract(DateTime.Now.TimeOfDay).Ticks < 0)
            {
                return DateTime.Now.AddDays(1).Date;
            }
            else
            {
                return DateTime.Now.Date;
            }
        }

        private void ConstructCommand()
        {
            CloseCommand = new Command(async () => await ClosePopup());
            SaveCommand = new Command(async () => await Save());
            ShowSettingToneCommand = new Command(async () => await ShowSettingTone());
        }

        // Command
        public Command CloseCommand { get; set; }
        public Command SaveCommand { get; set; }
        public Command ShowSettingToneCommand { get; set; }

        // Property

        public Alarm Alarm
        {
            get; set;
        }

        public string Name
        {
            get { return Alarm.Name; }
            set
            {
                if (Alarm.Name == value) return;
                Alarm.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public DateTime Date
        {
            get
            {
                return Alarm.Date;
            }
            set
            {
                if (Alarm.Date == value) return;
                Alarm.Date = value;
                OnPropertyChanged(nameof(DateString));
                OnPropertyChanged(nameof(Date));
            }
        }

        public TimeSpan Time
        {
            get
            {
                if (Alarm.Time == TimeSpan.Zero)
                {
                    Alarm.Time = DateTime.Now.TimeOfDay;
                }
                return Alarm.Time;
            }
            set
            {
                if (Alarm.Time == value) return;
                Alarm.Time = value;
                DateToStringWhenTimeChanged();
                OnPropertyChanged(nameof(Time));
            }
        }
        public DaysOfWeek Days
        {
            get { return Alarm.Days; }
            set
            {
                if (Alarm.Days == value) return;
                Alarm.Days = value;
                OnPropertyChanged(nameof(Days));
            }
        }

        //public bool IsAlarmOn
        //{
        //    get { return Alarm.IsAlarmOn; }
        //    set
        //    {
        //        if (Alarm.IsAlarmOn == value) return;
        //        Alarm.IsAlarmOn = value;
        //        OnPropertyChanged(nameof(IsAlarmOn));
        //    }
        //}

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

        //public bool IsVibrateOn
        //{
        //    get { return Alarm.IsVibrateOn; }
        //    set
        //    {
        //        if (Alarm.IsVibrateOn == value) return;
        //        Alarm.IsVibrateOn = value;
        //        OnPropertyChanged(nameof(IsVibrateOn));
        //    }
        //}

        public bool IsCountOn
        {
            get { return Alarm.IsCountOn; }
            set
            {
                if (Alarm.IsCountOn == value) return;
                Alarm.IsCountOn = value;
                IsCountSoundOn = value;
                OnPropertyChanged(nameof(IsCountOn));
            }
        }

        public bool IsCountSoundOn
        {
            get { return Alarm.IsCountSoundOn; }
            set
            {
                if (Alarm.IsCountSoundOn == value) return;
                Alarm.IsCountSoundOn = value;
                OnPropertyChanged(nameof(IsCountSoundOn));
            }
        }

        public string DateString
        {
            get
            {
                return Alarm.DateString;
            }
        }

        private void DateToStringWhenTimeChanged()
        {
            if (Alarm.IsToday && Time.Subtract(DateTime.Now.TimeOfDay).Ticks < 0)
            {
                Date = DateTime.Now.AddDays(1).Date;
            }
            else if (Alarm.IsToday && Time.Subtract(DateTime.Now.TimeOfDay).Ticks > 0)
            {
                Date = DateTime.Now.Date;
            }
        }

        // Validation
        public bool HasDayBeenSelected { get; set; } = true;

        // Methods

        private async Task ClosePopup()
        {
            await Navigation.PopAsync(true);
        }

        private async Task Save()
        {
            if (string.IsNullOrEmpty(Name))
            {
                await Application.Current.MainPage.DisplayAlert("", "미션 이름을 깜빡하셨어요!", "확인");
            }
            else if (Alarm.TimeOffset.Subtract(DateTime.Now).Ticks < 0 && !DaysOfWeek.GetHasADayBeenSelected(Alarm.Days))
            {
                await Application.Current.MainPage.DisplayAlert("", "이미 지난 시간으로 설정하셨어요!", "확인");
            }
            else
            {
                if (!Alarm.IsActive)
                {
                    App.IsInitFinished = false;
                    Alarm.IsActive = true;
                    App.IsInitFinished = true;
                }
                Service.SaveAlarm(Alarm);
                await ClosePopup();
            }
        }

        private async Task ShowSettingTone()
        {
            await Navigation.PushAsync(new SettingTonePage(Navigation, Alarm));
        }
    }
}
