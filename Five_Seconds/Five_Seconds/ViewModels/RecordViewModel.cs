using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.ViewModels
{
    public class RecordViewModel : BaseViewModel
    {
        public Mission Item { get; set; }
        public RecordViewModel(Mission item = null)
        {
            Title = item?.Description;
            Item = item;
        }
    }
}
