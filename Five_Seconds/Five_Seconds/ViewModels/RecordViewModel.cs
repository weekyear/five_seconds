using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Five_Seconds.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Five_Seconds.ViewModels
{
    public class RecordViewModel : BaseViewModel
    {
        public RecordViewModel(INavigation navigation) : base(navigation)
        {
            ConstructCommand();
            SetWeekRecords();
        }


        private void ConstructCommand()
        {
            CloseCommand = new Command(async () => await ClosePopup());
            PreviousMonthCommand = new Command(() => PreviousMonth());
            NextMonthCommand = new Command(() => NextMonth());
            ShowRecordDetailCommand = new Command<object>(async (w) => await ShowRecordDetail(w));
        }

        // Command
        public Command CloseCommand { get; set; }
        public Command PreviousMonthCommand { get; set; }
        public Command NextMonthCommand { get; set; }
        public Command<object> ShowRecordDetailCommand { get; set; }

        // Property

        private DateTime selectedMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
        public DateTime SelectedMonth
        {
            get { return selectedMonth; }
            set
            {
                if (selectedMonth.Month == value.Month) return;
                selectedMonth = value;
                OnPropertyChanged("SelectedMonth");
            }
        }

        public List<Record> Records { get; set; } = new List<Record>
        {
            new Record(new Alarm() { Id = 1, Name = "일어나자", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 02, 15, 30, 00))}, true),
            new Record(new Alarm() { Id = 2, Name = "밥묵자", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 04, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 3, Name = "공부하자", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 15, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 4, Name = "일하자1", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 16, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 5, Name = "일하자2", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 17, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 6, Name = "일하자3", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 25, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 7, Name = "일하자4", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 27, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 8, Name = "일하자5", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 28, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 9, Name = "일하자6", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 30, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 10, Name = "일하자7", TimeOffset = new DateTimeOffset(new DateTime(2019, 8, 25, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 11, Name = "일하자8", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 25, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 12, Name = "일하자9", TimeOffset = new DateTimeOffset(new DateTime(2019, 9, 29, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 13, Name = "일어나자", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 02, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 14, Name = "일어나자", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 02, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 15, Name = "일어나자", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 02, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 16, Name = "일하자5", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 28, 5, 30, 00))}, false),
            new Record(new Alarm() { Id = 17, Name = "일하자5", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 28, 5, 30, 00))}, false),
            new Record(new Alarm() { Id = 18, Name = "일하자5", TimeOffset = new DateTimeOffset(new DateTime(2019, 09, 28, 5, 30, 00))}, false),
        };

        public List<Record> MonthRecords
        {
            get
            {
                var selectedMonth = SelectedMonth;
                return Records.FindAll((r) => r.Date.Year == selectedMonth.Year && r.Date.Month == selectedMonth.Month);
            }
        }

        public ObservableCollection<WeekRecord> WeekRecords { get; set; } = new ObservableCollection<WeekRecord>();

        private async Task ClosePopup()
        {
            await Navigation.PopAsync(true);
        }

        private void PreviousMonth()
        {
            SelectedMonth = SelectedMonth.AddMonths(-1);

            SetWeekRecords();
        }

        private void NextMonth()
        {
            SelectedMonth = SelectedMonth.AddMonths(1);

            SetWeekRecords();
        }

        private async Task ShowRecordDetail(object weekRecord)
        {
            var wR = weekRecord as WeekRecord;
            await Navigation.PushAsync(new RecordDetailPage(Navigation, wR));
        }

        private void SetWeekRecords()
        {
            var weekRecords = new ObservableCollection<WeekRecord>();
            var startDateOfSelectedMonth = SelectedMonth;
            var startDateOfWeek = new DateTime(startDateOfSelectedMonth.Year, startDateOfSelectedMonth.Month, 1, 0, 0, 0);

            while (startDateOfWeek.Date.DayOfWeek != DayOfWeek.Monday) startDateOfWeek = startDateOfWeek.AddDays(-1);

            var startDateOfMonth = startDateOfWeek;

            while (startDateOfMonth.Ticks <= startDateOfSelectedMonth.Ticks)
            {
                var RecordsOfWeek = MonthRecords.FindAll((r) => r.TimeOffset.DateTime.Ticks > startDateOfWeek.Ticks && r.TimeOffset.DateTime.Ticks <= startDateOfWeek.AddDays(7).Ticks);

                var weekRecord = new WeekRecord()
                {
                    StartDateOfWeek = startDateOfWeek,
                    DayRecords = RecordsOfWeek
                };

                weekRecords.Add(weekRecord);

                startDateOfWeek = startDateOfWeek.AddDays(7);
                startDateOfMonth = new DateTime(startDateOfWeek.Year, startDateOfWeek.Month, 1, 0, 0, 0);
            }

            WeekRecords.Clear();

            foreach (var w in weekRecords)
            {
                WeekRecords.Add(w);
            }

            //WeekRecords = weekRecords;
        }

        public class WeekRecord
        {
            public DateTime StartDateOfWeek { get; set; }

            public List<Record> DayRecords { get; set; } = new List<Record>();

            public string NumOfDayRecords
            {
                get { return $"알람 {DayRecords.Count} 개"; }
            }

            public bool HasRecord
            {
                get
                {
                    if (DayRecords.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            public double SuccessRate
            {
                get
                {
                    if (DayRecords.Count == 0) return -1;
                    var successList = DayRecords.FindAll((r) => r.IsSuccess == true);
                    return (double)successList.Count / DayRecords.Count;
                }
            }
        }
    }
}
