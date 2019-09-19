using Android.Content;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using SQLite;
using System;
using System.Collections.Generic;

namespace Five_Seconds.Droid.Services
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public static Mission mission;
        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("OnReceive_AlarmReceiver");
            var id = intent.GetIntExtra("id", 0);
            var deviceStorage = new DeviceStorageAndroid();
            var itemDatabase = new ItemDatabaseGeneric(new SQLiteConnection(deviceStorage.GetFilePath("MissionsSQLite.db3")));
            var missionsRepo = new MissionsRepository(itemDatabase);

            Console.WriteLine("After CreateDB_AlarmReceiver");
            Console.WriteLine($"deviceStorage is Null ? : {deviceStorage == null}");
            Console.WriteLine($"itemDatabase is Null ? : {itemDatabase == null}");
            Console.WriteLine($"{missionsRepo}");
            Console.WriteLine($"missionsRepo is Null ? : {missionsRepo == null}");

            if (missionsRepo != null)
            {
                Console.WriteLine(id);
                mission = missionsRepo.GetMission(id);
                Console.WriteLine(mission.AlarmId);
                mission.Alarm = missionsRepo.GetAlarm(mission.AlarmId);
                Console.WriteLine(mission.Alarm.DaysId);
                mission.Alarm.Days = missionsRepo.GetDaysOfWeek(mission.Alarm.DaysId);

                Console.WriteLine("After FindMission_AlarmReceiver");
            }
            else
            {

                Console.WriteLine("return");
                return;
            }

            if (mission.IsActive)
            {
                AlarmController.SetNextAlarm(mission);
                Console.WriteLine("After SetNextAlarm_AlarmReceiver");
                StartAlarmActivity(context, id);

                Console.WriteLine("After StartAlarmActivity_AlarmReceiver");
            }
        }

        private void StartAlarmActivity(Context context, int id)
        {
            var disIntent = new Intent(context, typeof(AlarmActivity));
            disIntent.PutExtra("id", id);
            disIntent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(disIntent);
        }
    }
}