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
        public RecordViewModel(INavigation navigation, Mission mission = null) : base(navigation)
        {
            Title = mission?.Name;
            Mission = mission;
        }

        public Mission Mission
        {
            get;
            set;
        }

        public List<Record> Records
        {
            get { return Mission.Records; }
        }
    }
}
