using Five_Seconds.Models;
using Five_Seconds.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class RecordViewModel : BaseViewModel
    {
        public RecordViewModel(INavigation navigation, IMissionsRepository missionRepo, Mission mission = null) : base(navigation, missionRepo)
        {
            Title = mission?.Name;
            Mission = mission;
        }

        public Mission Mission
        {
            get;
            set;
        }

        public ObservableCollection<Record> Records
        {
            get { return Mission.Records; }
        }
    }
}
