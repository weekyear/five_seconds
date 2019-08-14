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

namespace Five_Seconds.ViewModels
{
    public class MissionsViewModel : BaseViewModel
    {
        private INavigation Navigation;
        public MissionsViewModel(INavigation navigation)
        {
            Title = "자, 5초 준다";

            Navigation = navigation;

            InitMissions();

            ConstructCommand();
        }

        private void InitMissions()
        {
            repository.DeleteAllMissions();

            var missionsList = repository.GetMissions() as List<Mission>;

            if (missionsList.Count == 0)
            {
                var dateNow = DateTime.UtcNow;

                Record record1 = new Record() { Date = DateTime.UtcNow, IsSuccess = false, RecordTime = 5 };
                Record record2 = new Record() { Date = DateTime.UtcNow.AddDays(1), IsSuccess = false, RecordTime = 3 };
                Record record3 = new Record() { Date = DateTime.UtcNow.AddDays(2), IsSuccess = true, RecordTime = 7 };

                var records = new ObservableCollection<Record>();
                records.Add(record3);
                records.Add(record2);
                records.Add(record1);

                var mockItems = new List<Mission>
                {
                    //new Mission { Description = "일어나기", Time = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 1, 20, 00), Percentage = "80%", Records = records },
                    new Mission { Description = "일어나기", TimeOfDay = new TimeSpan(1, 20, 00), Percentage = "80%" },
                    new Mission { Description = "운동하기", TimeOfDay = new TimeSpan(2, 30, 00), Percentage = "40%" },
                    new Mission { Description = "공부하기", TimeOfDay = new TimeSpan(3, 40, 00), Percentage = "50%" },
                    new Mission { Description = "잠자기", TimeOfDay = new TimeSpan(4, 50, 00), Percentage = "70%" }
                };

                foreach (var item in mockItems)
                {
                    repository.SaveMission(item);
                }
            }
        }

        private void ConstructCommand()
        {
            AddMissionCommand = new Command(async () => await AddMission());
            ShowMenuCommand = new Command<object>(async (m) => await ShowMenu(m));
        }

        // Property

        public ObservableCollection<Mission> Missions
        {
            get => repository.Missions;
        }

        public Command AddMissionCommand { get; set; }
        public Command<object> ShowMenuCommand { get; set; }

        public async Task AddMission()
        {
            await PopupNavigation.Instance.PushAsync(new MissionPopupPage());
        }

        public async Task ShowMenu(object _mission)
        {
            var mission = _mission as Mission;
            string[] actionSheetBtns = { "Modify", "Record", "Delete" };

            string action = await MessageBoxService.ShowActionSheet("Options", "Cancel", null, actionSheetBtns);

            switch (action)
            {
                case "Modify":
                    await PopupNavigation.Instance.PushAsync(new MissionPopupPage(mission));
                    break;
                case "Record":
                    await ShowMissionRecord(mission);
                    break;
                case "Delete":
                    repository.DeleteMission(mission.Id);
                    break;
            }
        }

        private async Task ShowMissionRecord(Mission mission)
        {
            await Navigation.PushAsync(new RecordPage(new RecordViewModel(mission)));
        }

    }
}