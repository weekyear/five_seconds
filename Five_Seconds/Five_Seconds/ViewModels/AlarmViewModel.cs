﻿using Five_Seconds.CustomControls;
using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Resources;
using Five_Seconds.Services;
using Five_Seconds.Views;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class AlarmViewModel : BaseViewModel
    {
        public AlarmViewModel(INavigation navigation, Alarm alarm) : base(navigation)
        {
            Alarm = new Alarm(alarm);

            InitTimePicker();

            SubscribeMessage();

            ConstructCommand();

            ResetWhenAlarmTypeChanged(AlarmType);
        }

        private void InitTimePicker()
        {
            if (Alarm.Time.Hours == 12)
            {
                AmPm = 1;
                Hours = Alarm.Time.Hours;
            }
            else if (Alarm.Time.Hours == 0)
            {
                AmPm = 0;
                Hours = Alarm.Time.Hours + 12;
            }
            else if (Alarm.Time.Hours < 12)
            {
                AmPm = 0;
                Hours = Alarm.Time.Hours;
            }
            else
            {
                AmPm = 1;
                Hours = Alarm.Time.Hours - 12;
            }
            Minutes = Alarm.Time.Minutes;
        }

        private void SubscribeMessage()
        {
            MessagingCenter.Subscribe<DaysOfWeekSelectionView>(this, "dayOfWeek_Clicked", (sender) =>
            {
                IsToday = true;
                Date = SetMinimumDate();
                OnPropertyChanged(nameof(DateString));
            });

            MessagingCenter.Subscribe<SettingToneViewModel, Alarm>(this, "changeAlarmTone", (sender, alarm) =>
            {
                Alarm.Tone = alarm.Tone;
                Alarm.Volume = alarm.Volume;
            });
            
            MessagingCenter.Subscribe<AppListPage, AppPackage>(this, "changeAppPkg", (sender, appPkg) =>
            {
                Alarm.PackageName = appPkg.PackageName;
                Alarm.AppLabel = appPkg.Label;
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
            OpenAppListCommand = new Command(async() => await OpenAppList());
        }

        // Command
        public Command CloseCommand { get; set; }
        public Command SaveCommand { get; set; }
        public Command ShowSettingToneCommand { get; set; }
        public Command OpenAppListCommand { get; set; }

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
        
        public string WakeUpText
        {
            get { return Alarm.WakeUpText; }
            set
            {
                if (Alarm.WakeUpText == value) return;
                Alarm.WakeUpText = value;
                OnPropertyChanged(nameof(WakeUpText));
            }
        }

        public string PackageName
        {
            get { return Alarm.PackageName; }
            set
            {
                if (Alarm.PackageName == value) return;
                Alarm.PackageName = value;
                OnPropertyChanged(nameof(PackageName));
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
                if (AmPm == 0)
                {
                    if (Hours == 12)
                    {
                        return new TimeSpan(Hours - 12, Minutes, 0);
                    }
                    else
                    {
                        return new TimeSpan(Hours, Minutes, 0);
                    }
                }
                else
                {
                    if (Hours == 12)
                    {
                        return new TimeSpan(Hours, Minutes, 0);
                    }
                    else
                    {
                        return new TimeSpan(Hours + 12, Minutes, 0);
                    }
                }
            }
        }

        private int amPm;
        public int AmPm
        {
            get
            {
                return amPm;
            }
            set
            {
                if (amPm == value) return;
                amPm = value;
                DateToStringWhenTimeChanged();
                OnPropertyChanged(nameof(AmPm));
            }
        }

        private int hours;
        public int Hours
        {
            get
            {
                return hours;
            }
            set
            {
                if (hours == value) return;
                if (hours == 11 && value == 12 || hours == 12 && value == 11)
                {
                    if (AmPm == 1)
                    {
                        AmPm = 0;
                    }
                    else
                    {
                        AmPm = 1;
                    }
                }
                hours = value;
                DateToStringWhenTimeChanged();
                OnPropertyChanged(nameof(Hours));
            }
        }

        private int minutes;
        public int Minutes
        {
            get
            {
                return minutes;
            }
            set
            {
                if (minutes == value) return;
                minutes = value;
                DateToStringWhenTimeChanged();
                OnPropertyChanged(nameof(Minutes));
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

        public string DateString
        {
            get
            {
                return Alarm.DateString;
            }
        }
        public bool IsToday { get; set; } = true;

        private void DateToStringWhenTimeChanged()
        {
            if (IsToday)
            {
                int changedDay;

                if (Time.Subtract(DateTime.Now.TimeOfDay).Ticks < 0)
                {
                    changedDay = 1;
                }
                else
                {
                    changedDay = 0;
                }

                Date = DateTime.Now.AddDays(changedDay).Date;
            }
        }

        public string AlarmDescription
        {
            get; set;
        }

        public int AlarmType
        {
            get { return Alarm.AlarmType; }
            set
            {
                if (Alarm.AlarmType == value) return;
                Alarm.AlarmType = value;
                ResetWhenAlarmTypeChanged(value);
                OnPropertyChanged(nameof(AlarmType));
            }
        }

        private void ResetWhenAlarmTypeChanged(int alarmType)
        {
            switch (alarmType)
            {
                case 0:
                    AlarmDescription = AppResources.SimpleAlarmDescription;
                    break;
                case 1:
                    AlarmDescription = AppResources.VoiceAlarmDescription;
                    break;
                case 2:
                    AlarmDescription = AppResources.DoNotDelayAlarmDescription;
                    break;
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
            if (!string.IsNullOrEmpty(Name)) Name = Name.TrimStart().TrimEnd();
            if (!string.IsNullOrEmpty(WakeUpText)) WakeUpText = WakeUpText.TrimStart().TrimEnd();
            if (!string.IsNullOrEmpty(PackageName)) PackageName = PackageName.TrimStart().TrimEnd();

            Alarm.TimeOffset = new DateTime(Date.Year, Date.Month, Date.Day, Time.Hours, Time.Minutes, 0);

            if (string.IsNullOrEmpty(Name))
            {
                await Application.Current.MainPage.DisplayAlert("", AppResources.ForgotAlarmName, AppResources.OK);
            }
            else if (Alarm.TimeOffset.Subtract(DateTime.Now).Ticks < 0 && !DaysOfWeek.GetHasADayBeenSelected(Alarm.Days))
            {
                await Application.Current.MainPage.DisplayAlert("", AppResources.SetTimePast, AppResources.OK);
            }
            else if (string.IsNullOrEmpty(WakeUpText) && Alarm.HasWakeUpText)
            {
                await Application.Current.MainPage.DisplayAlert("", AppResources.ForgotWordsThatWakeMeUp, AppResources.OK);
            }
            else if (string.IsNullOrEmpty(PackageName) && Alarm.IsLinkOtherApp)
            {
                await Application.Current.MainPage.DisplayAlert("", AppResources.ForgotLinkOtherApps, AppResources.OK);
            }
            else
            {
                if (!Alarm.IsActive) { Alarm.ChangeIsActive(Alarm, true); }
                Alarm.IsLaterAlarm = false;
                Alarm.IsGoOffPreAlarm = false;

                var id = Service.SaveAlarm(Alarm);
                
                if (Preferences.Get("MaxAlarmId", 3) < id)
                {
                    Preferences.Set("MaxAlarmId", id);
                }

                await ClosePopup();

                var diffString = CreateDateString.CreateTimeRemainingString(Alarm.NextAlarmTime);
                DependencyService.Get<IToastService>().Show(diffString);
            }
        }

        private async Task ShowSettingTone()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                await Navigation.PushAsync(new SettingTonePage(Navigation, Alarm));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OpenAppList()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                var pkgList = DependencyService.Get<IOpenAppList>().OpenAppList();

                await Navigation.PushAsync(new AppListPage(pkgList));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
