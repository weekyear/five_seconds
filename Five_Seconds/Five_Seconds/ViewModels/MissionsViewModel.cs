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
            Title = "5초의 알람";

            MessageBoxService = messageBoxService;

            ConstructCommand();

            SubscribeMessage();
        }

        private void ConstructCommand()
        {
            ShowAddMissionCommand = new Command(async () => await ShowAddMission());
            ShowCountDownCommand = new Command(() => ShowCountDown());
            CancelNotifyCommand = new Command(() => CancelNotify());
            ShowMenuCommand = new Command<object>(async (m) => await ShowMenu(m));
        }

        private void SubscribeMessage()
        {
           MessagingCenter.Subscribe<MissionService>(this, "save", (sender) =>
           {
               OnPropertyChanged(nameof(Missions));
           });

           MessagingCenter.Subscribe<MissionService>(this, "delete", (sender) =>
           {
               OnPropertyChanged(nameof(Missions));
           });
        }

        // Property

        public Command ShowAddMissionCommand { get; set; }
        public Command ShowCountDownCommand { get; set; }
        public Command CancelNotifyCommand { get; set; }
        public Command<object> ShowMenuCommand { get; set; }
        public ObservableCollection<Mission> Missions
        {
            get => Service.Missions;
        }

        public string NextAlarmString
        {
            get => CreateDateString.CreateNextDateTimeString(App.MissionsRepo.GetNextAlarm());
        }

        private string CreateNextAlarmString()
        {
            var alarmId = App.MissionsRepo.GetNextAlarmId();

            string nextNameString;
            string nextTimeString;

            if (alarmId != 0)
            {
                var mission = App.MissionsRepo.GetMission(alarmId);
                var alarm = App.MissionsRepo.GetAlarm(alarmId);

                nextNameString = mission.Name;
                nextTimeString = alarm.NextAlarmTime.ToString();
            }
            else
            {
                nextNameString = "다음 알람이 없습니다.";
                nextTimeString = string.Empty;
            }

            return nextNameString + nextTimeString;
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

        public async Task ShowMenu(object _mission)
        {
            var mission = _mission as Mission;
            string[] actionSheetBtns = { "수정", "삭제" };

            string action = await MessageBoxService.ShowActionSheet("알람 옵션", "취소", null, actionSheetBtns);

            await ClickMenuAction(action, mission);
        }

        private async Task ClickMenuAction(string action, Mission mission)
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