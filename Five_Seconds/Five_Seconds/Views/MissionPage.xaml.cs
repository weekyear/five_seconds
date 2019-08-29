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
    public partial class MissionPage : ContentPage
    {
        MissionViewModel viewModel;

        public MissionPage(INavigation navigation)
        {
            viewModel = new MissionViewModel(navigation);

            BindingContext = viewModel;

            InitializeComponent();
        }

        public MissionPage(INavigation navigation, Mission mission)
        {
            viewModel = new MissionViewModel(navigation, mission);

            BindingContext = viewModel;

            InitializeComponent();
        }

        void OnIsSelectedChanged(object sender, ClickedEventArgs e)
        {
            var button = sender as DayOfWeekButton;

            button.IsSelected = !button.IsSelected;

            if (button.IsSelected == true)
            {
                button.BorderColor = Color.IndianRed;
            }
            else
            {
                button.BorderColor = Color.Transparent;
            }
        }

        private void DayOfWeekButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as DayOfWeekButton;

            button.IsSelected = !button.IsSelected;

            if (button.IsSelected == true)
            {
                button.BorderColor = Color.IndianRed;
            }
            else
            {
                button.BorderColor = Color.Transparent;
            }
        }
    }
}