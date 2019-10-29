using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Services;
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
        private readonly IMessageBoxService MessageBoxService;

        public RecordViewModel(INavigation navigation, IMessageBoxService messageBoxService) : base(navigation)
        {
            ConstructCommand();

            MessageBoxService = messageBoxService;

            CreateTagItems();

            UpdateWeekRecords(null);

            SubscribeMessage();

            SaveTestRecords();

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
            var ListRecordsForSearch = AllRecordsByName.ToList();
            foreach (var record in Records)
            {
                var IsAlreadyExist = ListRecordsForSearch.Exists(r => r == record.Name);
                if (!IsAlreadyExist)
                {
                    ListRecordsForSearch.Add(record.Name);
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
                UpdateWeekRecords(null);
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
                OnPropertyChanged("SelectedMonth");
            }
        }

        public List<Record> TestRecords { get; set; } = new List<Record>
        {
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 02, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 07, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 15, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 25, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 1, Name = "일어나서 이불개자", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 30, 15, 30, 00))}, false),
            new Record(new Alarm() { Id = 2, Name = "아침 운동 좋아", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 04, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 2, Name = "아침 운동 좋아", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 05, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 2, Name = "아침 운동 좋아", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 19, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 2, Name = "아침 운동 좋아", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 24, 5, 30, 00))}, true),
            new Record(new Alarm() { Id = 2, Name = "아침 운동 좋아", TimeOffset = new DateTimeOffset(new DateTime(2019, 10, 26, 5, 30, 00))}, true),
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
        };

        private void SaveTestRecords()
        {
            if (Records.Count == 0)
            {
                foreach (var _record in TestRecords)
                {
                    App.AlarmsRepo.SaveRecord(_record);
                    Records = App.AlarmsRepo.RecordFromDB;
                }
            }
        }

        public List<Record> Records { get; set; } = App.AlarmsRepo.RecordFromDB;

        public List<string> AllRecordsByName { get; set; } = new List<string>();
        public List<string> RecordsBySearch { get; set; } = new List<string>();

        public bool IsSearching { get; set; }
        public bool IsNotExistSearchResult 
        { 
            get { return IsSearching && RecordsBySearch.Count == 0; }
        }


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

        public bool HasNoWeekRecords
        {
            get
            {
                if (WeekRecords.Count == 0)
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
                var RecordsOfWeek = MonthRecordsByTag.FindAll((r) => r.DateTime.Ticks > startDateOfWeek.Ticks && r.DateTime.Ticks <= startDateOfWeek.AddDays(7).Ticks);

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
                MessageBoxService.ShowAlert("검색어 개수 초과", "검색어 수는 7개까지만 허용됩니다.");
                return;
            }

            var _tagItem = SearchTag.ValidateAndReturn(tag);

            UpdateWeekRecords(_tagItem);

            TagItems.Add(_tagItem);
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
            list.Sort();
            RecordsBySearch = list;
        }

        public class WeekRecord
        {
            public DateTime StartDateOfWeek { get; set; }

            public List<Record> DayRecords { get; set; } = new List<Record>();

            public string NumOfDayRecords
            {
                get
                {
                    if (DayRecords.Count == 0) { return "기록 없음"; }
                    return $"알람 {DayRecords.Count} 개";
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
