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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordPage : ContentPage, ISearchPage
    {
        readonly RecordViewModel viewModel;

        public bool IsSearching
        {
            get { return viewModel.IsSearching; }
            set
            {
                if (viewModel.IsSearching == value) return;
                viewModel.IsSearching = value;
            }
        }

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

            SearchBarTextChanged += HandleSearchBarTextChanged;
            SearchBarTextSubmited += HandleSearchBarTextSubmited;

            InitializeComponent();

            BindingContext = viewModel;
        }

        protected override void OnDisappearing()
        {
            IsSearching = false;
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            if (IsSearching)
            {
                IsSearching = false;
                DependencyService.Get<IHelper>().CollapseSearchView();
                return true;
            }
            else
            {
                DependencyService.Get<IAdMobInterstitial>().Show("ca-app-pub-8413101784746060/6812351989");
            }

            return base.OnBackButtonPressed();
        }

        public event EventHandler<string> SearchBarTextChanged;
        public event EventHandler<string> SearchBarTextSubmited;

        public void OnSearchBarTextChanged(string text) => SearchBarTextChanged?.Invoke(this, text);
        public void OnSearchBarTextSubmited(string text) => SearchBarTextSubmited?.Invoke(this, text);
        
        void HandleSearchBarTextChanged(object sender, string searchBarText)
        {
            viewModel.TextChangedCommand.Execute(searchBarText);
        }

        void HandleSearchBarTextSubmited(object sender, string searchBarText)
        {
            viewModel.SearchCommand.Execute(searchBarText);
        }

        private void WeekRecords_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                viewModel.ShowRecordDetailCommand.Execute(e.Item);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                IsBusy = false;
            }
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

        private void SearchListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            viewModel.SearchCommand.Execute(e.Item);
            IsSearching = false;
            DependencyService.Get<IHelper>().CollapseSearchView();
        }

        private void SearchListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (SearchListView.SelectedItem != null || e.SelectedItem != null)
            {
                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}