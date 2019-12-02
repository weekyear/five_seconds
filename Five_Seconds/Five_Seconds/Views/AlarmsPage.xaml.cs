﻿using Xamarin.Forms;
using Five_Seconds.ViewModels;
using Five_Seconds.Services;
using Five_Seconds.CustomControls;
using Five_Seconds.Models;
using System;
using System.Runtime.CompilerServices;
using Five_Seconds.Helpers;
using Xamarin.Forms.Xaml;

namespace Five_Seconds.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlarmsPage : ContentPage
    {
        readonly AlarmsViewModel viewModel;

        public AlarmsPage()
        {
            InitializeComponent();

            viewModel = new AlarmsViewModel(Navigation, new MessageBoxService());

            BindingContext = viewModel;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.SetIsSelectedModeFalse();
        }

        protected void ShowMenuByItemClicked(object sender, ItemTappedEventArgs e)
        {
            if (!viewModel.IsSelectedMode)
            {
                viewModel.ShowModifyAlarmCommand.Execute(e.Item);
            }
            else
            {
                var alarm = e.Item as Alarm;
                viewModel.ChangeIsSelectedOfAlarm(alarm);
            }
        }

        private void AlarmsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (AlarmsListView.SelectedItem != null || e.SelectedItem != null)
            {
                ((ListView)sender).SelectedItem = null;
            }
        }

        private void ListItemLabel_IsActiveChanged(object sender, EventArgs e)
        {
            var label = sender as ListItemLabel;
            if (label.IsActive)
            {
                label.TextColor = Color.Black;
            }
            else
            {
                label.TextColor = Color.LightGray;
            }
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

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            var viewCell = sender as ViewCell;
            var item = viewCell.BindingContext as Alarm;
            viewModel.ShowModifyAlarmCommand.Execute(item);
        }

        protected override bool OnBackButtonPressed()
        {
            if (viewModel.IsSelectedMode)
            {
                viewModel.SetIsSelectedModeFalse();

                return true;
            }
            return base.OnBackButtonPressed();
        }

        private void Button_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsSelectedMode") return;

            if (viewModel.IsSelectedMode)
            {
                DeleteBtn.FadeTo(1, 500, Easing.SpringIn);
            }
        }
    }
}