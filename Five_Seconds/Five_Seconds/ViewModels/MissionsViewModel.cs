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
    public class MissionsViewModel : BaseViewModel
    {
        private readonly IMessageBoxService MessageBoxService;

        public MissionsViewModel(INavigation navigation, IMessageBoxService messageBoxService) : base(navigation)
        {
            MessageBoxService = messageBoxService;

            ConstructCommand();

            SubscribeMessage();
        }

        private void ConstructCommand()
        {
            ShowAddMissionCommand = new Command(async () => await ShowAddMission());
            ShowCountDownCommand = new Command(() => ShowCountDown());
            CancelNotifyCommand = new Command(() => CancelNotify());
            ShowMissionMenuCommand = new Command<object>(async (m) => await ShowMissionMenu(m));
            ShowMainMenuCommand = new Command(async () => await ShowMainMenu());
        }

        private void SubscribeMessage()
        {
           MessagingCenter.Subscribe<MissionService>(this, "changeMissions", (sender) =>
           {
               OnPropertyChanged(nameof(Missions));
               OnPropertyChanged(nameof(NextAlarmString));
           });
        }

        // Property

        public Command ShowAddMissionCommand { get; set; }
        public Command ShowCountDownCommand { get; set; }
        public Command CancelNotifyCommand { get; set; }
        public Command<object> ShowMissionMenuCommand { get; set; }
        public Command ShowMainMenuCommand { get; set; }
        public ObservableCollection<Mission> Missions
        {
            get => Service.Missions;
        }

        public string NextAlarmString
        {
            get => CreateDateString.CreateNextDateTimeString(App.Service.GetNextAlarm());
        }

        public async Task ShowAddMission()
        {
            await Navigation.PushAsync(new MissionPage(Navigation));
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

        public async Task ShowMissionMenu(object _mission)
        {
            var mission = _mission as Mission;
            string[] actionSheetBtns = { "수정", "삭제" };

            string action = await MessageBoxService.ShowActionSheet("알람 옵션", "취소", null, actionSheetBtns);

            await ClickMissionMenuAction(action, mission);
        }

        private async Task ClickMissionMenuAction(string action, Mission mission)
        {
            switch (action)
            {
                case "수정":
                    await ShowModifyMission(mission);
                    break;
                case "Record":
                    await ShowMissionRecord(mission);
                    break;
                case "삭제":
                    Service.DeleteMission(mission);
                    break;
            }
        }

        public async Task ShowMainMenu()
        {
            string[] actionSheetBtns = { "5초의 법칙이란", "5초의 알람 간단 사용법" };

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
            }
        }

        public async Task ShowModifyMission(Mission mission)
        {
            await Navigation.PushAsync(new MissionPage(Navigation, mission));
        }

        private async Task ShowMissionRecord(Mission mission)
        {
            await Navigation.PushAsync(new RecordPage(new RecordViewModel(base.Navigation, mission)));
        }
    }
}