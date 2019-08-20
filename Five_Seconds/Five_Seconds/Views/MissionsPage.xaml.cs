﻿using System;
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
using Rg.Plugins.Popup.Services;
using Five_Seconds.Services;
using Five_Seconds.CustomControls;

namespace Five_Seconds.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [AdMaiora.RealXaml.Client.RootPage]
    public partial class MissionsPage : ContentPage
    {
        MissionsViewModel viewModel;

        public MissionsPage()
        {
            InitializeComponent();

            viewModel = new MissionsViewModel(Navigation, App.MissionsRepo, new MessageBoxService(), PopupNavigation.Instance);

            BindingContext = viewModel;

            MissionsListView.ItemSelected += MissionsListView_ItemSelected;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected void ShowMenuByItemClicked(object sender, ItemTappedEventArgs e)
        {
            viewModel.ShowMenuCommand.Execute(e.Item);
        }
        private void MissionsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (MissionsListView.SelectedItem != null || e.SelectedItem != null)
            {
                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}