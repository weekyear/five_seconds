using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class RecordViewModel : BaseViewModel
    {
        public RecordViewModel(INavigation navigation, Alarm alarm = null) : base(navigation)
        {
            Title = alarm?.Name;
            Alarm = alarm;
        }

        public Alarm Alarm
        {
            get;
            set;
        }
    }
}
