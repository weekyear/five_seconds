using Five_Seconds.Models;
using Five_Seconds.Repository;
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

        public MissionPopupPage(INavigation navigation, IPopupNavigation popupNavigation)
        {
            viewModel = new MissionPopupViewModel(navigation, popupNavigation);

            BindingContext = viewModel;

            InitializeComponent();
        }

        public MissionPopupPage(INavigation navigation, Mission mission, IPopupNavigation popupNavigation)
        {
            viewModel = new MissionPopupViewModel(navigation, mission, popupNavigation);

            BindingContext = viewModel;

            InitializeComponent();
        }
    }
}