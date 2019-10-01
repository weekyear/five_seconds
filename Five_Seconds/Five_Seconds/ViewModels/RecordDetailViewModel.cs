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
        public RecordDetailViewModel(INavigation navigation, WeekRecord weekRecord) : base(navigation)
        {
            ConstructCommand();

            WeekRecord = weekRecord;

            SetDayRecords();
        }


        private void ConstructCommand()
        {
            CloseCommand = new Command(async () => await ClosePopup());
        }

        // Command
        public Command CloseCommand { get; set; }
        public Command PreviousMonthCommand { get; set; }
        public Command NextMonthCommand { get; set; }

        // Property

        public WeekRecord WeekRecord
        {
            get; set;
        }

        public DateTime StartDateOfWeek
        {
            get { return WeekRecord.StartDateOfWeek; }
        }

        public ObservableCollection<DayRecordList> WeekRecords { get; set; } = new ObservableCollection<DayRecordList>();

        private async Task ClosePopup()
        {
            await Navigation.PopAsync(true);
        }

        private void SetDayRecords()
        {
            var monList = new DayRecordList() { DayOfWeek = "월요일" };
            var tueList = new DayRecordList() { DayOfWeek = "화요일" };
            var wedList = new DayRecordList() { DayOfWeek = "수요일" };
            var thuList = new DayRecordList() { DayOfWeek = "목요일" };
            var friList = new DayRecordList() { DayOfWeek = "금요일" };
            var satList = new DayRecordList() { DayOfWeek = "토요일" };
            var sunList = new DayRecordList() { DayOfWeek = "일요일" };

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

            WeekRecords = new ObservableCollection<DayRecordList>
            {
                monList, tueList, wedList, thuList, friList, satList, sunList
            };
        }

        public class DayRecordList : List<Record>
        {
            public string DayOfWeek { get; set; }
            public List<Record> DayRecords => this;
            public string NumOfDayRecords
            {
                get { return $"{DayRecords.Count} 개"; }
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
