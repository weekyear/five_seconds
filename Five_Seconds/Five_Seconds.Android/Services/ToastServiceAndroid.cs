using Android.App;
using Android.Widget;
using Five_Seconds.Droid.Services;
using Five_Seconds.Services;

[assembly: Xamarin.Forms.Dependency(typeof(ToastServiceAndroid))]
namespace Five_Seconds.Droid.Services
{
    public class ToastServiceAndroid : IToastService
    {
        public void Show(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }
    }
}