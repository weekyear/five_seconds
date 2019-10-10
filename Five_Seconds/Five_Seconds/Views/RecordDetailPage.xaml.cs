using Five_Seconds.CustomControls;
using Five_Seconds.Models;
using Five_Seconds.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Five_Seconds.ViewModels.RecordDetailViewModel;
using static Five_Seconds.ViewModels.RecordViewModel;

namespace Five_Seconds.Views
{
    [AdMaiora.RealXaml.Client.RootPage]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordDetailPage : ContentPage
    {
        RecordDetailViewModel viewModel;
        public RecordDetailPage(INavigation navigation, WeekRecord weekRecord, List<Record> allRecords)
        {
            viewModel = new RecordDetailViewModel(navigation, weekRecord, allRecords);

            Resources = new ResourceDictionary
            {
                {
                    "TagValidatorFactory",
                    new Func<string, object>((arg) => (BindingContext as RecordDetailViewModel)?.ValidateAndReturn(arg))
                }
            };


            Application.Current.Resources.Add(Resources);

            InitializeComponent();

            BindingContext = viewModel;
        }

        private void DayRecords_SwipeLeft(object sender, EventArgs e)
        {
            viewModel.NextWeekCommand.Execute(null);
        }

        private void DayRecords_SwipeRight(object sender, EventArgs e)
        {
            viewModel.PreviousWeekCommand.Execute(null);
        }

        private void DayRecords_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (DayRecords.SelectedItem != null || e.SelectedItem != null)
            {
                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}