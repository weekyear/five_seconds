using Five_Seconds.Models;
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
    public partial class AppListPage : ContentPage
    {
        public List<AppPackage> Apps { get; set; }
        public AppListPage(List<AppPackage> pkgList)
        {
            Apps = pkgList;

            BindingContext = this;

            InitializeComponent();
        }

        private async void AppListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var appPkg = e.Item as AppPackage;
            MessagingCenter.Send(this, "changeAppPkg", appPkg);
            
            await ClosePopup();
        }

        private async Task ClosePopup()
        {
            await Navigation.PopAsync(true);
        }

    }
}