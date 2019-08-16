using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Models
{
    public class LocalData : ILocalData
    {
        ItemDatabaseGeneric itemDatabase = null;

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

        public LocalData()
        {
            if (Device.RuntimePlatform == "Test") return;
            itemDatabase = new ItemDatabaseGeneric();
        }
        public Mission GetMission(int id)
        {
            return itemDatabase.GetObject<Mission>(id);
        }
        public IEnumerable<Mission> GetFirstMissions()
        {
            return itemDatabase.GetObjects<Mission>();
        }
        public IEnumerable<Mission> GetMissions()
        {
            return itemDatabase.GetObjects<Mission>();
        }
        public int SaveMission(Mission mission)
        {
            AddOrModifyMissionToMissions(mission);
            SendMessage("save");
            return itemDatabase.SaveObject(mission);
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
            DeleteMissionOfMissions(id);
            SendMessage("delete");
            return itemDatabase.DeleteObject<Mission>(id);
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
            itemDatabase.DeleteAllObjects<Mission>();
        }

        private void SendMessage(string type)
        {
            var messageType = type;
            MessagingCenter.Send(this, messageType);
        }
    }
}
