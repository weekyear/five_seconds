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

        private void ToneListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var settingTone = e.Item as SettingTone;

            viewModel.Alarm.Tone = settingTone.Name;
            viewModel.SetIsSelected(settingTone);
        }
    }
}