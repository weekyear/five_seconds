using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Five_Seconds.Droid.BroadcastReceivers;
using Five_Seconds.Droid.Services;
using Five_Seconds.Models;
using Five_Seconds.Services;
using Environment = System.Environment;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class NotificationAndroid : IAlarmNotification
    {
        private static readonly string NOTIFICATION_CHANNEL_ID = "com.beside.five_seconds";
        private static readonly string channelName = "Alarm";

        public static NotificationManager SetNotificationManager()
        {
            var manager = Application.Context.GetSystemService("notification") as NotificationManager;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, channelName, NotificationImportance.Low);


                manager?.CreateNotificationChannel(chan);
            }

            return manager;
        }

        public void CancelAllNotification()
        {
            var notificationManager = Application.Context.GetSystemService("notification") as NotificationManager;
            notificationManager.CancelAll();
        }

        public static void NotifyPreAlarm(Alarm alarm, Intent intent)
        {
            var context = Application.Context;

            var manager = SetNotificationManager();

            string nameString;
            string timeString;

            if (alarm != null)
            {
                nameString = $"'{alarm.Name}' 알람이 곧 울립니다.";

                switch (CultureInfo.CurrentCulture.Name)
                {
                    case "ko-KR":
                        timeString = $"{alarm.TimeOffset.DateTime.ToShortTimeString()}";
                        break;
                    case "en-US":
                        timeString = $"{alarm.TimeOffset.DateTime.ToShortTimeString()}";
                        break;
                    default:
                        timeString = $"{alarm.TimeOffset.DateTime.ToShortTimeString()}";
                        break;
                }
            }
            else
            {
                nameString = "다음 알람이 없습니다.";
                timeString = string.Empty;
            }

            var bundle = intent.Extras;

            var actionIntent1 = CreateActionIntent(bundle, context.GetString(Resource.String.AlarmPreOff));
            var pIntent1 = PendingIntent.GetBroadcast(context, alarm.Id, actionIntent1, PendingIntentFlags.OneShot);

            var actionIntent2 = CreateActionIntent(bundle, "GoOffPre");
            var pIntent2 = PendingIntent.GetBroadcast(context, alarm.Id, actionIntent2, PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(true)
                    .SetSmallIcon(Resource.Drawable.ic_five_seconds_mini)
                    .SetContentTitle(nameString)
                    .SetContentText(timeString)
                    .SetPriority((int)NotificationImportance.Default)
                    .SetVisibility(NotificationCompat.VisibilityPublic)
                    .SetContentIntent(OpenAppIntent())
                    .AddAction(0, context.GetString(Resource.String.AlarmPreOff), pIntent1)
                    .AddAction(0, context.GetString(Resource.String.GoOffNow), pIntent2)
                    .SetAutoCancel(false)
                    .Build();

            manager.Notify(alarm.Id, notification);
        }

        public static void NotifyFailedAlarm(Alarm alarm, DateTime alarmTime)
        {
            var context = Application.Context;

            var manager = SetNotificationManager();

            string title;
            string message;

            if (alarm != null)
            {
                title = context.GetString(Resource.String.Failure);
                switch (CultureInfo.CurrentCulture.Name)
                {
                    case "ko-KR":
                        message = $"{alarmTime.ToShortTimeString()}에 {alarm.Name}를 실패하였습니다.";
                        break;
                    case "en-US":
                        message = $"You failed {alarm.Name} at {alarmTime.ToShortTimeString()}";
                        break;
                    default:
                        message = $"You failed {alarm.Name} at {alarmTime.ToShortTimeString()}";
                        break;
                }
            }
            else
            {
                title = context.GetString(Resource.String.Failure);
                message = string.Empty;
            }

            var notificationBuilder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(false)
                    .SetSmallIcon(Resource.Drawable.ic_five_seconds_mini)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetPriority((int)NotificationImportance.Default)
                    .SetVisibility(NotificationCompat.VisibilityPublic)
                    .SetContentIntent(OpenAppIntent())
                    .SetAutoCancel(true)
                    .Build();

            manager.Notify(alarm.Id, notification);
        }

        public static void NotifyLaterAlarm(Alarm alarm, Intent intent)
        {
            var context = Application.Context;

            var manager = SetNotificationManager();

            string nameString;
            string timeString;

            if (alarm != null)
            {
                nameString = alarm.Name;

                switch (CultureInfo.CurrentCulture.Name)
                {
                    case "ko-KR":
                        timeString = $"{alarm.LaterAlarmTime.ToShortTimeString()}에 울립니다";
                        break;
                    case "en-US":
                        timeString = $"Alarm will ring at {alarm.LaterAlarmTime.ToShortTimeString()}";
                        break;
                    default:
                        timeString = $"Alarm will ring at {alarm.LaterAlarmTime.ToShortTimeString()}";
                        break;
                }
            }
            else
            {
                nameString = "다음 알람이 없습니다.";
                timeString = string.Empty;
            }

            var bundle = intent.Extras;

            var actionIntent1 = CreateActionIntent(bundle, context.GetString(Resource.String.AlarmOff));
            var pIntent1 = PendingIntent.GetBroadcast(context, alarm.Id, actionIntent1, PendingIntentFlags.OneShot);

            var actionIntent2 = CreateActionIntent(bundle, context.GetString(Resource.String.GoOffNow));
            var pIntent2 = PendingIntent.GetBroadcast(context, alarm.Id, actionIntent2, PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(true)
                    .SetSmallIcon(Resource.Drawable.ic_five_seconds_mini)
                    .SetContentTitle(nameString)
                    .SetContentText(timeString)
                    .SetPriority((int)NotificationImportance.Default)
                    .SetVisibility(NotificationCompat.VisibilityPublic)
                    .SetContentIntent(OpenAppIntent())
                    .AddAction(0, context.GetString(Resource.String.AlarmOff), pIntent1)
                    .AddAction(0, context.GetString(Resource.String.GoOffNow), pIntent2)
                    .SetAutoCancel(false)
                    .Build();

            manager.Notify(alarm.Id, notification);
        }

        public static void NotifyFeedbackAlarm(Alarm alarm, double previousRate, double recentRate)
        {
            var context = Application.Context;

            var manager = SetNotificationManager();

            string title;
            string message;
            string bigText;

            if (alarm != null)
            {
                if (previousRate > recentRate)
                {
                    title = $"'{alarm.Name}'의 성공률이 좋지 않아요 ㅠ";
                    message = $"최근 '{alarm.Name}'의 성공률이 이전 성공률보다 {(previousRate - recentRate) * 100:0.##}% 낮네요. 조금 더 힘내봅시다!!";
                    bigText = $"최근 '{alarm.Name}'의 성공률이 이전 성공률보다 {(previousRate - recentRate) * 100:0.##}% 낮네요. 조금 더 힘내봅시다!!"
                        + Environment.NewLine
                        + Environment.NewLine
                        + $"앱에서 '{alarm.Name}'의 성공률을 확인해보세요~";
                }
                else
                {
                    title = $"'{alarm.Name}'의 성공률이 좋네요!";
                    message = $"최근 '{alarm.Name}'의 성공률이 이전 성공률보다 {(recentRate - previousRate) * 100:0.##}% 높아요. 지금 이대로 계속 해봐요!!";
                    bigText = $"최근 '{alarm.Name}'의 성공률이 이전 성공률보다 {(recentRate - previousRate) * 100:0.##}% 높아요. 지금 이대로 계속 해봐요!!"
                        + Environment.NewLine
                        + Environment.NewLine
                        + $"앱에서 '{alarm.Name}'의 성공률을 확인해보세요~";
                }
            }
            else
            {
                title = $"알람 오류입니다.";
                message = $"개발자 이메일 jwnsgus@gmail.com으로 오류 보고해주세요.";
                bigText = $"개발자 이메일 jwnsgus@gmail.com으로 오류 보고해주세요.";
            }

            var style = new NotificationCompat.BigTextStyle();
            style.BigText(bigText);

            var notificationBuilder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(false)
                    .SetSmallIcon(Resource.Drawable.ic_five_seconds_mini)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetPriority((int)NotificationImportance.Default)
                    .SetVisibility(NotificationCompat.VisibilityPublic)
                    .SetStyle(style)
                    .SetContentIntent(OpenAppIntent())
                    .SetAutoCancel(true)
                    .Build();

            manager.Notify(alarm.Id, notification);
        }
        
        public static void NotifyFirstFeedbackAlarm(Alarm alarm, double recentRate)
        {
            var context = Application.Context;

            var manager = SetNotificationManager();

            string title;
            string message;
            string bigText;

            if (alarm != null)
            {
                if (recentRate > 0.75)
                {
                    title = $"'{alarm.Name}'의 성공률이 좋네요!!ㅎㅎ";
                    message = $"'{alarm.Name}'의 성공률이 {recentRate * 100:0.##}%네요. 지금 이대로 계속 해봅시다.";
                    bigText = $"'{alarm.Name}'의 성공률이 {recentRate * 100:0.##}%네요. 지금 이대로 계속 해봅시다."
                        + Environment.NewLine
                        + Environment.NewLine
                        + $"앱에서 '{alarm.Name}'의 성공률을 확인해보세요~";
                }
                else if (recentRate < 0.5)
                {
                    title = $"'{alarm.Name}'의 성공률이 좋지 않아요 ㅠ";
                    message = $"'{alarm.Name}'의 성공률이 {recentRate * 100:0.##}%네요. 조금 더 힘내봅시다!!";
                    bigText = $"'{alarm.Name}'의 성공률이 {recentRate * 100:0.##}%네요. 조금 더 힘내봅시다!!"
                        + Environment.NewLine
                        + Environment.NewLine
                        + $"앱에서 '{alarm.Name}'의 성공률을 확인해보세요~";
                }
                else
                {
                    title = $"'{alarm.Name}'의 성공률이 나쁘지 않네요";
                    message = $"'{alarm.Name}'의 성공률이 {recentRate * 100:0.##}%네요. 조금 더 힘내봅시다!!";
                    bigText = $"'{alarm.Name}'의 성공률이 {recentRate * 100:0.##}%네요. 조금 더 힘내봅시다!!"
                        + Environment.NewLine
                        + Environment.NewLine
                        + $"앱에서 '{alarm.Name}'의 성공률을 확인해보세요~";
                }
            }
            else
            {
                title = $"알람 오류입니다.";
                message = $"개발자 이메일 jwnsgus@gmail.com으로 오류 보고해주세요.";
                bigText = $"개발자 이메일 jwnsgus@gmail.com으로 오류 보고해주세요.";
            }

            var style = new NotificationCompat.BigTextStyle();
            style.BigText(bigText);

            var notificationBuilder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(false)
                    .SetSmallIcon(Resource.Drawable.ic_five_seconds_mini)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetPriority((int)NotificationImportance.Default)
                    .SetVisibility(NotificationCompat.VisibilityPublic)
                    .SetStyle(style)
                    .SetContentIntent(OpenAppIntent())
                    .SetAutoCancel(true)
                    .Build();

            manager.Notify(alarm.Id, notification);
        }


        public static void NotifyAppComeback()
        {
            var context = Application.Context;

            var manager = SetNotificationManager();

            string title = "5초의 알람으로 돌아와요~";
            string message = "5초의 알람엔 기록, 음성 알람, 의지 알람 등 많은 기능이 있어요."
                        + Environment.NewLine
                        + Environment.NewLine
                        + "앱에서 많은 기능을 확인해보세요~";
            string bigText = "5초의 알람엔 기록, 음성 알람, 의지 알람 등 많은 기능이 있어요."
                        + Environment.NewLine
                        + Environment.NewLine
                        + "앱에서 많은 기능을 확인해보세요~";

            var style = new NotificationCompat.BigTextStyle();
            style.BigText(bigText);

            var notificationBuilder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(false)
                    .SetSmallIcon(Resource.Drawable.ic_five_seconds_mini)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetPriority((int)NotificationImportance.Default)
                    .SetVisibility(NotificationCompat.VisibilityPublic)
                    .SetStyle(style)
                    .SetContentIntent(OpenAppIntent())
                    .SetAutoCancel(true)
                    .Build();

            manager.Notify(-98, notification);
        }

        public static void NotifyAlarmComeback()
        {
            var context = Application.Context;

            var manager = SetNotificationManager();

            string title = "5초의 알람을 잊으신건 아니시죠..";
            string message = "5초의 법칙으로 삶을 바꾸겠다는 의지를 떠올리세요! 마포대교는 무너져도 우리의 굳건한 의지는 무너지지 않습니다!"
                        + Environment.NewLine
                        + Environment.NewLine
                        + "앱에서 5초의 알람을 설정해보세요~";
            string bigText = "5초의 법칙으로 삶을 바꾸겠다는 의지를 떠올리세요! 마포대교는 무너져도 우리의 굳건한 의지는 무너지지 않습니다!"
                        + Environment.NewLine
                        + Environment.NewLine
                        + "앱에서 5초의 알람을 설정해보세요~";

            var style = new NotificationCompat.BigTextStyle();
            style.BigText(bigText);

            var notificationBuilder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID);
            var notification = notificationBuilder.SetOngoing(false)
                    .SetSmallIcon(Resource.Drawable.ic_five_seconds_mini)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetPriority((int)NotificationImportance.Default)
                    .SetVisibility(NotificationCompat.VisibilityPublic)
                    .SetStyle(style)
                    .SetContentIntent(OpenAppIntent())
                    .SetAutoCancel(true)
                    .Build();

            manager.Notify(-99, notification);
        }

        private static Intent CreateActionIntent(Bundle bundle, string action)
        {
            var actionIntent = new Intent(Application.Context, typeof(NotificationReceiver));
            actionIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            actionIntent.PutExtras(bundle);
            actionIntent.SetAction(action);

            return actionIntent;
        }

        private static PendingIntent OpenAppIntent()
        {
            Intent notificationIntent = new Intent(Application.Context, typeof(MainActivity));

            notificationIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            
            PendingIntent pendingIntent = PendingIntent.GetActivity(Application.Context, 0, notificationIntent, 0);

            return pendingIntent;
        }

        public static void CancelLaterNotification(Context context, int id)
        {
            NotificationManager manager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            if (id != -100000)
            {
                manager.Cancel(id);
            }
        }
    }
}