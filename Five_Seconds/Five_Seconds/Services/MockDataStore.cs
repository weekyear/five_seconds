//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Threading.Tasks;
//using Five_Seconds.Models;

//namespace Five_Seconds.Services
//{
//    public class MockDataStore : IDataStore<Mission>
//    {
//        List<Mission> missions;

//        public MockDataStore()
//        {
//            var dateNow = DateTime.UtcNow;

//            Record record1 = new Record() { Date = DateTime.UtcNow, IsSuccess = false, RecordTime = 5 };
//            Record record2 = new Record() { Date = DateTime.UtcNow.AddDays(1), IsSuccess = false, RecordTime = 3 };
//            Record record3 = new Record() { Date = DateTime.UtcNow.AddDays(2), IsSuccess = true, RecordTime = 7 };

//            var records = new ObservableCollection<Record>();
//            records.Add(record3);
//            records.Add(record2);
//            records.Add(record1);

//            missions = new List<Mission>();
//            var mockItems = new List<Mission>
//            {
//                new Mission { Description = "일어나기", Time = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 1, 20, 00), Percentage = "80%", Records = records },
//                new Mission { Description = "운동하기", Time = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 2, 30, 00), Percentage = "40%" },
//                new Mission { Description = "공부하기", Time = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 3, 40, 00), Percentage = "50%" },
//                new Mission { Description = "잠자기", Time = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 4, 50, 00), Percentage = "70%" }
//            };

//            foreach (var item in mockItems)
//            {
//                missions.Add(item);
//            }
//        }

//        public async Task<bool> AddItemAsync(Mission item)
//        {
//            missions.Add(item);

//            return await Task.FromResult(true);
//        }

//        public async Task<bool> UpdateItemAsync(Mission item)
//        {
//            var oldItem = missions.Where((Mission arg) => arg.Id == item.Id).FirstOrDefault();
//            missions.Remove(oldItem);
//            missions.Add(item);

//            return await Task.FromResult(true);
//        }

//        public async Task<bool> DeleteItemAsync(string id)
//        {
//            var oldItem = missions.Where((Mission arg) => arg.Id == id).FirstOrDefault();
//            missions.Remove(oldItem);

//            return await Task.FromResult(true);
//        }

//        public async Task<Mission> GetItemAsync(string id)
//        {
//            return await Task.FromResult(missions.FirstOrDefault(s => s.Id == id));
//        }

//        public async Task<IEnumerable<Mission>> GetItemsAsync(bool forceRefresh = false)
//        {
//            return await Task.FromResult(missions);
//        }
//    }
//}