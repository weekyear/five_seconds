using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Five_Seconds.Models;
using Five_Seconds.Views;
using Five_Seconds.Services;
using Five_Seconds.Helpers;
using System.Collections.Generic;
using System.Linq;
using Five_Seconds.Resources;
using System.Globalization;
using Xamarin.Forms.Internals;

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
        public OrderableCollection<Alarm> Alarms
        {
            get
            {
                var alarms = AssignIndexToToDos(Service.Alarms);
                return ConvertListToObservableCollection(alarms);
            }
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
                switch (CultureInfo.CurrentCulture.Name)
                {
                    case "ko-KR":
                        return $"{numOfDeleteAlarms}개 삭제";
                    case "en-US":
                        return $"Delete {numOfDeleteAlarms} Alarms";
                    default:
                        return $"Delete {numOfDeleteAlarms} Alarms";
                }
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
                if (!value) ClearAllSelectedAlarms();
                OnPropertyChanged(nameof(IsSelectedMode));
            }
        }

        public void SetIsSelectedModeFalse()
        {
            if (IsSelectedMode == true)
            {
                IsSelectedMode = false;
            }
        }
        private void ClearAllSelectedAlarms()
        {
            var selectedAlarms = Alarms.Where((t) => t.IsSelected == true);
            selectedAlarms.ForEach((s) => s.IsSelected = false);
        }

        private async Task ShowAddAlarm()
        {
            await Navigation.PushAsync(new AlarmPage(Navigation)).ConfigureAwait(false);
        }

        private void ShowCountDown()
        {
            void action() => DependencyService.Get<ICountDown>().ShowCountDown();
            //void action() => DependencyService.Get<ICrashTest>().CrashTest();
            MessageBoxService.ShowConfirm(AppResources.FiveCount, AppResources.FiveCountDetail, null, action);
        }
        
        private void DeleteAlarms()
        {
            var DeleteAlarms = Alarms.Where(a => a.IsSelected == true);
            foreach (var alarm in DeleteAlarms)
            {
                Service.DeleteAlarm(alarm);
            }
            SetIsSelectedModeFalse();
        }

        private async Task ShowModifyAlarm(object _alarm)
        {
            var alarm = _alarm as Alarm; 
            await ModifyAlarm(alarm);
        }

        private async Task ShowMainMenu()
        {
            string[] actionSheetBtns = { AppResources.WhatFiveSecondsRule, AppResources.BriefDescription };

            string action = await MessageBoxService.ShowActionSheet(AppResources.Menu, AppResources.Cancel, null, actionSheetBtns);

            await ClickMenuAction(action);
        }

        private async Task ClickMenuAction(string action)
        {
            if (action == AppResources.WhatFiveSecondsRule)
            {
                await Navigation.PushAsync(new AboutPage());
            }
            else if (action == AppResources.BriefDescription)
            {
                var welcomePage = AppIntro.CreateAppIntro();
                await Navigation.PushModalAsync(welcomePage);
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

        private OrderableCollection<T> ConvertListToObservableCollection<T>(IEnumerable<T> list)
        {
            var collection = new OrderableCollection<T>();

            list.ForEach((item) => collection.Add(item));

            return collection;
        }

        private IEnumerable<Alarm> AssignIndexToToDos(IEnumerable<Alarm> alarms)
        {
            var _alarms = alarms.OrderBy((d) => d.Index);
            int i = 0;
            foreach (var alarm in _alarms)
            {
                alarm.Index = i++;
            }
            return _alarms;
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