using Five_Seconds.Models;
using Five_Seconds.Services;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class MissionPopupViewModel : BaseViewModel
    {
        private readonly IPopupNavigation PopupNavigation;
        public MissionPopupViewModel(INavigation navigation, ILocalData localData, IPopupNavigation popupNavigation) : base(navigation, localData)
        {
            Mission = new Mission();

            PopupNavigation = popupNavigation;

            ConstructCommand();
        }

        public MissionPopupViewModel(INavigation navigation, ILocalData localData, Mission mission, IPopupNavigation popupNavigation) : base(navigation, localData)
        {
            Mission = new Mission(mission);

            PopupNavigation = popupNavigation;

            ConstructCommand();
        }

        private void ConstructCommand()
        {
            CloseCommand = new Command(async() => await ClosePopup());
            SaveCommand = new Command(async() => await Save());
        }

        // Command
        public Command CloseCommand { get; set; }
        public Command SaveCommand { get; set; }

        // Property
        public Mission Mission
        {
            get; set;
        }

        public string Description
        {
            get { return Mission.Description; }
            set
            {
                if (Mission.Description == value) return;
                Mission.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public TimeSpan TimeOfDay
        {
            get
            {
                if (Mission.TimeOfDay == TimeSpan.Zero)
                {
                    Mission.TimeOfDay = DateTime.Now.TimeOfDay;
                }
                return Mission.TimeOfDay;
            }
            set
            {
                if (Mission.TimeOfDay == value) return;
                Mission.TimeOfDay = value;
                OnPropertyChanged(nameof(TimeOfDay));
            }
        }
        public int TimeLimit
        {
            get { return Mission.TimeLimit; }
            set
            {
                if (Mission.TimeLimit == value) return;
                Mission.TimeLimit = value;
                OnPropertyChanged(nameof(TimeLimit));
            }
        }

        public DateTime Time
        {
            get; set;
        }

        // Methods

        private async Task ClosePopup()
        {
            await PopupNavigation.PopAsync(true);
        }
        private async Task Save()
        {
            base.LocalData.SaveMission(Mission);
            await ClosePopup();
        }

        public static bool AreEqual<T>(T left, T right)
        {
            if (left == null)
                return right == null;
            if (left is IComparable<T>)
            {
                IComparable<T> lval = left as IComparable<T>;
                if (right is IComparable<T>)
                    return lval.CompareTo(right) == 0;
                else
                    throw new ArgumentException("Type does not implement IComparable<T>", nameof(right));
            }
            else // 실패
            {
                throw new ArgumentException("Type does not implement IComparable<T>", nameof(left));
            }
        }
    }
}
