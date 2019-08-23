using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Five_Seconds.Models;
using Five_Seconds.Views;
using System.Collections.Generic;
using Rg.Plugins.Popup.Contracts;
using Five_Seconds.Services;
using Five_Seconds.Repository;

namespace Five_Seconds.ViewModels
{
    public class MissionsViewModel : BaseViewModel
    {
        private readonly IMessageBoxService MessageBoxService;
        private readonly IPopupNavigation PopupNavigation;
        public MissionsViewModel(INavigation navigation, IMissionsRepository missionsRepo, IMessageBoxService messageBoxService, IPopupNavigation popupNavigation) : base(navigation, missionsRepo)
        {
            Title = "자, 5초 준다";

            MessageBoxService = messageBoxService;
            PopupNavigation = popupNavigation;

            MissionRepo.DeleteAllMissions();

            //InitMissions();

            ConstructCommand();
        }

        private void InitMissions()
        {
            if (Device.RuntimePlatform == "Test") return;

            MissionRepo.DeleteAllMissions();

            var missionsList = MissionRepo.GetMissions() as List<Mission>;

            if (missionsList.Count == 0)
            {
                Record record1 = new Record() { Date = DateTime.UtcNow, IsSuccess = false, RecordTime = 5 };
                Record record2 = new Record() { Date = DateTime.UtcNow.AddDays(1), IsSuccess = false, RecordTime = 3 };
                Record record3 = new Record() { Date = DateTime.UtcNow.AddDays(2), IsSuccess = true, RecordTime = 7 };

                var records = new ObservableCollection<Record>();
                records.Add(record3);
                records.Add(record2);
                records.Add(record1);

                missionsList = new List<Mission>
                {
                    new Mission { Name = "일어나기", Alarm = new Alarm(new TimeSpan(1, 20, 00)), Percentage = 0.835, Records = records },
                    new Mission { Name = "운동하기", Alarm = new Alarm(new TimeSpan(2, 30, 00)), Percentage = 0.434 },
                    new Mission { Name = "공부하기", Alarm = new Alarm(new TimeSpan(3, 40, 00)), Percentage = 0.025 },
                    new Mission { Name = "잠자기", Alarm = new Alarm(new TimeSpan(4, 50, 00)), Percentage = 1.00 }
                };

                foreach (var item in missionsList)
                {
                    MissionRepo.SaveMission(item);
                }
            }
            else
            {
                foreach (var item in missionsList)
                {
                    MissionRepo.Missions.Add(item);
                }
            }
        }

        private void ConstructCommand()
        {
            ShowAddMissionCommand = new Command(async () => await ShowAddMission());
            ShowMenuCommand = new Command<object>(async (m) => await ShowMenu(m));
        }

        // Property
        public ObservableCollection<Mission> Missions
        {
            get => MissionRepo.Missions;
        }

        public Command ShowAddMissionCommand { get; set; }
        public Command<object> ShowMenuCommand { get; set; }

        public async Task ShowAddMission()
        {
            await PopupNavigation.PushAsync(new MissionPopupPage(Navigation, MissionRepo, PopupNavigation));
        }

        public async Task ShowMenu(object _mission)
        {
            var mission = _mission as Mission;
            string[] actionSheetBtns = { "Modify", "Record", "Delete" };

            string action = await MessageBoxService.ShowActionSheet("Options", "Cancel", null, actionSheetBtns);

            await ClickMenuAction(action, mission);
        }

        private async Task ClickMenuAction(string action, Mission mission)
        {
            switch (action)
            {
                case "Modify":
                    await ShowModifyMission(mission);
                    break;
                case "Record":
                    await ShowMissionRecord(mission);
                    break;
                case "Delete":
                    MissionRepo.DeleteMission(mission.Id);
                    break;
            }
        }

        public async Task ShowModifyMission(Mission mission)
        {
            await PopupNavigation.PushAsync(new MissionPopupPage(Navigation, MissionRepo, mission, PopupNavigation));
        }

        private async Task ShowMissionRecord(Mission mission)
        {
            await Navigation.PushAsync(new RecordPage(new RecordViewModel(base.Navigation, base.MissionRepo, mission)));
        }

    }
}