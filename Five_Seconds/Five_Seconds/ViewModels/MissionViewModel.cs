using Five_Seconds.CustomControls;
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
    public class MissionViewModel : BaseViewModel
    {
        public MissionViewModel(INavigation navigation) : base(navigation)
        {
            Mission = new Mission();

            SubscribeMessage();

            ConstructCommand();
        }

        public MissionViewModel(INavigation navigation, Mission mission) : base(navigation)
        {
            Mission = new Mission(mission);

            SubscribeMessage();

            ConstructCommand();
        }

        private void SubscribeMessage()
        {
            MessagingCenter.Subscribe<DaysOfWeekSelectionView>(this, "dayOfWeek_Clicked", (sender) =>
            {
                OnPropertyChanged(nameof(DateString));
            });
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
        public Mission Mission
        {
            get; set;
        }

        public string Name
        {
            get { return Mission.Name; }
            set
            {
                if (Mission.Name == value) return;
                Mission.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public Alarm Alarm
        {
            get { return Mission.Alarm; }
            set
            {
                if (Mission.Alarm == value) return;
                Mission.Alarm = value;
                OnPropertyChanged(nameof(Alarm));
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
                if (Mission.Alarm.Time == TimeSpan.Zero)
                {
                    Mission.Alarm.Time = DateTime.Now.TimeOfDay;
                }
                return Mission.Alarm.Time;
            }
            set
            {
                if (Mission.Alarm.Time == value) return;
                Mission.Alarm.Time = value;
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

        public bool IsAlarmOn
        {
            get { return Mission.Alarm.IsAlarmOn; }
            set
            {
                if (Mission.Alarm.IsAlarmOn == value) return;
                Mission.Alarm.IsAlarmOn = value;
                OnPropertyChanged(nameof(IsAlarmOn));
            }
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

        public bool IsVibrateOn
        {
            get { return Mission.Alarm.IsVibrateOn; }
            set
            {
                if (Mission.Alarm.IsVibrateOn == value) return;
                Mission.Alarm.IsVibrateOn = value;
                OnPropertyChanged(nameof(IsVibrateOn));
            }
        }

        private string dateString;
        public string DateString
        {
            get
            {
                dateString = DateToString();
                return dateString;
            }
            set
            {
                if (dateString == value) return;
                dateString = value;
                OnPropertyChanged(nameof(DateString));
            }
        }

        public string DateToString()
        {
            if (DaysOfWeek.GetHasADayBeenSelected(Alarm.Days))
            {
                return ConvertDaysOfWeekToString();
            }

            return ConvertDateToString(Date);
        }

        private string ConvertDaysOfWeekToString()
        {
            var stringBuilder = new StringBuilder();

            var allDays = Alarm.Days.AllDays;
            var allDaysString = Alarm.Days.AllDaysString;

            for (int i = 0; i < 7; i++)
            {
                if (allDays[i])
                {
                    stringBuilder.Append($", {allDaysString[i]}");
                }
            }
            stringBuilder.Remove(0, 2);

            return stringBuilder.ToString();
        }

        private void DateToStringWhenTimeChanged()
        {
            if (Date.Date == DateTime.Now.Date && Time.Subtract(DateTime.Now.TimeOfDay).Ticks < 0)
            {
                Date = Date.AddDays(1);
            }
            else if (Date.Date.Subtract(DateTime.Now.Date).Days == 1 && Time.Subtract(DateTime.Now.TimeOfDay).Ticks > 0)
            {
                Date = Date.AddDays(-1);
            }
        }

        private string ConvertDateToString(DateTime date)
        {
            if (date.Subtract(DateTime.Now).Days == 1)
            {
                return $"내일-{date.ToShortDateString()},({date.DayOfWeek})";
            }

            var dateTime = $"{date.ToShortDateString()},({date.DayOfWeek})";
            return dateTime;
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
            else
            {
                Service.SaveMission(Mission);
                await ClosePopup();
            }
        }

        private async Task ShowSettingTone()
        {
            await Navigation.PushAsync(new SettingTonePage(Navigation, Mission));
        }
    }
}
