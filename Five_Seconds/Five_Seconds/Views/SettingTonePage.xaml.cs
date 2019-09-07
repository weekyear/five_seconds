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

namespace Five_Seconds.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingTonePage : ContentPage
    {
        private SettingToneViewModel viewModel;

        public SettingTonePage(INavigation navigation, Mission mission)
        {
            viewModel = new SettingToneViewModel(navigation, mission);

            BindingContext = viewModel;

            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.StopToneCommand.Execute(null);
        }

        private void ToneListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedTone = e.SelectedItem as AlarmTone;
            ToneListView.SelectedItem = e.SelectedItem;
            viewModel.ToneSaveCommand.Execute(selectedTone);
        }

        private void PlayButton_IsPlayingChanged(object sender, EventArgs e)
        {
            var button = sender as PlayButton;
            var tone = button.BindingContext as AlarmTone;

            if (tone.IsPlaying)
            {
                button.ImageSource = button.ImageSourcePause;
            }
            else
            {
                button.ImageSource = button.ImageSourcePlay;
            }
        }
    }
}