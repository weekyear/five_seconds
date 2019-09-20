using Xamarin.Forms;
using Five_Seconds.ViewModels;
using Five_Seconds.Services;
using Five_Seconds.CustomControls;
using Five_Seconds.Models;
using System;
using System.Runtime.CompilerServices;
using Five_Seconds.Helpers;

namespace Five_Seconds.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [AdMaiora.RealXaml.Client.RootPage]
    public partial class AlarmsPage : ContentPage
    {
        AlarmsViewModel viewModel;

        public AlarmsPage()
        {
            InitializeComponent();

            viewModel = new AlarmsViewModel(Navigation, new MessageBoxService());

            BindingContext = viewModel;
        }

        protected void ShowMenuByItemClicked(object sender, ItemTappedEventArgs e)
        {
            viewModel.ShowAlarmMenuCommand.Execute(e.Item);
        }

        private void AlarmsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (AlarmsListView.SelectedItem != null || e.SelectedItem != null)
            {
                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}