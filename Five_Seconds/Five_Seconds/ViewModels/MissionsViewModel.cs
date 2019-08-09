using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using Five_Seconds.Models;
using Five_Seconds.Views;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;

namespace Five_Seconds.ViewModels
{
    public class MissionsViewModel : BaseViewModel
    {
        public MissionsViewModel()
        {
            Title = "자, 5초 준다";
            Missions = new ObservableCollection<Mission>();

            InitMissions();

            ConstructCommand();
        }

        private void ConstructCommand()
        {
            LoadItemsCommand = new Command(() => ExecuteLoadItemsCommand());
            AddMissionCommand = new Command(async () => await AddMission());
        }

        private void InitMissions()
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
                new Mission { Description = "일어나기", Time = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 1, 20, 00), Percentage = "80%" },
                new Mission { Description = "운동하기", Time = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 2, 30, 00), Percentage = "40%" },
                new Mission { Description = "공부하기", Time = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 3, 40, 00), Percentage = "50%" },
                new Mission { Description = "잠자기", Time = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 4, 50, 00), Percentage = "70%" }
            };

            foreach (var item in mockItems)
            {
                repository.SaveMission(item);
            }

            var missions = repository.GetMissions();

            foreach (var mission in missions)
            {
                Missions.Add(mission);
            }
        }

        void ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Missions.Clear();
                var items = repository.GetMissions();
                foreach (var item in items)
                {
                    Missions.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ShowMissionRecord(Mission mission)
        {
            await Navigation.PushAsync(new RecordPage(new RecordViewModel(mission)));
        }

        public async Task ShowMenu(object sender, ItemTappedEventArgs e)
        {
            Mission mission = (Mission)e.Item;
            string description = mission.Description;
            string time = mission.Time.ToShortTimeString();
            string percentage = mission.Percentage;
            string[] actionSheetBtns = { "Modify", "Record", "Delete" };
            string action = await MessageBoxService.ShowActionSheet("Options", "Cancel", null, actionSheetBtns);

            switch (action)
            {
                case "Modify":
                    await PopupNavigation.Instance.PushAsync(new MissionPopupPage());
                    break;
                case "Record":
                    await ShowMissionRecord(mission);
                    break;
                case "Delete":

                    break;
            }
        }

        public async Task AddMission()
        {
            await PopupNavigation.Instance.PushAsync(new MissionPopupPage());
        }


        public INavigation Navigation;
        public ObservableCollection<Mission> Missions { get; set; }
        public Command LoadItemsCommand { get; set; }

        public Command AddMissionCommand { get; set; }
    }
}