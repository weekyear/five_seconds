using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Repository
{
    public class MissionsRepository : IMissionsRepository
    {
        public ItemDatabaseGeneric ItemDatabase { get; } = App.ItemDatabase;

        IAlarmSetter alarmSetter = DependencyService.Get<IAlarmSetter>();

        private ObservableCollection<Mission> missions = new ObservableCollection<Mission>();
        public ObservableCollection<Mission> Missions
        {
            get => missions;
            set
            {
                if (missions == value) return;
                missions = value;
            }
        }

        public MissionsRepository()
        {
            if (Device.RuntimePlatform == "Test") return;
        }

        public Mission GetMission(int id)
        {
            var mission = ItemDatabase.GetObject<Mission>(id);
            mission.Alarm = ItemDatabase.GetObject<Alarm>(mission.AlarmId);
            mission.Alarm.Days = ItemDatabase.GetObject<DaysOfWeek>(mission.Alarm.DaysId);
            return mission;
        }

        public IEnumerable<Mission> GetFirstMissions()
        {
            return ItemDatabase.GetObjects<Mission>();
        }

        public IEnumerable<Mission> GetMissions()
        {
            return ItemDatabase.GetObjects<Mission>();
        }

        public int SaveMission(Mission mission)
        {
            AddOrModifyMissionToMissions(mission);
            SendMessage("save");
            mission.Alarm.DaysId = ItemDatabase.SaveObject(mission.Alarm.Days);
            mission.AlarmId = ItemDatabase.SaveObject(mission.Alarm);
            var id = ItemDatabase.SaveObject(mission);
            alarmSetter.SetAlarm(mission);
            return id;
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

        public int DeleteMission(int id)
        {
            alarmSetter.DeleteAlarm(id);
            DeleteMissionOfMissions(id);
            SendMessage("delete");
            return ItemDatabase.DeleteObject<Mission>(id);
        }
        private void DeleteMissionOfMissions(int id)
        {
            for (int i = 0; i < Missions.Count; i++)
            {
                if (Missions[i].Id == id)
                {
                    Missions.RemoveAt(i);
                    return;
                }
            }
        }

        public void DeleteAllMissions()
        {
            DeleteAllAlarms();
            ItemDatabase.DeleteAllObjects<Mission>();
        }

        public void DeleteAllAlarms()
        {
            var listMission = GetMissions() as List<Mission>;
            alarmSetter.DeleteAllAlarms(listMission);
        }

        public Alarm GetAlarm(int id)
        {
            return GetMission(id).Alarm;
        }

        public List<Alarm> GetAllAlarms()
        {
            var allMissions = GetMissions();
            var listAlarm = new List<Alarm>();

            foreach (var mission in allMissions)
            {
                if (mission.Alarm.IsActive == true)
                {
                    listAlarm.Add(mission.Alarm);
                }
            }
            return listAlarm;
        }

        private void SendMessage(string type)
        {
            var messageType = type;
            MessagingCenter.Send(this, messageType);
        }
    }
}
