using Five_Seconds.Models;
using Five_Seconds.ViewModels;
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

        private int step_value = 1;
        DateTime _triggerTime;

        public MissionPopupPage()
        {
            viewModel = new MissionPopupViewModel();

            BindingContext = viewModel;

            InitializeComponent();
        }

        public MissionPopupPage(Mission mission)
        {
            viewModel = new MissionPopupViewModel(mission);

            BindingContext = viewModel;
            Mission = mission;

            InitializeComponent();
        }

        public Mission Mission
        {
            get; set;
        }

        //void SliderValueChanged(object sender, ValueChangedEventArgs e)
        //{
        //    var newStep = Math.Round(e.NewValue / step_value);

        //    TheSlider.Value = newStep * step_value;
        //    Timeout_Text.Text = TheSlider.Value.ToString() + "초";
        //}
        void MissionPickerIndexChanged(object sender, EventArgs e)
        {
            MissionType_Text.Text = (string)MissionPicker.ItemsSource[MissionPicker.SelectedIndex];
        }

        void OnTimePickerPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Time")
            {
                SetTriggerTime();
            }
        }

        void SetTriggerTime()
        {
            _triggerTime = DateTime.Today + _timePicker.Time;
            if (_triggerTime < DateTime.Now)
            {
                _triggerTime += TimeSpan.FromDays(1);
            }
        }
    }
}