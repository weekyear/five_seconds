using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Android.Views;
using Android.Widget;
using Five_Seconds.Services;
using Xamarin.Forms;
using Application = Android.App.Application;

namespace Five_Seconds.Droid.Services
{
    public class SpeechToText_Android : ISpeechToText
    {
        public static AutoResetEvent autoEvent = new AutoResetEvent(false);
        public static string SpeechText;
        const int VOICE = 10;
        public async Task<string> SpeechToTextAsync()
        {
            var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Sprechen Sie jetzt");
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);

            SpeechText = "";
            autoEvent.Reset();
            ((Activity)Application.Context).StartActivityForResult(voiceIntent, VOICE);
            await Task.Run(() => { autoEvent.WaitOne(new TimeSpan(0, 2, 0)); });
            return SpeechText;
        }
    }
}