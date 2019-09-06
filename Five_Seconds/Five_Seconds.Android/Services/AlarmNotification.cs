using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace Five_Seconds.Droid.Services
{
    public class AlarmNotification
    {
        private static string NOTIFICATION_CHANNEL_ID = "com.beside.five_seconds";

        public static Notification GetNextAlarmNotification(Context context)
        {
            string nextNameString;
            string nextTimeString;

            var alarmId = App.MissionsRepo.GetNextAlarmId();

            if (alarmId != 0)
            {
                var mission = App.MissionsRepo.GetMission(alarmId);
                var alarm = App.MissionsRepo.GetAlarm(alarmId);

                nextNameString = mission.Name;
                nextTimeString = alarm.NextAlarmTime.ToString();
            }
            else
            {
                nextNameString = "다음 알람이 없습니다.";
                nextTimeString = string.Empty;
            }
            

            var notificationBuilder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(true)
                    .SetSmallIcon(Resource.Drawable.ic_icon_notificatioin)
                    .SetContentTitle(nextNameString)
                    .SetContentText(nextTimeString)
                    .SetPriority((int)NotificationImportance.Min)
                    .SetCategory(Notification.CategoryService)
                    .SetChannelId(NOTIFICATION_CHANNEL_ID)
                    .Build();

            return notification;
        }
    }
}