using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class RecordViewModel : BaseViewModel
    {
        public RecordViewModel(INavigation navigation, ILocalData localData, Mission mission = null) : base(navigation, localData)
        {
            Title = mission?.Description;
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
