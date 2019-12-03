using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Resources;
using Five_Seconds.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Five_Seconds.Helpers.SearchTag;
using static Five_Seconds.ViewModels.RecordViewModel;

namespace Five_Seconds.ViewModels
{
    public class RecordDetailViewModel : BaseViewModel
    {
        private readonly IMessageBoxService MessageBoxService;

        public RecordDetailViewModel(INavigation navigation, WeekRecord weekRecord, IEnumerable<Record> allRecords, IMessageBoxService messageBoxService) : base(navigation)
        {
            ConstructCommand();

            MessageBoxService = messageBoxService;

            Records = allRecords.ToList();

            WeekRecord = weekRecord;

            CreateTagItems();

            SetDayRecords();

            InitRecordsForSearch();
        }


        private void ConstructCommand()
        {
            SearchCommand = new Command<string>((t) => Search(t));
            TextChangedCommand = new Command<string>((t) => TextChanged(t));
            RemoveTagCommand = new Command<TagItem>((t) => RemoveTag(t));
            CloseCommand = new Command(async () => await ClosePopup());
            PreviousWeekCommand = new Command(() => PreviousWeek());
            NextWeekCommand = new Command(() => NextWeek());
            ShowRecordMenuCommand = new Command<object>(async (a) => await ShowRecordMenu(a));
        }
        private void InitRecordsForSearch()
        {
            AllRecordsByName.Clear();

            var ListRecordsForSearch = AllRecordsByName.ToList();
            foreach (var record in Records)
            {
                var IsAlreadyExist = ListRecordsForSearch.Exists(r => r == record.Name);
                if (!IsAlreadyExist)
                {
                    ListRecordsForSearch.Add(record.Name);
                    AllRecordsByName.Add(record.Name);
                    if (TagItems.ToList().Exists(t => t.Name != record.Name))
                    {
                        RecordsBySearch.Add(record.Name);
                    }
                }
            }
        }

        // Command

        public Command<string> SearchCommand { get; set; }
        public Command<string> TextChangedCommand { get; set; }
        public Command<TagItem> RemoveTagCommand { get; set; }
        public Command CloseCommand { get; set; }
        public Command PreviousWeekCommand { get; set; }
        public Command NextWeekCommand { get; set; }
        public Command<object> ShowRecordMenuCommand { get; set; }

        // Property

        public List<Record> Records { get; set; }

        // Property for Search //

        public List<string> AllRecordsByName { get; set; } = new List<string>();
        public ObservableCollection<string> RecordsBySearch { get; set; } = new ObservableCollection<string>();
        public bool IsSearching { get; set; }
        public bool IsNotExistSearchResult
        {
            get { return IsSearching && RecordsBySearch.Count == 0; }
        }

        // Property for Search //


        private List<Record> recordsByTag;
        public List<Record> RecordsByTag
        {
            get
            {
                if (recordsByTag == null) recordsByTag = Records;
                return recordsByTag;
            }
            set
            {
                if (recordsByTag == value) return;
                recordsByTag = value;
                OnPropertyChanged(nameof(RecordsByTag));
                OnPropertyChanged(nameof(WeekSuccessRate)); // 이거를 SuccessRate가 확실하게 바뀌는 다른 곳으로 옮겨야함
            }
        }

        public WeekRecord WeekRecord
        {
            get; set;
        }

        public DateTime SelectedWeek
        {
            get { return WeekRecord.StartDateOfWeek; }
            set
            {
                if (WeekRecord.StartDateOfWeek.Day == value.Day) return;
                WeekRecord.StartDateOfWeek = value;
                OnPropertyChanged(nameof(SelectedWeek));
                OnPropertyChanged(nameof(WeekRecord));
            }
        }

        public ObservableCollection<DayRecord> DayRecords { get; set; } = new ObservableCollection<DayRecord>();
        public double WeekSuccessRate
        {
            get
            {
                var dayRecords = WeekRecord.DayRecords;

                if (dayRecords.Count == 0) return -1;
                var successList = dayRecords.FindAll((r) => r.IsSuccess == true);
                return (double)successList.Count / dayRecords.Count;
            }
        }

