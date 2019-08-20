using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Repository
{
    public class MissionsRepository : IMissionsRepository
    {
        public ItemDatabaseGeneric ItemDatabase { get; } = App.ItemDatabase;

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
            return ItemDatabase.GetObject<Mission>(id);
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
            return ItemDatabase.SaveObject(mission);
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
            ItemDatabase.DeleteAllObjects<Mission>();
        }

        private void SendMessage(string type)
        {
            var messageType = type;
            MessagingCenter.Send(this, messageType);
        }
    }
}
