using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Repository
{
    public class MissionRepository
    {
        ItemDatabaseGeneric itemDatabase = null;
        public MissionRepository()
        {
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
            return itemDatabase.SaveObject<Mission>(mission);
        }
        public int DeleteMission(int id)
        {
            return itemDatabase.DeleteObject<Mission>(id);
        }
        public void DeleteAllMissions()
        {
            itemDatabase.DeleteAllObjects<Mission>();
        }
    }
}
