using Five_Seconds.Models;
using Five_Seconds.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Five_Seconds.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        AboutViewModel viewModel;
        public AboutPage()
        {
            InitializeComponent();

            viewModel = new AboutViewModel(Navigation);

            BindingContext = viewModel;
        }

        protected void ShowAboutByItemClicked(object sender, ItemTappedEventArgs e)
        {
            viewModel.ShowAboutCommand.Execute(e.ItemIndex);
        }

        private void AboutListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (AboutListView.SelectedItem != null || e.SelectedItem != null)
            {
                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}