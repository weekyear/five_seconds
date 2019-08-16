using Five_Seconds.Models;
using Five_Seconds.ViewModels;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Five_Seconds.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MissionPopupPage : PopupPage
    {
        MissionPopupViewModel viewModel;

        public MissionPopupPage(INavigation navigation, ILocalData localData, IPopupNavigation popupNavigation)
        {
            viewModel = new MissionPopupViewModel(navigation, localData, popupNavigation);

            BindingContext = viewModel;

            InitializeComponent();
        }

        public MissionPopupPage(INavigation navigation, ILocalData localData, Mission mission, IPopupNavigation popupNavigation)
        {
            viewModel = new MissionPopupViewModel(navigation, localData, mission, popupNavigation);

            BindingContext = viewModel;

            InitializeComponent();
        }
    }
}