using Android.App;
using Android.Content;
using Five_Seconds.Droid;
using Five_Seconds.Droid.Services;
using Five_Seconds.Services;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(CountDownAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class CountDownAndroid : ICountDown
    {
        public void ShowCountDown()
        {
            var disIntent = new Intent(Application.Context, typeof(AlarmActivity));
            disIntent.PutExtra("id", -1);
            disIntent.SetFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(disIntent);
        }
    }
}