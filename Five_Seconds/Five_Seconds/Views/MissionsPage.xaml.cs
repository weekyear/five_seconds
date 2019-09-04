using Xamarin.Forms;
using Five_Seconds.ViewModels;
using Five_Seconds.Services;
using Five_Seconds.CustomControls;
using Five_Seconds.Models;

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

            App.MissionsRepo.SaveMission(mission);
        }
    }
}