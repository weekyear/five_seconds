using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public RecordDetailViewModel(INavigation navigation, WeekRecord weekRecord, List<Record> allRecords, IMessageBoxService messageBoxService) : base(navigation)
        {
            ConstructCommand();

            MessageBoxService = messageBoxService;

            Records = allRecords;

            WeekRecord = weekRecord;

            CreateTagItems();

            SetDayRecords();
        }


        private void ConstructCommand()
        {
            SearchCommand = new Command<string>((t) => Search(t));
            RemoveTagCommand = new Command<TagItem>((t) => RemoveTag(t));
            CloseCommand = new Command(async () => await ClosePopup());
            PreviousWeekCommand = new Command(() => PreviousWeek());
            NextWeekCommand = new Command(() => NextWeek());
            ShowRecordMenuCommand = new Command<object>(async (a) => await ShowRecordMenu(a));
        }

        // Command

        public Command<string> SearchCommand { get; set; }
        public Command<TagItem> RemoveTagCommand { get; set; }
        public Command CloseCommand { get; set; }
        public Command PreviousWeekCommand { get; set; }
        public Command NextWeekCommand { get; set; }
        public Command<object> ShowRecordMenuCommand { get; set; }

        // Property

        public List<Record> Records { get; set; }

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
                if (DayRecords.Count == 0)
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

            if (tagItem != null)
            {
                recordsByTag = Records.FindAll((r) => r.Name.Contains(tagItem.Name));
            }

            if (TagItems.Count == 0 && recordsByTag.Count == 0)
            {
                RecordsByTag = Records;
            }
            else
            {
                foreach (var tag in TagItems)
                {
                    var records = Records.FindAll((r) => r.Name.Contains(tag.Name));
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

            var monList = new DayRecord() { DayOfWeek = "월", Date = startDateOfSelectedWeek };
            var tueList = new DayRecord() { DayOfWeek = "화", Date = startDateOfSelectedWeek.AddDays(1) };
            var wedList = new DayRecord() { DayOfWeek = "수", Date = startDateOfSelectedWeek.AddDays(2) };
            var thuList = new DayRecord() { DayOfWeek = "목", Date = startDateOfSelectedWeek.AddDays(3) };
            var friList = new DayRecord() { DayOfWeek = "금", Date = startDateOfSelectedWeek.AddDays(4) };
            var satList = new DayRecord() { DayOfWeek = "토", Date = startDateOfSelectedWeek.AddDays(5) };
            var sunList = new DayRecord() { DayOfWeek = "일", Date = startDateOfSelectedWeek.AddDays(6) };

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

            foreach (var dayList in dayRecordList)
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
                MessageBoxService.ShowAlert("검색어 개수 초과", "검색어 수는 7개까지만 허용됩니다.");
                return;
            }

            var _tagItem = SearchTag.ValidateAndReturn(tag);

            if (_tagItem != null)
            {
                UpdateWeekRecord(_tagItem);
                SendMessage("addTag", _tagItem);
            }

            TagItems.Add(_tagItem);
        }

        public async Task ShowRecordMenu(object _record)
        {
            var record = _record as Record;
            string[] actionSheetBtns = { "성공", "실패" };

            string action = await MessageBoxService.ShowActionSheet("기록 수정", "취소", null, actionSheetBtns);

            ClickRecordMenuAction(action, record);

            SetDayRecords();
        }

        private void ClickRecordMenuAction(string action, Record record)
        {
            //var dayRecord = DayRecords.Where(d => d.Date.DayOfWeek == record.Date.DayOfWeek).FirstOrDefault();

            //var _record = dayRecord.Find(r => r.Id == record.Id);

            foreach (var dayRecord in DayRecords)
            {
                foreach (var _record in dayRecord)
                {
                    if (_record.Id == record.Id)
                    {
                        if (action == "성공" && !_record.IsSuccess)
                        {
                            _record.IsSuccess = true;
                        }
                        else if (action == "실패" && _record.IsSuccess)
                        {
                            _record.IsSuccess = false;
                        }
                        else
                        {
                            return;
                        }

                        OnPropertyChanged(nameof(WeekSuccessRate));

                        App.AlarmsRepo.SaveRecord(_record);

                        SendMessage("changeWeekRecord", WeekRecord);
                    }
                }
            }
            //OnPropertyChanged(nameof(DayRecords));
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
                    if (DayRecords.Count == 0) { return "기록 없음"; }
                    return $"{DayRecords.Count} 개";
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
