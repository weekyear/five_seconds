using Five_Seconds.Models;
using Five_Seconds.Repository;
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

        public SettingTonePage(INavigation navigation, IMissionsRepository localData, Mission mission)
        {
            viewModel = new SettingToneViewModel(navigation, localData, mission);

            BindingContext = viewModel;

            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.StopToneCommand.Execute(null);
        }

        void AddConfirmToolbarItem(object sender, SelectedItemChangedEventArgs e)
        {
            //viewModel.SelectedTone = e.SelectedItem as AlarmTone;

            if (ToolbarItems.Count == 0)
            {
                ToolbarItems.Add(new ToolbarItem
                {
                    Text = "Save",
                    Icon = "save",
                    Command = viewModel.ToneSaveCommand
                });
            }
        }

        private void ToneListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedTone = e.SelectedItem as AlarmTone;
            //viewModel.SelectedTone = selectedTone;
            viewModel.ToneSaveCommand.Execute(selectedTone);
        }
    }
}