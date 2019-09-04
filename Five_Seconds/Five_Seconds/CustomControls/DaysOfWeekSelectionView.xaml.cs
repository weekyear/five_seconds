using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Five_Seconds.CustomControls
{
    public partial class DaysOfWeekSelectionView : ContentView
    {
        public static readonly BindableProperty DaysProperty = BindableProperty.Create("Days", typeof(DaysOfWeek), typeof(DaysOfWeekSelectionView), new DaysOfWeek());

        public DaysOfWeek Days
        {
            get { return (DaysOfWeek)GetValue(DaysProperty); }
            set { SetValue(DaysProperty, value); }
        }

        public DaysOfWeekSelectionView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }

        private void DayOfWeekButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as DayOfWeekButton;

            button.IsSelected = !button.IsSelected;
        }

        void OnIsSelectedChanged(object sender, EventArgs e)
        {
            var button = sender as DayOfWeekButton;

            if (button.IsSelected == true)
            {
                button.BorderColor = Color.DarkBlue;
            }
            else
            {
                button.BorderColor = Color.Transparent;
            }
        }
    }
}