using Five_Seconds.Models;
using Five_Seconds.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Services
{
    public class MissionService : IMissionService
    {
        public IMissionsRepository Repository { get; }
        public ObservableCollection<Mission> Missions { get; } = new ObservableCollection<Mission>();

        public static int FinalMissionId;

        public MissionService(IMissionsRepository repository)
        {
            Repository = repository;

            Missions = GetAllMissions();
        }

        public ObservableCollection<Mission> GetAllMissions()
        {
            Console.WriteLine("GetAllMissions_MissionService");
            var alarms = AssignDaysToAlarms();
            Console.WriteLine("After AssignDaysToAlarms_MissionService");
            var missions = AssignAlarmToMission(alarms);
            Console.WriteLine("After AssignAlarmToMission_MissionService");
            missions = AssignRecordToMission(missions);
            Console.WriteLine("After AssignRecordToMission_MissionService");
            return ConvertListToObservableCollection(missions);
        }

        private List<Alarm> AssignDaysToAlarms()
        {
            Console.WriteLine("In AssignDaysToAlarms_MissionService");
            var alarms = Repository.AlarmsFromDB;

            Console.WriteLine("Get AlarmsFromDB_MissionService");
            foreach (var days in Repository.DaysOfWeeksFromDB)
            {
                for (int i = 0; i < alarms.Count; i++)
                {
                    if (alarms[i].Id == days.Id)
                    {
                        alarms[i].Days = days;
                    }
                }
            }

            return alarms;
        }

        private List<Mission> AssignAlarmToMission(List<Alarm> alarms)
        {
            var missions = Repository.MissionsFromDB;

            foreach (var alarm in alarms)
            {
                for (int i = 0; i < missions.Count; i++)
                {
                    if (missions[i].Id == alarm.Id)
                    {
                        missions[i].Alarm = alarm;
                    }
                }
            }

            return missions;
        }

        private List<Mission> AssignRecordToMission(List<Mission> missions)
        {
            var records = Repository.RecordFromDB;

            foreach (var record in records)
            {
                for (int i = 0; i < missions.Count; i++)
                {
                    if (missions[i].Id == record.Id)
                    {
                        missions[i].Records.Add(record);
                    }
                }
            }

            return missions;
        }

        private ObservableCollection<T> ConvertListToObservableCollection<T>(List<T> list)
        {
            var collection = new ObservableCollection<T>();
            foreach (var item in list)
            {
                collection.Add(item);
            }
            return collection;
        }

        public Mission GetMission(int id)
        {
            return Missions.FirstOrDefault(m => m.Id == id);
        }

        public int DeleteMission(Mission mission)
        {
            DependencyService.Get<IAlarmSetter>().DeleteAlarm(mission.Id);
            var id = Repository.DeleteMission(mission.Id);
            Repository.DeleteAlarm(mission.Id);
            Repository.DeleteDaysOfWeek(mission.Id);
            
            foreach(var m in Missions)
            {
                if (m.Id == mission.Id)
                {
                    Missions.Remove(m);
                    break;
                }
            }

            SendChangeMissionsMessage();
            //DependencyService.Get<IAlarmNotification>().UpdateNotification();
            return id;
        }

        public int SaveMission(Mission mission)
        {
            var id = SaveMissionAtLocal(mission);
            DependencyService.Get<IAlarmSetter>().SetAlarm(mission);
            //DependencyService.Get<IAlarmNotification>().UpdateNotification();
            return id;
        }

        public int SaveMissionAtLocal(Mission mission)
        {
            mission.Alarm.DaysId = Repository.SaveDaysOfWeek(mission.Alarm.Days);
            mission.AlarmId = Repository.SaveAlarm(mission.Alarm);
            Repository.SaveMission(mission);
            AddOrModifyMissionToMissions(mission);
            SendChangeMissionsMessage();
            return mission.Id;
        }

        private void AddOrModifyMissionToMissions(Mission mission)
        {
            for (int i = 0; i < Missions.Count; i++)
            {
                if (Missions[i].Id == mission.Id)
                {
                    Missions[i] = mission;
                    return;
                }
            }
            Missions.Add(mission);
        }

        public void DeleteAllMissions()
        {
            foreach (var mission in Missions)
            {
                DeleteMission(mission);
            }
        }



        public Alarm GetNextAlarm()
        {
            DateTime min = DateTime.MaxValue;
            var listMission = Missions;

            if (listMission.Count == 0) return null;

            var nextAlarm = new Alarm() { Date = DateTime.MaxValue.Date };

            for (int i = 0; i < listMission.Count; i++)
            {
                if (listMission[i].IsActive)
                {
                    var alarmNextTime = listMission[i].Alarm.NextAlarmTime;

                    if (min.Subtract(alarmNextTime).TotalMilliseconds > 0)
                    {
                        min = alarmNextTime;
                        nextAlarm = listMission[i].Alarm;
                    }
                }
            }

            if (nextAlarm.Date == DateTime.MaxValue.Date)
            {
                return null;
            }
            return nextAlarm;
        }

        public void SendChangeMissionsMessage()
        {
            MessagingCenter.Send(this, "changeMissions");
        }
    }
}
