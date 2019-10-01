﻿using Five_Seconds.CustomControls;
using Five_Seconds.Models;
using Five_Seconds.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Five_Seconds.ViewModels.RecordViewModel;

namespace Five_Seconds.Views
{
    [AdMaiora.RealXaml.Client.RootPage]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordPage : ContentPage
    {
        RecordViewModel viewModel;

        public RecordPage(INavigation navigation)
        {
            viewModel = new RecordViewModel(navigation);

            InitializeComponent();

            BindingContext = viewModel;
        }

        private void WeekRecords_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            viewModel.ShowRecordDetailCommand.Execute(e.Item);
        }

        private void WeekRecords_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (ListWeekRecords.SelectedItem != null || e.SelectedItem != null)
            {
                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}