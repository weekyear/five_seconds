using Five_Seconds.CustomControls;
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
        public RecordDetailPage(INavigation navigation, WeekRecord weekRecord)
        {
            viewModel = new RecordDetailViewModel(navigation, weekRecord);

            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}