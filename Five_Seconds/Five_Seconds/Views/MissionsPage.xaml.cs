using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Five_Seconds.Models;
using Five_Seconds.Views;
using Five_Seconds.ViewModels;

namespace Five_Seconds.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MissionsPage : ContentPage
    {
        MissionsViewModel viewModel;

        public MissionsPage()
        {
            InitializeComponent();

            viewModel = new MissionsViewModel();

            viewModel.Navigation = Navigation;

            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Missions.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }

        protected async void ShowMenuByItemClicked(object sender, ItemTappedEventArgs e)
        {
            await viewModel.ShowMenu(sender, e);
        }

        protected async void AddMissionClicked(object sender, EventArgs e)
        {
            await viewModel.AddMission();
        }
    }
}