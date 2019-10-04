﻿using Five_Seconds.Models;
using Five_Seconds.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Five_Seconds.Helpers.SearchTag;

namespace Five_Seconds.ViewModels
{
    public class RecordViewModel : BaseViewModel
    {
        public RecordViewModel(INavigation navigation) : base(navigation)
        {
            ConstructCommand();
            SetWeekRecords();

            //ReloadTags();
        }

        private void ReloadTags()
        {
            var tags = new ObservableCollection<TagItem>(){
                new TagItem() { Name = "일하자" },
                new TagItem() { Name = "일하자1" },
                //new TagItem() { Name = "#Xamarin" },
                //new TagItem() { Name = "#DanielLuberda" },
                //new TagItem() { Name = "#Test" },
                //new TagItem() { Name = "#XamarinForms" },
                //new TagItem() { Name = "#TagEntryView" },
                //new TagItem() { Name = "#TapMe!" },
                //new TagItem() { Name = "#itsworking!" },
            };

            TagItems = tags;
        }


        private void ConstructCommand()
        {
            RemoveTagCommand = new Command<TagItem>((t) => RemoveTag(t));
            CloseCommand = new Command(async () => await ClosePopup());
            PreviousMonthCommand = new Command(() => PreviousMonth());
            NextMonthCommand = new Command(() => NextMonth());
            ShowRecordDetailCommand = new Command<object>(async (w) => await ShowRecordDetail(w));
        }

        // Command
        public Command<TagItem> RemoveTagCommand { get; set; }
        public Command CloseCommand { get; set; }
        public Command PreviousMonthCommand { get; set; }
        public Command NextMonthCommand { get; set; }
        public Command<object> ShowRecordDetailCommand { get; set; }
        public Func<string, object> TagValidatorFactory { get; set; }

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
            new Record(new Alarm() { Id = 1, Name = "일어나자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 02, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 2, Name = "밥묵자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 04, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 3, Name = "공부하자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 15, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 4, Name = "일하자1", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 16, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 5, Name = "일하자2", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 17, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 6, Name = "일하자3", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 25, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 7, Name = "일하자4", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 27, 5, 30, 00))}, false),
            new Record(new Alarm() { Id = 8, Name = "일하자5", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 28, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 9, Name = "일하자6", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 5, 30, 00))}, false),
            new Record(new Alarm() { Id = 10, Name = "일하자7", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 25, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 11, Name = "일하자8", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 25, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 12, Name = "일하자9", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 29, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 13, Name = "일어나자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 02, 15, 30, 00))}, true),
            new Record(new Alarm() { Id = 14, Name = "일어나자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 02, 16, 30, 00))}, false),
            new Record(new Alarm() { Id = 15, Name = "일어나자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 02, 18, 30, 00))}, true),
            new Record(new Alarm() { Id = 16, Name = "일하자10", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 28, 5, 30, 00))}, false),
            new Record(new Alarm() { Id = 17, Name = "일하자11", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 28, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 18, Name = "일하자12", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 28, 5, 30, 00))}, false),
        };

        //public List<Record> Records { get; set; } = App.AlarmsRepo.RecordFromDB;

        public List<Record> MonthRecords
        {
            get
            {
                var selectedMonth = SelectedMonth;
                return Records.FindAll((r) => r.Date.Year == selectedMonth.Year && r.Date.Month == selectedMonth.Month);
            }
        }

        private List<Record> monthRecordsByTag;
        public List<Record> MonthRecordsByTag
        {
            get
            {
                if (monthRecordsByTag == null) monthRecordsByTag = MonthRecords;
                return monthRecordsByTag;
            }
            set
            {
                if (monthRecordsByTag == value) return;
                monthRecordsByTag = value;
                OnPropertyChanged(nameof(MonthRecordsByTag));
                OnPropertyChanged(nameof(MonthSuccessRate));
            }
        }

        public double MonthSuccessRate
        {
            get
            {
                if (MonthRecordsByTag.Count == 0) return -1;
                var successList = MonthRecordsByTag.FindAll((r) => r.IsSuccess == true);
                return (double)successList.Count / MonthRecordsByTag.Count;
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

            SetMonthRecordsByTag(null);
            SetWeekRecords();
        }

        private void NextMonth()
        {
            SelectedMonth = SelectedMonth.AddMonths(1);

            SetMonthRecordsByTag(null);
            SetWeekRecords();
        }

        private async Task ShowRecordDetail(object weekRecord)
        {
            var wR = weekRecord as WeekRecord;
            await Navigation.PushAsync(new RecordDetailPage(Navigation, wR, Records));
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
                var RecordsOfWeek = MonthRecordsByTag.FindAll((r) => r.TimeOffset.DateTime.Ticks > startDateOfWeek.Ticks && r.TimeOffset.DateTime.Ticks <= startDateOfWeek.AddDays(7).Ticks);

                var weekRecord = new WeekRecord()
                {
                    StartDateOfWeek = startDateOfWeek,
                    DayRecords = RecordsOfWeek
                };

                weekRecords.Add(weekRecord);

                startDateOfWeek = startDateOfWeek.AddDays(7);
                startDateOfMonth = new DateTime(startDateOfWeek.Year, startDateOfWeek.Month, 1, 0, 0, 0);
            }

            // 여기에 태그 관련 리스트 뽑아내게 수정해야 함


            WeekRecords.Clear();

            WeekRecords = weekRecords;
        }

        // Tag

        public void RemoveTag(TagItem tagItem)
        {
            if (tagItem == null)
                return;

            TagItems.Remove(tagItem);

            SetMonthRecordsByTag(null);

            SetWeekRecords();
        }

        public TagItem ValidateAndReturn(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return null;

            if (TagItems.Any(v => v.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                return null;

            SetMonthRecordsByTag(tag);

            SetWeekRecords();

            return new TagItem()
            {
                Name = tag.ToLower()
            };
        }

        private void SetMonthRecordsByTag(string addedTag)
        {

            var tagItems = new ObservableCollection<TagItem>();

            foreach (var item in TagItems)
            {
                tagItems.Add(item);
            }

            if (tagItems.Count == 0 && addedTag == null)
            {
                MonthRecordsByTag = MonthRecords;
                return;
            }
            else if (addedTag != null)
            {
                tagItems.Add(new TagItem() { Name = addedTag.ToLower() });
            }

            var recordsByTag = new List<Record>();

            foreach (var tag in tagItems)
            {
                var records = MonthRecords.FindAll((r) => r.Name.Contains(tag.Name));
                foreach (var record in records)
                {
                    recordsByTag.Add(record);
                }
            }

            MonthRecordsByTag = recordsByTag.Distinct().ToList();
        }

        private ObservableCollection<TagItem> tagItems = new ObservableCollection<TagItem>();
        public ObservableCollection<TagItem> TagItems
        {
            get { return tagItems; }
            set
            {
                if (tagItems == value) return;
                tagItems = value;
                SetWeekRecords();
                OnPropertyChanged(nameof(TagItems));
            }
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
