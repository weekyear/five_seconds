using System;

using Five_Seconds.Models;

namespace Five_Seconds.ViewModels
{
    public class MissionDetailViewModel : BaseViewModel
    {
        public Mission Item { get; set; }
        public MissionDetailViewModel(Mission item = null)
        {
            Title = item?.Description;
            Item = item;
        }
    }
}
