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

        IAlarmSetter alarmSetter = DependencyService.Get<IAlarmSetter>();

        public MissionService(IMissionsRepository repository)
        {
            Repository = repository;

            Missions = GetAllMissions();
        }

        private ObservableCollection<Mission> GetAllMissions()
        {
            var alarms = AssignDaysToAlarms();
            var missions = AssignAlarmToMission(alarms);
            missions = AssignRecordToMission(missions);
            return ConvertListToObservableCollection(missions);
        }

        private List<Alarm> AssignDaysToAlarms()
        {
            var alarms = App.MissionsRepo.AlarmsFromDB;

            foreach (var days in App.MissionsRepo.DaysOfWeeksFromDB)
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
            var missions = App.MissionsRepo.MissionsFromDB;

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
            var records = App.MissionsRepo.RecordFromDB;

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
            alarmSetter.DeleteAlarm(mission.Id);
            var id = Repository.DeleteMission(mission.Id);
            Repository.DeleteAlarm(mission.Id);
            Repository.DeleteDaysOfWeek(mission.Id);
            Missions.Remove(mission);
            SendMessage("delete");
            return id;
        }

        public int SaveMission(Mission mission)
        {
            var id = SaveMissionAtLocal(mission);
            alarmSetter.SetAlarm(mission);
            return id;
        }

        public int SaveMissionAtLocal(Mission mission)
        {
            mission.Alarm.DaysId = Repository.SaveDaysOfWeek(mission.Alarm.Days);
            mission.AlarmId = Repository.SaveAlarm(mission.Alarm);
            Repository.SaveMission(mission);
            AddOrModifyMissionToMissions(mission);
            SendMessage("save");
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

        private void SendMessage(string type)
        {
            var messageType = type;
            MessagingCenter.Send(this, messageType);
        }
    }
}
