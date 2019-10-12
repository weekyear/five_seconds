using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Android.Views;
using Android.Widget;
using Five_Seconds.Droid.Services;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Button = Android.Widget.Button;

namespace Five_Seconds.Droid
{
    [Activity(Label = "5초의 알람", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AlarmActivity : Activity, IRecognitionListener
    {
        readonly IPlaySoundService _soundService = new PlaySoundServiceAndroid();
        Vibrator _vibrator;

        Action DelayedAction;

        private SpeechRecognizer mSpeechRecognizer;
        private Intent mSpeechRecognizerIntent;
        const int MY_PERMISSIONS_RECORD_AUDIO = 2357;
        private LinearLayout alarmTextLayout;
        private LinearLayout alarmLayout;
        private LinearLayout recordingLayout;
        private Button startButton;
        private ImageView tellmeView;
        public TextView countTextView;
        private TextView timeTextView;
        private TextView alarmTextView;
        private TextView pleaseRecordView;
        private TextView pleaseSayText;
        private EditText alarmEditText;
        private CountDown countDown;

        private int id;
        private string name;
        private string toneName;
        private bool IsAlarmOn;
        private bool IsVibrateOn;
        private bool IsCountOn = true;
        private bool IsCountSoundOn = true;
        private bool IsRepeating;
        private int alarmVolume;

        Alarm alarm;
        IAlarmsRepository alarmsRepo;

        public Handler Handler => new Handler();

        public AlarmActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Bundle bundle = Intent.Extras;

            id = (int)bundle.Get("id");

            if (id == -1)
            {
                OnlyCountDown();
                return;
            }

            GetDataFromBundle(bundle);

            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);

            SetMediaPlayer();
            SetVibrator();

            SetContentView(Resource.Layout.AlarmActivity);
            SetControls();

            AddWindowManagerFlags();

            HandleAlarmAfterCalled();

            CreateSpeechRecognizer();

            SetIsFailedCountDown();

            if (bundle == null) return;
        }

        private void SetIsFailedCountDown()
        {
            // 10분 경과
            DelayedAction = () => SetIsSuccessFalse();
            Handler.PostDelayed(DelayedAction, 600000);
            // 1분 경과
            //Handler.PostDelayed(DelayedAction, 10000);
        }

        private void SetIsSuccessFalse()
        {
            var record = new Record(alarm, false);
            alarmsRepo.SaveRecord(record);
            FinishAndRemoveTask();
        }

        private void SetIsSuccessTrue()
        {
            var record = new Record(alarm, true);
            alarmsRepo.SaveRecord(record);
        }

        private void OnlyCountDown()
        {
            SetContentView(Resource.Layout.AlarmActivity);
            SetControlsForCountActivity();
            ShowCountActivity();
        }

        private void HandleAlarmAfterCalled()
        {

            alarmsRepo = App.AlarmsRepo;

            if (alarmsRepo != null)
            {
                alarm = App.AlarmsRepo.GetAlarm(id);
                alarm.Days = App.AlarmsRepo.GetDaysOfWeek(alarm.DaysId);

                if (!IsRepeating)
                {
                    alarm.IsActive = false;
                    App.Service.SaveAlarmAtLocal(alarm);
                }
                else
                {
                    AlarmController.SetNextAlarm(alarm);
                }

                App.Service.SendChangeAlarmsMessage();
            }
        }

        private void GetDataFromBundle(Bundle bundle)
        {
            IsCountSoundOn = (bool)bundle.Get("IsCountSoundOn");
            IsCountOn = (bool)bundle.Get("IsCountOn");
            name = (string)bundle.Get("name");
            toneName = (string)bundle.Get("toneName");
            IsAlarmOn = (bool)bundle.Get("IsAlarmOn");
            IsVibrateOn = (bool)bundle.Get("IsVibrateOn");
            IsRepeating = (bool)bundle.Get("IsRepeating");
            alarmVolume = (int)bundle.Get("alarmVolume");
        }

        private void SetControls()
        {
            alarmTextLayout = FindViewById<LinearLayout>(Resource.Id.alarmTextLayout);
            alarmLayout = FindViewById<LinearLayout>(Resource.Id.alarmLayout);
            recordingLayout = FindViewById<LinearLayout>(Resource.Id.recordingLayout);
            startButton = FindViewById<Button>(Resource.Id.startButton);
            tellmeView = FindViewById<ImageView>(Resource.Id.tellmeView);
            countTextView = FindViewById<TextView>(Resource.Id.countTextView);
            timeTextView = FindViewById<TextView>(Resource.Id.timeTextView);
            alarmTextView = FindViewById<TextView>(Resource.Id.alarmTextView);
            pleaseRecordView = FindViewById<TextView>(Resource.Id.pleaseRecordView);
            pleaseSayText = FindViewById<TextView>(Resource.Id.pleaseSayText);
            alarmEditText = FindViewById<EditText>(Resource.Id.alarmEditText);

            //tellmeButton.Click += TellmeButton_Click;
            countTextView.Text = "5.00";
            alarmTextView.Text = name;
            timeTextView.Text = DateTime.Now.ToShortTimeString();

            tellmeView.Click += StartListening_Click;
            startButton.Click += StartButton_Click;
        }

        private void SetControlsForCountActivity()
        {
            alarmTextLayout = FindViewById<LinearLayout>(Resource.Id.alarmTextLayout);
            alarmLayout = FindViewById<LinearLayout>(Resource.Id.alarmLayout);
            countTextView = FindViewById<TextView>(Resource.Id.countTextView);

            countTextView.Text = "5.00";
        }

        private void AddWindowManagerFlags()
        {
            // add flags to turn screen on and appear over lock screen
            Window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            Window.AddFlags(WindowManagerFlags.DismissKeyguard);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            Window.AddFlags(WindowManagerFlags.TurnScreenOn);
        }

        private void CreateSpeechRecognizer()
        {
            mSpeechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(this);
            mSpeechRecognizerIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, Application.PackageName);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak now");
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);

