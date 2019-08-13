using Five_Seconds.Models;
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
        public MissionPopupViewModel()
        {
            Mission = new Mission();

            ConstructCommand();
        }

        public MissionPopupViewModel(Mission mission)
        {
            Mission = mission;

            ConstructCommand();
        }

        private void ConstructCommand()
        {
            CloseCommand = new Command(async() => await ClosePopup());
            SaveCommand = new Command(async() => await Save());
        }
        public Command CloseCommand { get; set; }
        public Command SaveCommand { get; set; }

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

        private async Task ClosePopup()
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
        private async Task Save()
        {
            repository.SaveMission(Mission);
            await PopupNavigation.Instance.PopAsync(true);
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
