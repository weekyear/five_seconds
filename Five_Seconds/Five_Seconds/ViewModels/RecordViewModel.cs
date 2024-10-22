﻿using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Resources;
using Five_Seconds.Services;
using Five_Seconds.Views;
using Microcharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Five_Seconds.Helpers.SearchTag;
using Entry = Microcharts.Entry;

namespace Five_Seconds.ViewModels
{
    public class RecordViewModel : BaseViewModel
    {
        private readonly IMessageBoxService MessageBoxService;

        private readonly int LabelFontSize = 12;
        private readonly int AnimationSec = 1;
        public RecordViewModel(INavigation navigation, IMessageBoxService messageBoxService) : base(navigation)
        {
            ConstructCommand();

            MessageBoxService = messageBoxService;

            CreateTagItems();

            UpdateWeekRecords(null);

            SubscribeMessage();

            //SaveTestRecords();

            InitRecordsForSearch();
        }


        private void ConstructCommand()
        {
            SearchCommand = new Command<string>((t) => Search(t));
            TextChangedCommand = new Command<string>((t) => TextChanged(t));
            RemoveTagCommand = new Command<TagItem>((t) => RemoveTag(t));
            CloseCommand = new Command(async () => await ClosePopup());
            PreviousMonthCommand = new Command(() => PreviousMonth());
            NextMonthCommand = new Command(() => NextMonth());
            ShowRecordDetailCommand = new Command<object>(async (w) => await ShowRecordDetail(w));
        }

        private void InitRecordsForSearch()
        {
            AllRecordsByName.Clear();

            foreach (var record in Records)
            {
                var IsAlreadyExist = AllRecordsByName.Exists(r => r == record.Name);
                if (!IsAlreadyExist)
                {
                    if (TagItems.ToList().Exists(t => t.Name != record.Name))
                    {
                        RecordsBySearch.Add(record.Name);
                    }
                    AllRecordsByName.Add(record.Name);
                }
            }
        }

        private void SubscribeMessage()
        {
            MessagingCenter.Subscribe<RecordDetailViewModel, TagItem>(this, "addTag", (sender, tagItem) =>
            {
                TagItems.Add(tagItem);
                UpdateWeekRecords(tagItem);
            });

            MessagingCenter.Subscribe<RecordDetailViewModel, TagItem>(this, "removeTag", (sender, tagItem) =>
            {
                TagItems.Remove(tagItem);
                UpdateWeekRecords(null);
            });
            
            MessagingCenter.Subscribe<RecordDetailViewModel, WeekRecord>(this, "changeWeekRecord", (sender, weekRecord) =>
            {
                ChangeWeekRecord(weekRecord);
                UpdateWeekRecords(null);
                OnPropertyChanged(nameof(MonthSuccessRate));
            });

            MessagingCenter.Subscribe<RecordDetailViewModel, Record>(this, "deleteRecord", (sender, record) =>
            {
                Records.Remove(record);
                InitRecordsForSearch();
                UpdateWeekRecords(null);
                RefreshRecordsBySearch();
                OnPropertyChanged(nameof(MonthSuccessRate));
            });
        }

