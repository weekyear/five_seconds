using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Five_Seconds.Models;
using Five_Seconds.Views;
using Five_Seconds.Services;
using System;
using Five_Seconds.Helpers;

namespace Five_Seconds.ViewModels
{
    public class AlarmsViewModel : BaseViewModel
    {
        private readonly IMessageBoxService MessageBoxService;

        public AlarmsViewModel(INavigation navigation, IMessageBoxService messageBoxService) : base(navigation)
        {
            MessageBoxService = messageBoxService;

            ConstructCommand();

            SubscribeMessage();
        }

        private void ConstructCommand()
        {
            ShowAddAlarmCommand = new Command(async () => await ShowAddAlarm());
            ShowCountDownCommand = new Command(() => ShowCountDown());
            CancelNotifyCommand = new Command(() => CancelNotify());
            ShowAlarmMenuCommand = new Command<object>(async (m) => await ShowAlarmMenu(m));
            ShowMainMenuCommand = new Command(async () => await ShowMainMenu());
        }

        private void SubscribeMessage()
        {
           MessagingCenter.Subscribe<AlarmService>(this, "changeAlarms", (sender) =>
           {
               OnPropertyChanged(nameof(Alarms));
               OnPropertyChanged(nameof(NextAlarmString));
           });
        }

        // Property

        public Command ShowAddAlarmCommand { get; set; }
        public Command ShowCountDownCommand { get; set; }
        public Command CancelNotifyCommand { get; set; }
        public Command<object> ShowAlarmMenuCommand { get; set; }
        public Command ShowMainMenuCommand { get; set; }
        public ObservableCollection<Alarm> Alarms
        {
            get => Service.Alarms;
        }

        public string NextAlarmString
        {
            get => CreateDateString.CreateNextDateTimeString(App.Service.GetNextAlarm());
        }

        public async Task ShowAddAlarm()
        {
            await Navigation.PushAsync(new AlarmPage(Navigation));
        }

        public void ShowCountDown()
        {
            Action action = () => DependencyService.Get<ICountDown>().ShowCountDown();
            MessageBoxService.ShowConfirm("5초 카운트", "5초 카운트를 시작하시겠습니까?", null, action);
        }

        public void CancelNotify()
        {
            DependencyService.Get<IAlarmNotification>().CancelNotification();
        }

        public async Task ShowAlarmMenu(object _alarm)
        {
            var alarm = _alarm as Alarm;
            string[] actionSheetBtns = { "수정", "삭제" };

            string action = await MessageBoxService.ShowActionSheet("알람 옵션", "취소", null, actionSheetBtns);

            await ClickAlarmMenuAction(action, alarm);
        }

        private async Task ClickAlarmMenuAction(string action, Alarm alarm)
        {
            switch (action)
            {
                case "수정":
                    await ShowModifyAlarm(alarm);
                    break;
                case "삭제":
                    Service.DeleteAlarm(alarm);
                    break;
            }
        }

        public async Task ShowMainMenu()
        {
            string[] actionSheetBtns = { "5초의 법칙이란", "5초의 알람 간단 사용법", "알람 기록" };

            string action = await MessageBoxService.ShowActionSheet("메뉴", "취소", null, actionSheetBtns);

            await ClickMenuAction(action);
        }

        private async Task ClickMenuAction(string action)
        {
            switch (action)
            {
                case "5초의 법칙이란":
                    await Navigation.PushAsync(new AboutPage());
                    break;
                case "5초의 알람 간단 사용법":
                    var welcomePage = AppIntro.CreateAppIntro();
                    await Navigation.PushModalAsync(welcomePage);
                    break;
                case "알람 기록":
                    await Navigation.PushAsync(new RecordPage(Navigation));
                    break;
            }
        }

        public async Task ShowModifyAlarm(Alarm alarm)
        {
            App.IsInitFinished = false;
            await Navigation.PushAsync(new AlarmPage(Navigation, alarm));
            App.IsInitFinished = true;
        }
    }
}