            mSpeechRecognizer.SetRecognitionListener(this);
        }

        private void StartListening_Click(object sender, EventArgs e)
        {
            RequestRecordAudioPermission();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            HandleVoiceRecognitionResult();
        }

        private void HandleVoiceRecognitionResult()
        {
            mSpeechRecognizer?.StopListening();
            recordingLayout.Visibility = ViewStates.Invisible;

            startButton.Text = "시작!";

            var editText = alarmEditText.Text.Replace(" ", "");
            var textView = alarmTextView.Text.Replace(" ", "");

            if (editText == textView)
            {
                _vibrator?.Cancel();

                ShowCountActivity();

                SetIsSuccessTrue();
            }
            else
            {
                SetVisibilityOfControls();
                ShowAlertDoNotMatchText();
            }
        }

        private void ShowCountActivity()
        {
            if (IsCountOn)
            {
                var toastService = new ToastServiceAndroid();

                toastService.Show("이제 5초를 셉니다!");

                HideAllViewExceptForCountText();

                if (IsCountSoundOn)
                {
                    _soundService.PlayCountAudio();
                }

                SetCountDown();
            }
            else
            {
                FinishAndRemoveTask();
            }
        }

        private void SetVisibilityOfControls()
        {
            tellmeView.Visibility = ViewStates.Gone;
            pleaseSayText.Visibility = ViewStates.Gone;
            alarmEditText.Visibility = ViewStates.Visible;
            startButton.Visibility = ViewStates.Visible;
        }

        private void ShowAlertDoNotMatchText()
        {
            AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            AlertDialog alert = dialog.Create();
            alert.SetTitle("미션 내용 불일치");
            alert.SetMessage("미션 내용을 정확히 기입하여주세요");
            alert.SetButton("확인", (c, ev) =>
            {
                alert.Dispose();
            });
            alert.Show();

            dialog.Dispose();
        }

        private void SetMediaPlayer()
        {
            if (IsAlarmOn)
            {
                AlarmTone alarmTone = AlarmTone.Tones.Find(a => a.Name == toneName);
                _soundService.PlayAudio(alarmTone, true, alarmVolume);
            }
        }

        private void SetVibrator()
        {
            if (IsVibrateOn)
            {
                _vibrator = Vibrator.FromContext(this);
                long[] mVibratePattern = new long[] { 0, 400, 1000, 600, 1000, 800, 1000, 1000 };
                VibrationEffect effect = VibrationEffect.CreateWaveform(mVibratePattern, 0);
                _vibrator.Vibrate(effect);
            }
        }

        private void HideAllViewExceptForCountText()
        {
            alarmTextLayout.Visibility = ViewStates.Invisible;
            alarmLayout.Visibility = ViewStates.Invisible;
            countTextView.Visibility = ViewStates.Visible;
        }

        private async void SetCountDown()
        {
            await Task.Delay(330);
            countDown = new CountDown(5000, 10, this);
            countDown.Start();
        }

        public override void OnBackPressed()
        {
        }

        public void OnBeginningOfSpeech()
        {
        }

        public void OnBufferReceived(byte[] buffer)
        {
        }

        public void OnEndOfSpeech()
        {
        }

        public void OnError(SpeechRecognizerError error)
        {
            //string message;
            switch (error)
            {

                case SpeechRecognizerError.Audio:
                    //message = "오디오 에러입니다.";
                    break;

                case SpeechRecognizerError.Client:
                    //message = "클라이언트 에러입니다.";
                    break;

                case SpeechRecognizerError.InsufficientPermissions:
                    Toast.MakeText(ApplicationContext, "오디오 녹음 권한을 허용해주세요", ToastLength.Long).Show();
                    break;

                case SpeechRecognizerError.Network:
                    Toast.MakeText(ApplicationContext, "네트워크 에러입니다. 네트워크 연결 확인을 해주세요.", ToastLength.Long).Show();
                    break;

                case SpeechRecognizerError.NetworkTimeout:
                    //message = "네트워크 시간초과입니다.";
                    break;

                case SpeechRecognizerError.NoMatch:
                    //message = "해당 음성 녹음 결과가 없습니다."; ;
                    break;

                case SpeechRecognizerError.RecognizerBusy:
                    //message = "다시 시도해주세요.";
                    break;

                case SpeechRecognizerError.Server:
                    Toast.MakeText(ApplicationContext, "서버에 문제가 있습니다. 텍스트 창에 해야할 일을 적어주세요.", ToastLength.Long).Show();
                    break;

                case SpeechRecognizerError.SpeechTimeout:
                    Toast.MakeText(ApplicationContext, "음성 녹음 시간이 초과되었습니다. 텍스트 창에 해야할 일을 적어주세요", ToastLength.Long).Show();
                    break;

                default:
                    //message = "알수없음";
                    break;
            }

        }

        public void OnEvent(int eventType, Bundle @params)
        {
        }

        public void OnPartialResults(Bundle partialResults)
        {
            IList<string> matches = partialResults.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            pleaseRecordView.Text = matches[0];
            pleaseRecordView.SetTextColor(Android.Graphics.Color.ParseColor("#263238"));
        }

        public void OnReadyForSpeech(Bundle @params)
        {
        }

        public void OnResults(Bundle results)
        {
            IList<string> matches = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            alarmEditText.Text = matches[0];
            HandleVoiceRecognitionResult();
        }

        public void OnRmsChanged(float rmsdB)
        {
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == MY_PERMISSIONS_RECORD_AUDIO)
            {
                if (grantResults[0] != Permission.Granted)
                {
                    Toast.MakeText(ApplicationContext,
                            "Application will not have audio on record", ToastLength.Short).Show();
                }
                else
                {
                    StartVoiceRecognition();
                }
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void RequestRecordAudioPermission()
        {
            if (CheckCallingOrSelfPermission(Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                // Should we show an explanation?
                if (ShouldShowRequestPermissionRationale(Manifest.Permission.RecordAudio))
                {
                    // Explain to the user why we need to read the contacts
                    Toast.MakeText(this, "이 앱은 음성 녹음 권한이 필요합니다.", ToastLength.Long).Show();
                }

                RequestPermissions(new string[] { Manifest.Permission.RecordAudio }, MY_PERMISSIONS_RECORD_AUDIO);

                // MY_PERMISSIONS_REQUEST_READ_EXTERNAL_STORAGE is an
                // app-defined int constant that should be quite unique

                return;
            }
            else if (CheckCallingOrSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted)
            {
                StartVoiceRecognition();
            }
        }

        private void StartVoiceRecognition()
        {
            _soundService?.StopAudio();
            recordingLayout.Visibility = ViewStates.Visible;
            mSpeechRecognizer?.StartListening(mSpeechRecognizerIntent);
        }

        private class CountDown : CountDownTimer
        {
            public long CountDownInterval { get; }
            public long MillisInFuture { get; }
            public Activity Activity { get; set; }

            public CountDown(long millisInFuture, long countDownInterval, Activity activity) : base(millisInFuture, countDownInterval)
            {
                Activity = activity;
                MillisInFuture = millisInFuture;
                CountDownInterval = countDownInterval;
            }

            public override void OnFinish()
            {
                Activity.FinishAndRemoveTask();
            }

            public override void OnTick(long millisUntilFinished)
            {
                var alarmActivity = Activity as AlarmActivity;
                var countTextView = alarmActivity.countTextView;

                double count = (double)millisUntilFinished / 1000;
                var stringFormat = string.Format("{0:f2}", count);
                countTextView.Text = stringFormat;
            }
        }
    }

    
}