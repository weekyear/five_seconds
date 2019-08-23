using Five_Seconds.Models;
using Five_Seconds.Repository;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class MissionPopupViewModel : BaseViewModel
    {
        private readonly IPopupNavigation PopupNavigation;
        public MissionPopupViewModel(INavigation navigation, IMissionsRepository missionRepo, IPopupNavigation popupNavigation) : base(navigation, missionRepo)
        {
            Mission = new Mission();

            PopupNavigation = popupNavigation;

            ConstructCommand();
        }

        public MissionPopupViewModel(INavigation navigation, IMissionsRepository missionRepo, Mission mission, IPopupNavigation popupNavigation) : base(navigation, missionRepo)
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
            get { return Mission.Name; }
            set
            {
                if (Mission.Name == value) return;
                Mission.Name = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public TimeSpan Time
        {
            get
            {
                if (Mission.Alarm.Time== TimeSpan.Zero)
                {
                    Mission.Alarm.Time = DateTime.Now.TimeOfDay;
                }
                return Mission.Alarm.Time;
            }
            set
            {
                if (Mission.Alarm.Time == value) return;
                Mission.Alarm.Time = value;
                OnPropertyChanged(nameof(Time));
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

        // Methods

        private async Task ClosePopup()
        {
            await PopupNavigation.PopAsync(true);
        }
        private async Task Save()
        {
            MissionRepo.SaveMission(Mission);
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
