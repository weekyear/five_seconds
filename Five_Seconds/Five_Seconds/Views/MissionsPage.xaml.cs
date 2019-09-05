using Xamarin.Forms;
using Five_Seconds.ViewModels;
using Five_Seconds.Services;
using Five_Seconds.CustomControls;
using Five_Seconds.Models;
using System;

namespace Five_Seconds.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [AdMaiora.RealXaml.Client.RootPage]
    public partial class MissionsPage : ContentPage
    {
        MissionsViewModel viewModel;

        public MissionsPage()
        {
            InitializeComponent();

            viewModel = new MissionsViewModel(Navigation, new MessageBoxService());

            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected void ShowMenuByItemClicked(object sender, ItemTappedEventArgs e)
        {
            viewModel.ShowMenuCommand.Execute(e.Item);
        }
        private void MissionsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (MissionsListView.SelectedItem != null || e.SelectedItem != null)
            {
                ((ListView)sender).SelectedItem = null;
            }
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            var activeSwitch = (Switch)sender;
            var mission = (Mission)activeSwitch.BindingContext;

            //ToastNextAlarm(mission.Alarm);

            App.MissionsRepo.SaveMission(mission);
        }

        private void ToastNextAlarm(Alarm alarm)
        {
            var dateTimeNow = DateTime.Now;
            var nextDate = CalculateNextDate(alarm);
            var nextTime = alarm.Time;

            var nextAlarmDateTime = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, nextTime.Hours, nextTime.Minutes, nextTime.Seconds);

            var diffTimeSpan = nextAlarmDateTime.Subtract(dateTimeNow);

            ShowNextAlarmToast(diffTimeSpan);
        }

        private DateTime CalculateNextDate(Alarm alarm)
        {
            if (DaysOfWeek.GetHasADayBeenSelected(alarm.Days))
            {
                return DateTime.Now.Date.AddDays(CalculateAddingDaysWhenHasDaysOfWeek(alarm));
            }
            else
            {
                return alarm.Date;
            }
        }

        private double CalculateAddingDaysWhenHasDaysOfWeek(Alarm alarm)
        {
            var allDays = alarm.Days.AllDays;

            int addingDays = 8;

            for (int i = 0; i < 7; i++)
            {
                if (allDays[i])
                {
                    var today = (int)DateTime.Now.DayOfWeek;
                    var diffDays = i - today >= 0 ? i - today : i - today + 7;
                    if (addingDays > diffDays)
                    {
                        addingDays = diffDays;
                    }
                }
            }

            return addingDays;
        }

        private void ShowNextAlarmToast(TimeSpan diff)
        {
            var diffString = CreateTimeRemainingString(diff);

            DependencyService.Get<ToastService>().Show(diffString);
        }

        private string CreateTimeRemainingString(TimeSpan diff)
        {
            if (diff.Days > 0)
            {
                return $"{diff.Days + 1}일 후에 5초의 법칙을 실행합니다!";
            }
            else if (diff.Hours > 0)
            {
                return $"{diff.Hours}시간 {diff.Minutes}분 후에 5초의 법칙을 실행합니다!";
            }
            else if (diff.Minutes > 0)
            {
                return $"{diff.Minutes}분 후에 5초의 법칙을 실행합니다!";
            }
            else if (diff.Seconds > 0)
            {
                return $"{diff.Seconds}초 후에 5초의 법칙을 실행합니다!";
            }
            else
            {
                return "이미 지난 시간입니다.";
            }
        }
    }
}