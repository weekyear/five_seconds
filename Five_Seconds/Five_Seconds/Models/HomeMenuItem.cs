using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Models
{
    public enum MenuItemType
    {
        Main,
        About,
        AppIntro
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
