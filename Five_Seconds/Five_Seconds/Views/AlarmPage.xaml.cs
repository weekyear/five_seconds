using Five_Seconds.CustomControls;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Five_Seconds.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Five_Seconds.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlarmPage : ContentPage
    {
        readonly AlarmViewModel viewModel;

        public AlarmPage(INavigation navigation)
        {
            viewModel = new AlarmViewModel(navigation);

            BindingContext = viewModel;

            InitializeComponent();
        }

        public AlarmPage(INavigation navigation, Alarm alarm)
        {
            viewModel = new AlarmViewModel(navigation, alarm);

            BindingContext = viewModel;

            InitializeComponent();
        }

        private void CalendarButton_Clicked(object sender, EventArgs e)
        {
            DatePicker.MinimumDate = viewModel.SetMinimumDate();
            DatePicker.Date = viewModel.Date;
            DatePicker.Focus();
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            var _switch = sender as Switch;
            if (_switch.IsToggled)
            {
                _switch.ThumbColor = Color.SkyBlue;
            }
            else
            {
                _switch.ThumbColor = Color.LightGray;
            }
        }

        private void DatePicker_Unfocused(object sender, FocusEventArgs e)
        {
            if (DatePicker.Date.Subtract(DateTime.Now.Date).Days == 0)
            {
                viewModel.IsToday = true;
            }
            else
            {
                viewModel.IsToday = false;
            }
        }

        private void TypeButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as TypeButton;
            viewModel.AlarmType = button.ButtonType;
        }
    }
}