        public bool HasNoRecord
        {
            get
            {
                if (!IsSearching && DayRecords.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool HasRecord
        {
            get
            {
                return !HasNoRecord;
            }
        }

        // Method

        private async Task ClosePopup()
        {
            await Navigation.PopAsync(true);
        }

        private void PreviousWeek()
        {
            SelectedWeek = SelectedWeek.AddDays(-7);

            UpdateWeekRecord(null);
        }

        private void NextWeek()
        {
            SelectedWeek = SelectedWeek.AddDays(7);

            UpdateWeekRecord(null);
        }

        private void UpdateWeekRecord(TagItem tagItem)
        {
            UpdateRecordsByTag(tagItem);

            var weekRecords = new ObservableCollection<WeekRecord>();
            var startDateOfSelectedWeek = SelectedWeek;
            var startDateOfWeek = new DateTime(startDateOfSelectedWeek.Year, startDateOfSelectedWeek.Month, startDateOfSelectedWeek.Day, 0, 0, 0);

            while (startDateOfWeek.Date.DayOfWeek != DayOfWeek.Monday) startDateOfWeek = startDateOfWeek.AddDays(-1);

            var startDateOfMonth = startDateOfWeek;

            var RecordsOfWeek = RecordsByTag.FindAll((r) => r.DateTime.Ticks > startDateOfWeek.Ticks && r.DateTime.Ticks <= startDateOfWeek.AddDays(7).Ticks);

            var weekRecord = new WeekRecord()
            {
                StartDateOfWeek = startDateOfWeek,
                DayRecords = RecordsOfWeek
            };

            WeekRecord = weekRecord;

            SetDayRecords();

            OnPropertyChanged(nameof(HasRecord));
            OnPropertyChanged(nameof(HasNoRecord));
        }

        private void UpdateRecordsByTag(TagItem tagItem)
        {
            var recordsByTag = new List<Record>();
            var tagItems = new List<TagItem>(TagItems);

            if (tagItem != null)
            {
                tagItems.Add(tagItem);
            }

            if (tagItems.Count == 0 && recordsByTag.Count == 0)
            {
                RecordsByTag = Records;
            }
            else
            {
                foreach (var tag in tagItems)
                {
                    var records = Records.FindAll((r) => string.Compare(r.Name, tag.Name) == 0);
                    foreach (var record in records)
                    {
                        recordsByTag.Add(record);
                    }
                }

                RecordsByTag = recordsByTag.Distinct().ToList();
            }
        }

        private void SetDayRecords()
        {
            var startDateOfSelectedWeek = SelectedWeek;

            var monList = new DayRecord() { DayOfWeek = AppResources.Monday, Date = startDateOfSelectedWeek };
            var tueList = new DayRecord() { DayOfWeek = AppResources.Tuesday, Date = startDateOfSelectedWeek.AddDays(1) };
            var wedList = new DayRecord() { DayOfWeek = AppResources.Wednesday, Date = startDateOfSelectedWeek.AddDays(2) };
            var thuList = new DayRecord() { DayOfWeek = AppResources.Thursday, Date = startDateOfSelectedWeek.AddDays(3) };
            var friList = new DayRecord() { DayOfWeek = AppResources.Friday, Date = startDateOfSelectedWeek.AddDays(4) };
            var satList = new DayRecord() { DayOfWeek = AppResources.Saturday, Date = startDateOfSelectedWeek.AddDays(5) };
            var sunList = new DayRecord() { DayOfWeek = AppResources.Sunday, Date = startDateOfSelectedWeek.AddDays(6) };

            foreach (var record in WeekRecord.DayRecords)
            {
                switch (record.Date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        monList.Add(record);
                        break;
                    case DayOfWeek.Tuesday:
                        tueList.Add(record);
                        break;
                    case DayOfWeek.Wednesday:
                        wedList.Add(record);
                        break;
                    case DayOfWeek.Thursday:
                        thuList.Add(record);
                        break;
                    case DayOfWeek.Friday:
                        friList.Add(record);
                        break;
                    case DayOfWeek.Saturday:
                        satList.Add(record);
                        break;
                    case DayOfWeek.Sunday:
                        sunList.Add(record);
                        break;
                }
            }

            var dayRecordList = new ObservableCollection<DayRecord>
            {
                monList, tueList, wedList, thuList, friList, satList, sunList
            };

            DayRecords.Clear();

            foreach (var dayList in dayRecordList.ToList())
            {
                if (dayList.Count != 0)
                {
                    DayRecords.Add(dayList);
                }
            }
        }

        // Tag

        public ObservableCollection<TagItem> TagItems { get; set; } = new ObservableCollection<TagItem>();
        private void CreateTagItems()
        {
            foreach (var item in StaticTagItems)
            {
                TagItems.Add(item);
            }
        }

        public void RemoveTag(TagItem tagItem)
        {
            SearchTag.RemoveTag(tagItem);

            if (tagItem == null)
                return;

            TagItems.Remove(tagItem);

            UpdateWeekRecord(null);

            SendMessage("removeTag", tagItem);

            RecordsBySearch.Add(tagItem.Name);

            var list = new List<string>(RecordsBySearch);
            list.Sort();

            SortRecordsBySearch(list);
        }

        public TagItem ValidateAndReturn(string tag)
        {
            var _tagItem = SearchTag.ValidateAndReturn(tag);

            if (_tagItem != null)
            {
                UpdateWeekRecord(_tagItem);
                SendMessage("addTag", _tagItem);
            }

            return _tagItem;
        }

        private void SendMessage<T>(string type, T item)
        {
            MessagingCenter.Send(this, type, item);
        }

        private void Search(string tag)
        {
            if (TagItems.Count > 7)
            {
                MessageBoxService.ShowAlert(AppResources.QueryCountExceeded, AppResources.QueryCountExceededDetail);
                return;
            }

            var _tagItem = SearchTag.ValidateAndReturn(tag);

            if (_tagItem != null)
            {
                UpdateWeekRecord(_tagItem);
                SendMessage("addTag", _tagItem);
            }

            TagItems.Add(_tagItem);

            RecordsBySearch.Remove(_tagItem.Name);
        }
        
        private void TextChanged(string searchText)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(searchText))
            {
                list = AllRecordsByName.ToList();
            }
            else
            {
                list = AllRecordsByName.Where(s => s.Contains(searchText)).ToList();
                if (list.Count == 0)
                {
                    OnPropertyChanged(nameof(IsNotExistSearchResult));
                }
            }

            foreach (var tagItem in TagItems)
            {
                list.Remove(tagItem.Name);
            }

            SortRecordsBySearch(list);
        }

        private void SortRecordsBySearch(List<string> list)
        {
            list.Sort();

            RecordsBySearch.Clear();

            RecordsBySearch = new ObservableCollection<string>(list);
        }

        private void RefreshRecordsBySearch()
        {
            List<string> list = AllRecordsByName.ToList();

            foreach (var tagItem in TagItems)
            {
                list.Remove(tagItem.Name);
            }

            SortRecordsBySearch(list);
        }

        public async Task ShowRecordMenu(object _record)
        {
            var record = _record as Record;
            string[] actionSheetBtns = { AppResources.Success, AppResources.Failure, AppResources.Delete };

            string action = await MessageBoxService.ShowActionSheet(AppResources.RecordOption, AppResources.Cancel, null, actionSheetBtns);

            ClickRecordMenuAction(action, record);

            SetDayRecords();
        }

        private void ClickRecordMenuAction(string action, Record record)
        {
            //var dayRecord = DayRecords.Where(d => d.Date.DayOfWeek == record.Date.DayOfWeek).FirstOrDefault();

            //var _record = dayRecord.Find(r => r.Id == record.Id);

            foreach (var dayRecord in DayRecords.ToList())
            {
                foreach (var _record in dayRecord)
                {
                    if (_record.Id == record.Id)
                    {
                        if (action == AppResources.Delete)
                        {
                            ConfirmDeletingRecord(record);
                            RefreshRecordsBySearch();
                        }
                        else
                        {
                            if (action == AppResources.Success && !_record.IsSuccess)
                            {
                                ConfirmChangingSuccessRecord(_record);
                            }
                            else if (action == AppResources.Failure && _record.IsSuccess)
                            {
                                _record.IsSuccess = false;
                                App.AlarmsRepo.SaveRecord(_record);
                                SendMessage("changeWeekRecord", WeekRecord);
                                OnPropertyChanged(nameof(WeekSuccessRate));
                                SetDayRecords();
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }
        private void ConfirmDeletingRecord(Record record)
        {
            void action() => DeleteRecord(record);
            MessageBoxService.ShowConfirm(AppResources.DeleteRecord, AppResources.AskDeleteRecord, null, action);
        }

        private void DeleteRecord(Record record)
        {
            Records.Remove(record);
            InitRecordsForSearch();

            WeekRecord.DayRecords.Remove(record);
            App.AlarmsRepo.DeleteRecord(record.Id);
            SendMessage("deleteRecord", record);
            OnPropertyChanged(nameof(WeekSuccessRate));
            SetDayRecords();
        }

        private void ConfirmChangingSuccessRecord(Record record)
        {
            void action() => ChangeSuccessRecord(record);
            MessageBoxService.ShowConfirm(AppResources.ChangeSuccess, AppResources.ChangeSuccessDetail, null, action);
        }

        private void ChangeSuccessRecord(Record record)
        {
            record.IsSuccess = true;
            App.AlarmsRepo.SaveRecord(record);
            SendMessage("changeWeekRecord", WeekRecord);
            OnPropertyChanged(nameof(WeekSuccessRate));
            SetDayRecords();
        }

        public class DayRecord : List<Record>
        {
            public string DayOfWeek { get; set; }
            public List<Record> DayRecords => this;
            public string NumOfDayRecords
            {
                get
                {
                    if (DayRecords.Count == 0) { return AppResources.NoRecord; }

                    switch (CultureInfo.CurrentCulture.Name)
                    {
                        case "ko-KR":
                            return $"{DayRecords.Count} 개";
                        case "en-US":
                            return $"{DayRecords.Count} Alarms";
                        default:
                            return $"{DayRecords.Count} Alarms";
                    }
                }
            }

            public DateTime Date { get; set; }

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
