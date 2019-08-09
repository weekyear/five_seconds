using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using Five_Seconds.Models;
using Five_Seconds.Views;
using Rg.Plugins.Popup.Services;

namespace Five_Seconds.ViewModels
{
    public class MissionsViewModel : BaseViewModel
    {
        public MissionsViewModel()
        {
            Title = "자, 5초 준다";
            Items = new ObservableCollection<Mission>();

            ConstructCommand();
        }

        private void ConstructCommand()
        {
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddMissionCommand = new Command(async () => await AddMission());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
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
        public ObservableCollection<Mission> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public Command AddMissionCommand { get; set; }
    }
}