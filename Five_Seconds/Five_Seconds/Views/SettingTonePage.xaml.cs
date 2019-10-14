using Five_Seconds.CustomControls;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Five_Seconds.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Five_Seconds.ViewModels.SettingToneViewModel;

namespace Five_Seconds.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingTonePage : ContentPage
    {
        private readonly SettingToneViewModel viewModel;

        public SettingTonePage(INavigation navigation, Alarm alarm)
        {
            viewModel = new SettingToneViewModel(navigation, alarm);

            BindingContext = viewModel;

            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.StopToneCommand.Execute(null);
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.SetAllAlarmTones();
        }

        private void ToneListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            viewModel.SelectedTone = e.SelectedItem as SettingTone;

            viewModel.ToneSaveCommand.Execute(viewModel.SelectedTone);
        }

        private void ToneListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            viewModel.SelectedTone = e.Item as SettingTone;

            viewModel.ChangeIsSelected();
        }
    }
}