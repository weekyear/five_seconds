using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Five_Seconds.ViewModels.RecordViewModel;

namespace Five_Seconds.ViewModels
{
    public class RecordDetailViewModel : BaseViewModel
    {
        public RecordDetailViewModel(INavigation navigation, WeekRecord weekRecord, List<Record> allRecords) : base(navigation)
        {
            ConstructCommand();

            Records = allRecords;

            WeekRecord = weekRecord;

            SetDayRecords();
        }


        private void ConstructCommand()
        {
            CloseCommand = new Command(async () => await ClosePopup());
            PreviousWeekCommand = new Command(() => PreviousWeek());
            NextWeekCommand = new Command(() => NextWeek());
        }

        // Command
        public Command CloseCommand { get; set; }
        public Command PreviousWeekCommand { get; set; }
        public Command NextWeekCommand { get; set; }

        // Property

        public List<Record> Records { get; set; }

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
                UpdateWeekRecord(value);
                OnPropertyChanged(nameof(SelectedWeek));
                OnPropertyChanged(nameof(WeekRecord));
            }
        }

        public ObservableCollection<DayRecordList> WeekRecords { get; set; } = new ObservableCollection<DayRecordList>();
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

        private async Task ClosePopup()
        {
            await Navigation.PopAsync(true);
        }

        private void PreviousWeek()
        {
            SelectedWeek = SelectedWeek.AddDays(-7);

            SetDayRecords();
        }

        private void NextWeek()
        {
            SelectedWeek = SelectedWeek.AddDays(7);

            SetDayRecords();
        }

        private void UpdateWeekRecord(DateTime selectedWeek)
        {
            var weekRecords = new ObservableCollection<WeekRecord>();
            var startDateOfSelectedWeek = selectedWeek;
            var startDateOfWeek = new DateTime(startDateOfSelectedWeek.Year, startDateOfSelectedWeek.Month, startDateOfSelectedWeek.Day, 0, 0, 0);

            while (startDateOfWeek.Date.DayOfWeek != DayOfWeek.Monday) startDateOfWeek = startDateOfWeek.AddDays(-1);

            var startDateOfMonth = startDateOfWeek;

            var RecordsOfWeek = Records.FindAll((r) => r.TimeOffset.DateTime.Ticks > startDateOfWeek.Ticks && r.TimeOffset.DateTime.Ticks <= startDateOfWeek.AddDays(7).Ticks);

            var weekRecord = new WeekRecord()
            {
                StartDateOfWeek = startDateOfWeek,
                DayRecords = RecordsOfWeek
            };

            WeekRecord = weekRecord;
        }

        private void SetDayRecords()
        {
            var startDateOfSelectedWeek = SelectedWeek;

            var monList = new DayRecordList() { DayOfWeek = "월", Date = startDateOfSelectedWeek };
            var tueList = new DayRecordList() { DayOfWeek = "화", Date = startDateOfSelectedWeek.AddDays(1) };
            var wedList = new DayRecordList() { DayOfWeek = "수", Date = startDateOfSelectedWeek.AddDays(2) };
            var thuList = new DayRecordList() { DayOfWeek = "목", Date = startDateOfSelectedWeek.AddDays(3) };
            var friList = new DayRecordList() { DayOfWeek = "금", Date = startDateOfSelectedWeek.AddDays(4) };
            var satList = new DayRecordList() { DayOfWeek = "토", Date = startDateOfSelectedWeek.AddDays(5) };
            var sunList = new DayRecordList() { DayOfWeek = "일", Date = startDateOfSelectedWeek.AddDays(6) };

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

            var dayRecordList = new ObservableCollection<DayRecordList>
            {
                monList, tueList, wedList, thuList, friList, satList, sunList
            };

            WeekRecords.Clear();

            foreach (var dayList in dayRecordList)
            {
                if (dayList.Count != 0)
                {
                    WeekRecords.Add(dayList);
                }
            }
        }

        public class DayRecordList : List<Record>
        {
            public string DayOfWeek { get; set; }
            public List<Record> DayRecords => this;
            public string NumOfDayRecords
            {
                get { return $"{DayRecords.Count} 개"; }
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
