using Five_Seconds.CustomControls;
using Five_Seconds.Models;
using Five_Seconds.Services;
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
    public partial class RecordPage : ContentPage, ISearchPage
    {
        readonly RecordViewModel viewModel;

        public RecordPage(INavigation navigation, IMessageBoxService messageBoxService)
        {
            viewModel = new RecordViewModel(navigation, messageBoxService);

            Resources = new ResourceDictionary
            {
                {
                    "TagValidatorFactory",
                    new Func<string, object>((arg) => (BindingContext as RecordViewModel)?.ValidateAndReturn(arg))
                }
            };


            Application.Current.Resources.Add(Resources);

            SearchBarTextSubmited += HandleSearchBarTextSubmited;

            InitializeComponent();

            BindingContext = viewModel;
        }

        public event EventHandler<string> SearchBarTextSubmited;

        public void OnSearchBarTextSubmited(string text) => SearchBarTextSubmited?.Invoke(this, text);
        
        void HandleSearchBarTextSubmited(object sender, string searchBarText)
        {
            viewModel.SearchCommand.Execute(searchBarText);
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

        private void ListWeekRecords_SwipeLeft(object sender, EventArgs e)
        {
            viewModel.NextMonthCommand.Execute(null);
        }

        private void ListWeekRecords_SwipeRight(object sender, EventArgs e)
        {
            viewModel.PreviousMonthCommand.Execute(null);
        }

        private void TagEntry_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}