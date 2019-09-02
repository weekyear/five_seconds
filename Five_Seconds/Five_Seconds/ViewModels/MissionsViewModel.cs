using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Five_Seconds.Models;
using Five_Seconds.Views;
using Five_Seconds.Services;

namespace Five_Seconds.ViewModels
{
    public class MissionsViewModel : BaseViewModel
    {
        private readonly IMessageBoxService MessageBoxService;
        public MissionsViewModel(INavigation navigation, IMessageBoxService messageBoxService) : base(navigation)
        {
            Title = "자, 5초 센다";

            MessageBoxService = messageBoxService;

            ConstructCommand();

            SubscribeMessage();
        }

        private void ConstructCommand()
        {
            ShowAddMissionCommand = new Command(async () => await ShowAddMission());
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
        public ObservableCollection<Mission> Missions
        {
            get => Service.Missions;
        }

        public Command ShowAddMissionCommand { get; set; }
        public Command<object> ShowMenuCommand { get; set; }

        public async Task ShowAddMission()
        {
            await Navigation.PushAsync(new MissionPage(Navigation));
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