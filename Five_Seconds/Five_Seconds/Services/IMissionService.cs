using Five_Seconds.Models;
using Five_Seconds.Repository;
using System.Collections.ObjectModel;

namespace Five_Seconds.Services
{
    public interface IMissionService
    {
        IMissionsRepository Repository { get; }
        ObservableCollection<Mission> Missions { get; }
        Mission GetMission(int id);
        int DeleteMission(Mission mission);
        int SaveMission(Mission mission);
        int SaveMissionAtLocal(Mission mission);
        void DeleteAllMissions();
        void SendChangeMissionsMessage();
    }
}
