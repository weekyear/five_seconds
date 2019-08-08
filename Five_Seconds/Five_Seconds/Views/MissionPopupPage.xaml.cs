using Five_Seconds.Models;
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
        private int step_value = 1;
        DateTime _triggerTime;

        public MissionPopupPage()
        {
            InitializeComponent();
        }

        public MissionPopupPage(Mission mission)
        {
            Mission = mission;
        }

        public Mission Mission
        {
            get; set;
        }

        public DateTime Time
        {
            get; set;
        }

        private void SetMissionData()
        {
            Mission_Name.Text = Mission.Description;
            Timeout_Text.Text = Mission.Time.ToString() + "초";
        }

        private async void Close_Popup(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }

        void SliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / step_value);

            TheSlider.Value = newStep * step_value;
            Timeout_Text.Text = TheSlider.Value.ToString() + "초";
        }
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