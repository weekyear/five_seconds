using Five_Seconds.Models;
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
    public partial class RecordPage : ContentPage
    {
        RecordViewModel viewModel;

        public RecordPage(RecordViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public RecordPage()
        {
            InitializeComponent();

            var item = new Mission
            {
                Description = "This is an item description."
            };

            viewModel = new RecordViewModel(item);
            BindingContext = viewModel;
        }
    }
}