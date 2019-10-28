using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Five_Seconds.Models;
using Five_Seconds.Views;
using Five_Seconds.Services;
using System;
using Five_Seconds.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Five_Seconds.ViewModels
{
    public class AlarmsViewModel : BaseViewModel
    {
        private readonly IMessageBoxService MessageBoxService;

        public AlarmsViewModel(INavigation navigation, IMessageBoxService messageBoxService) : base(navigation)
        {
            MessageBoxService = messageBoxService;

            SubscribeMessage();

            ConstructCommand();
        }

        private void SubscribeMessage()
        {
            MessagingCenter.Subscribe<AlarmService>(this, "changeAlarms", (sender) =>
            {
                OnPropertyChanged(nameof(Alarms));
                OnPropertyChanged(nameof(NextAlarmString));
            });
        }

        private void ConstructCommand()
        {
            ShowAddAlarmCommand = new Command(async () => await ShowAddAlarm());
            ShowCountDownCommand = new Command(() => ShowCountDown());
            DeleteAlarmsCommand = new Command(() => DeleteAlarms());
            ShowRecordCommand = new Command(async() => await ShowRecord());
            ShowModifyAlarmCommand = new Command<object>(async (m) => await ShowModifyAlarm(m));
            ShowMainMenuCommand = new Command(async () => await ShowMainMenu());
        }

        // Property

        public Command ShowAddAlarmCommand { get; set; }
        public Command ShowCountDownCommand { get; set; }
        public Command DeleteAlarmsCommand { get; set; }
        public Command ShowRecordCommand { get; set; }
        public Command<object> ShowModifyAlarmCommand { get; set; }
        public Command ShowMainMenuCommand { get; set; }
        public ObservableCollection<Alarm> Alarms
        {
            get => ConvertListToObservableCollection(Service.Alarms);
        }

        public string NextAlarmString
        {
            get => CreateDateString.CreateNextDateTimeString(App.Service.GetNextAlarm());
        }

        public string DeleteAlarmString
        {
            get
            {
                var numOfDeleteAlarms = Alarms.Where(a => a.IsSelected == true).Count();
                return $"{numOfDeleteAlarms}개 삭제";
            }
        }

        private bool isSelectedMode;
        public bool IsSelectedMode
        {
            get { return isSelectedMode; }
            set
            {
                if (isSelectedMode == value) return;
                isSelectedMode = value;
                OnPropertyChanged(nameof(IsSelectedMode));
                OnPropertyChanged(nameof(IsNotSelectedMode));
            }
        }

        public bool IsNotSelectedMode
        {
            get { return !IsSelectedMode; }
        }


        private async Task ShowAddAlarm()
        {
            await Navigation.PushAsync(new AlarmPage(Navigation));
        }

        private void ShowCountDown()
        {
            void action() => DependencyService.Get<ICountDown>().ShowCountDown();
            MessageBoxService.ShowConfirm("5초 카운트", "5초 카운트를 시작하시겠습니까?", null, action);
        }
        
        private void DeleteAlarms()
        {
            var DeleteAlarms = Alarms.Where(a => a.IsSelected == true);
            foreach (var alarm in DeleteAlarms)
            {
                Service.DeleteAlarm(alarm);
            }
            ClearAllSelectedAlarm();
        }

        private async Task ShowModifyAlarm(object _alarm)
        {
            var alarm = _alarm as Alarm; 
            await ModifyAlarm(alarm);
        }

        private async Task ShowMainMenu()
        {
            string[] actionSheetBtns = { "5초의 법칙이란", "5초의 알람 간단 사용법" };

            string action = await MessageBoxService.ShowActionSheet("메뉴", "취소", null, actionSheetBtns);

            await ClickMenuAction(action);
        }

        private async Task ClickMenuAction(string action)
        {
            switch (action)
            {
                case "5초의 법칙이란":
                    await Navigation.PushAsync(new AboutPage());
                    break;
                case "5초의 알람 간단 사용법":
                    var welcomePage = AppIntro.CreateAppIntro();
                    await Navigation.PushModalAsync(welcomePage);
                    break;
            }
        }

        private async Task ShowRecord()
        {
            await Navigation.PushAsync(new RecordPage(Navigation, MessageBoxService));
        }

        private async Task ModifyAlarm(Alarm alarm)
        {
            Alarm.IsInitFinished = false;
            await Navigation.PushAsync(new AlarmPage(Navigation, alarm));
            Alarm.IsInitFinished = true;
        }

        private ObservableCollection<T> ConvertListToObservableCollection<T>(List<T> list)
        {
            var collection = new ObservableCollection<T>();

            list.ForEach((item) => collection.Add(item));

            return collection;
        }

        public void ClearAllSelectedAlarm()
        {
            IsSelectedMode = false;

            foreach (var alarm in Alarms)
            {
                alarm.IsSelected = false;
            }
        }

        public void ChangeIsSelectedOfAlarm(Alarm alarm)
        {
            alarm.IsSelected = !alarm.IsSelected;
            OnPropertyChanged(nameof(DeleteAlarmString));
        }
    }
}