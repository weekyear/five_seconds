using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Five_Seconds.Models
{
    public interface ILocalData
    {
        // getters
        ObservableCollection<Mission> Missions { get; set; }

        // methods
        Mission GetMission(int id);
        IEnumerable<Mission> GetFirstMissions();
        IEnumerable<Mission> GetMissions();
        int SaveMission(Mission mission);
        int DeleteMission(int id);
        void DeleteAllMissions();
    }
}
