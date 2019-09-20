﻿using Five_Seconds.CustomControls;
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
        AlarmViewModel viewModel;

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
            datePicker.MinimumDate = viewModel.SetMinimumDate();
            datePicker.Date = viewModel.Date;
            datePicker.Focus();
        }
    }
}