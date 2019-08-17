using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using Five_Seconds.Models;
using Five_Seconds.Views;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using Five_Seconds.Repository;
using Rg.Plugins.Popup.Contracts;
using Five_Seconds.Services;

namespace Five_Seconds.ViewModels
{
    public class MissionsViewModel : BaseViewModel
    {
        private readonly IMessageBoxService MessageBoxService;
        private readonly IPopupNavigation PopupNavigation;
        public MissionsViewModel(INavigation navigation, ILocalData localData, IMessageBoxService messageBoxService, IPopupNavigation popupNavigation) : base(navigation, localData)
        {
            Title = "자, 5초 준다";

            MessageBoxService = messageBoxService;
            PopupNavigation = popupNavigation;

            InitMissions();

            ConstructCommand();
        }

        private void InitMissions()
        {
            if (Device.RuntimePlatform == "Test") return;

            LocalData.DeleteAllMissions();

            var missionsList = LocalData.GetMissions() as List<Mission>;

            if (missionsList.Count == 0)
            {
                Record record1 = new Record() { Date = DateTime.UtcNow, IsSuccess = false, RecordTime = 5 };
                Record record2 = new Record() { Date = DateTime.UtcNow.AddDays(1), IsSuccess = false, RecordTime = 3 };
                Record record3 = new Record() { Date = DateTime.UtcNow.AddDays(2), IsSuccess = true, RecordTime = 7 };

                var records = new ObservableCollection<Record>();
                records.Add(record3);
                records.Add(record2);
                records.Add(record1);

                var mockItems = new List<Mission>
                {
                    new Mission { Description = "일어나기", TimeOfDay = new TimeSpan(1, 20, 00), Percentage = 0.835, Records = records },
                    new Mission { Description = "운동하기", TimeOfDay = new TimeSpan(2, 30, 00), Percentage = 0.434 },
                    new Mission { Description = "공부하기", TimeOfDay = new TimeSpan(3, 40, 00), Percentage = 0.025 },
                    new Mission { Description = "잠자기", TimeOfDay = new TimeSpan(4, 50, 00), Percentage = 1.00 }
                };

                foreach (var item in mockItems)
                {
                    LocalData.SaveMission(item);
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
            get => LocalData.Missions;
        }

        public Command ShowAddMissionCommand { get; set; }
        public Command<object> ShowMenuCommand { get; set; }

        public async Task ShowAddMission()
        {
            await PopupNavigation.PushAsync(new MissionPopupPage(Navigation, LocalData, PopupNavigation));
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
                    LocalData.DeleteMission(mission.Id);
                    break;
            }
        }

        public async Task ShowModifyMission(Mission mission)
        {
            await PopupNavigation.PushAsync(new MissionPopupPage(Navigation, LocalData, mission, PopupNavigation));
        }

        private async Task ShowMissionRecord(Mission mission)
        {
            await Navigation.PushAsync(new RecordPage(new RecordViewModel(base.Navigation, base.LocalData, mission)));
        }

    }
}