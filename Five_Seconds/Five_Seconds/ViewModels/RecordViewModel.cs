using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class RecordViewModel : BaseViewModel
    {
        public Mission Item { get; set; }
        public RecordViewModel(INavigation navigation, ILocalData localData, Mission item = null) : base(navigation, localData)
        {
            Title = item?.Description;
            Item = item;
        }
    }
}