        // Command
        public Command<string> SearchCommand { get; set; }
        public Command<string> TextChangedCommand { get; set; }
        public Command<TagItem> RemoveTagCommand { get; set; }
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
                OnPropertyChanged(nameof(SelectedMonth) );
            }
        }

        public List<Record> TestRecords { get; set; } = new List<Record>
        {
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 9, 29, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 02, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 07, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 15, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 25, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, true),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, true),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, true),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, true),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, true),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, true),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, true),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, true),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, true),
            new Record(new Alarm() { Id = 2, Name = "아침 운동 좋아", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 04, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 2, Name = "아침 운동 좋아", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 05, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 2, Name = "아침 운동 좋아", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 19, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 2, Name = "아침 운동 좋아", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 24, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 2, Name = "아침 운동 좋아", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 26, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 3, Name = "공부하자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 05, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 3, Name = "공부하자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 05, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 3, Name = "공부하자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 05, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 3, Name = "공부하자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 05, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 3, Name = "공부하자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 12, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 3, Name = "공부하자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 27, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 3, Name = "공부하자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 18, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 3, Name = "공부하자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 16, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 3, Name = "공부하자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 22, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 4, Name = "야식은 나의 적", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 06, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 4, Name = "야식은 나의 적", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 16, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 4, Name = "야식은 나의 적", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 11, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 4, Name = "야식은 나의 적", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 13, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 4, Name = "야식은 나의 적", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 23, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 5, Name = "일단 침대에 눕자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 7, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 5, Name = "일단 침대에 눕자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 17, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 5, Name = "일단 침대에 눕자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 14, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 5, Name = "일단 침대에 눕자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 26, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 5, Name = "일단 침대에 눕자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 31, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 6, Name = "점심 먹고 짜투리 독서", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 5, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 6, Name = "점심 먹고 짜투리 독서", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 9, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 6, Name = "점심 먹고 짜투리 독서", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 12, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 6, Name = "점심 먹고 짜투리 독서", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 21, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 6, Name = "점심 먹고 짜투리 독서", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 25, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 7, Name = "점심 먹고", TimeOffset = new DateTimeOffset(new DateTime(2019, 11, 2, 5, 30, 00))}, true),
        };

        private void SaveTestRecords()
        {
            if (Records.Count == 0)
            {
                foreach (var _record in TestRecords)
                {
                    App.AlarmsRepo.SaveRecord(_record);
                    Records = App.AlarmsRepo.RecordFromDB.ToList();
                }
            }
        }

        public List<Record> Records { get; set; } = App.AlarmsRepo.RecordFromDB.ToList();

        public List<string> AllRecordsByName { get; set; } = new List<string>();
        public ObservableCollection<string> RecordsBySearch { get; set; } = new ObservableCollection<string>();

        public bool IsSearching { get; set; }
        public bool IsNotExistSearchResult 
        { 
            get { return IsSearching && RecordsBySearch.Count == 0; }
        }

        private bool isGraph = false;
        public bool IsGraph 
        {
            get { return isGraph; }
            set
            {
                if (isGraph == value) return;
                SetProperty(ref isGraph, value, nameof(IsGraph));
                OnPropertyChanged(nameof(IsNotGraph));
            }
        }
        public bool IsNotGraph
        {
            get { return !IsGraph; }
        }
        public Chart SuccessChart { get; set; }


        public List<Record> MonthRecords
        {
            get
            {
                var selectedMonth = SelectedMonth;
                return Records.FindAll(r => r.Date.Year == selectedMonth.Year && IsCheckedSelectedMonthForMonthRecords(r));
            }
        }

        private bool IsCheckedSelectedMonthForMonthRecords(Record record)
        {
            var prevMonth = SelectedMonth.AddMonths(-1);
            while (prevMonth.Date.DayOfWeek != DayOfWeek.Monday) prevMonth = prevMonth.AddDays(-1);

            var nextMonth = SelectedMonth.AddMonths(1);
            while (nextMonth.Date.DayOfWeek != DayOfWeek.Sunday) nextMonth = nextMonth.AddDays(1);

            if (prevMonth <= record.Date && record.Date <= nextMonth)
            {
                return true;
            }
            else
            {
                return false;
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

        public bool HasNoWeekRecords
        {
            get
            {
                if (!IsSearching && WeekRecords.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void ChangeWeekRecord(WeekRecord weekRecord)
        {
            foreach (var _weekRecord in WeekRecords.ToList())
            {
                if (_weekRecord.StartDateOfWeek.Date == weekRecord.StartDateOfWeek.Date)
                {
                    _weekRecord.DayRecords = weekRecord.DayRecords;
                }
            }
        }

        private async Task ClosePopup()
        {
            await Navigation.PopAsync(true);
        }

        private void PreviousMonth()
        {
            SelectedMonth = SelectedMonth.AddMonths(-1);

            UpdateWeekRecords(null);
        }

        private void NextMonth()
        {
            SelectedMonth = SelectedMonth.AddMonths(1);

            UpdateWeekRecords(null);
        }

        private async Task ShowRecordDetail(object weekRecord)
        {
            var wR = weekRecord as WeekRecord;
            await Navigation.PushAsync(new RecordDetailPage(Navigation, wR, Records, MessageBoxService));
        }

        private void UpdateWeekRecords(TagItem tagItem)
        {
            UpdateMonthRecordsByTag(tagItem);

            var weekRecords = new ObservableCollection<WeekRecord>();
            var startDateOfSelectedMonth = SelectedMonth;
            var startDateOfWeek = new DateTime(startDateOfSelectedMonth.Year, startDateOfSelectedMonth.Month, 1, 0, 0, 0);

            while (startDateOfWeek.Date.DayOfWeek != DayOfWeek.Monday) startDateOfWeek = startDateOfWeek.AddDays(-1);

            var startDateOfMonth = startDateOfWeek;

            while (startDateOfMonth.Ticks <= startDateOfSelectedMonth.Ticks)
            {
                var RecordsOfWeek = MonthRecordsByTag.FindAll(r => startDateOfWeek.Ticks < r.DateTime.Ticks && r.DateTime.Ticks <= startDateOfWeek.AddDays(7).Ticks);

                var weekRecord = new WeekRecord()
                {
                    StartDateOfWeek = startDateOfWeek,
                    DayRecords = RecordsOfWeek
                };

                if (weekRecord.HasRecord)
                {
                    weekRecords.Add(weekRecord);
                }

                startDateOfWeek = startDateOfWeek.AddDays(7);
                startDateOfMonth = new DateTime(startDateOfWeek.Year, startDateOfWeek.Month, 1, 0, 0, 0);
            }

            // 여기에 태그 관련 리스트 뽑아내게 수정해야 함

            WeekRecords.Clear();

            WeekRecords = weekRecords;

            RefreshRecordChart();

            OnPropertyChanged(nameof(SuccessChart));
            OnPropertyChanged(nameof(HasNoWeekRecords));
        }

        private void UpdateMonthRecordsByTag(TagItem tagItem)
        {
            var recordsByTag = new List<Record>();
            var tagItems = new List<TagItem>(TagItems.ToList());

            if (tagItem != null)
            {
                tagItems.Add(tagItem);
            }

            if (tagItems.Count == 0 && recordsByTag.Count == 0)
            {
                MonthRecordsByTag = MonthRecords;
            }
            else
            {
                foreach (var tag in tagItems)
                {
                    var records = MonthRecords.FindAll((r) => string.Compare(r.Name, tag.Name) == 0);
                    foreach (var record in records)
                    {
                        recordsByTag.Add(record);
                    }
                }

                MonthRecordsByTag = recordsByTag.Distinct().ToList();
            }
        }

        // Search & Tag

        public void RemoveTag(TagItem tagItem)
        {
            SearchTag.RemoveTag(tagItem);

            if (tagItem == null)
                return;

            TagItems.Remove(tagItem);

            UpdateWeekRecords(null);

            RecordsBySearch.Add(tagItem.Name);

            var list = new List<string>(RecordsBySearch);
            list.Sort();

            SortRecordsBySearch(list);
        }

        public TagItem ValidateAndReturn(string tag)
        {
            var _tagItem = SearchTag.ValidateAndReturn(tag);

            UpdateWeekRecords(_tagItem);

            return _tagItem;
        }

        public ObservableCollection<TagItem> TagItems { get; set; } = new ObservableCollection<TagItem>();

        private void CreateTagItems()
        {
            foreach (var item in StaticTagItems)
            {
                TagItems.Add(item);
            }
        }

        private void Search(string tag)
        {
            if (TagItems.Count > 7)
            {
                MessageBoxService.ShowAlert(AppResources.QueryCountExceeded, AppResources.QueryCountExceededDetail);
                return;
            }

            var _tagItem = SearchTag.ValidateAndReturn(tag);

            UpdateWeekRecords(_tagItem);

            TagItems.Add(_tagItem);
            
            RecordsBySearch.Remove(_tagItem.Name);
        }
        
        private void TextChanged(string searchText)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(searchText))
            {
                list = AllRecordsByName;
            }
            else
            {
                list = AllRecordsByName.FindAll(s => s.Contains(searchText));
                if (list.Count == 0)
                {
                    OnPropertyChanged(nameof(IsNotExistSearchResult));
                }
            }

            foreach (var tagItem in TagItems)
            {
                list.Remove(tagItem.Name);
            }

            list.Sort();

            SortRecordsBySearch(list);
        }

        private void SortRecordsBySearch(List<string> list)
        {
            RecordsBySearch.Clear();

            RecordsBySearch = new ObservableCollection<string>(list);
        }

        private void RefreshRecordsBySearch()
        {
            var list = new List<string>();

            list = AllRecordsByName;

            foreach (var tagItem in TagItems)
            {
                list.Remove(tagItem.Name);
            }

            SortRecordsBySearch(list);
        }

        private void RefreshRecordChart()
        {
            var entries = new List<Entry>();

            for (int i = 0; i < WeekRecords.Count; i++)
            {
                var weekRecord = WeekRecords[i];

                var startDate = weekRecord.StartDateOfWeek;
                var endDate = startDate.AddDays(6);

                var entry = new Entry((float)weekRecord.SuccessRate)
                {
                    Label = $"{startDate.Month}.{startDate.Day} ~ {endDate.Month}.{endDate.Day}",
                    Color = SkiaSharp.SKColor.Parse("#1565c0"),
                    ValueLabel = $"{string.Format("{0:0.##}", Math.Round(weekRecord.SuccessRate * 100))}%"
                };
                entries.Add(entry);
            }

            SuccessChart = new BarChart()
            {
                Entries = entries,
                LabelTextSize = DependencyService.Get<INativeFont>().GetNativeSize(LabelFontSize),
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                AnimationDuration = TimeSpan.FromSeconds(AnimationSec)
            };
        }

        public class WeekRecord
        {
            public DateTime StartDateOfWeek { get; set; }

            public List<Record> DayRecords { get; set; } = new List<Record>();

            public string NumOfDayRecords
            {
                get
                {
                    if (DayRecords.Count == 0) { return AppResources.NoRecord; }
                    switch (CultureInfo.CurrentCulture.Name)
                    {
                        case "ko-KR":
                            return $"알람 {DayRecords.Count} 개";
                        case "en-US":
                            return $"{DayRecords.Count} Alarms";
                        default:
                            return $"{DayRecords.Count} Alarms";
                    }
                }
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
