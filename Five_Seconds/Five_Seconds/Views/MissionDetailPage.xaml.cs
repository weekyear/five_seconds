using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Five_Seconds.Models;
using Five_Seconds.ViewModels;

namespace Five_Seconds.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MissionDetailPage : ContentPage
    {
        MissionDetailViewModel viewModel;

        public MissionDetailPage(MissionDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public MissionDetailPage()
        {
            InitializeComponent();

            var item = new Mission
            {
                Description = "This is an item description."
            };

            viewModel = new MissionDetailViewModel(item);
            BindingContext = viewModel;
        }
    }
}