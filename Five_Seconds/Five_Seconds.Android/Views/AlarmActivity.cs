using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Five_Seconds.Droid.Services;
using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using static Android.Views.View;
using Button = Android.Widget.Button;
using RelativeLayout = Android.Widget.RelativeLayout;
using View = Android.Views.View;

namespace Five_Seconds.Droid
{
    [Activity(Label = "5초의 알람", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AlarmActivity : Activity, IRecognitionListener
    {
        readonly IPlaySoundService _soundService = new PlaySoundServiceAndroid();
        Vibrator _vibrator;

        Dialog feedbackDialog;

        private SpeechRecognizer mSpeechRecognizer;
        private Intent mSpeechRecognizerIntent;
        const int MY_PERMISSIONS_RECORD_AUDIO = 2357;
        private RelativeLayout alarmTextLayout;
        private LinearLayout tellAlarmLayout;
        private LinearLayout buttonLayout;
        private Button startButton;
        private Button laterButton;
        private ImageView tellmeView;
        public TextView countTextView;
        private TextView timeTextView;
        private TextView timeOutTextView;
        private TextView alarmTextView;
        private TextView pleaseSayText;
        private EditText alarmEditText;
        private View LaterAlarmDialog;
        private CountDown countDown;

        AdView adView;

        private int id;
        private string name;
        private string toneName;
        private bool IsAlarmOn;
        private bool IsVibrateOn;
        private bool IsCountOn = true;
        private bool IsCountSoundOn = true;
        private bool IsRepeating;
        private int alarmVolume;
        private bool IsLaterAlarm = false;

        private DateTime AlarmTimeNow;
        private bool IsSuccess = false;

        private bool IsPausePassed;
        private bool IsFinished;

        Alarm alarm;
        IAlarmsRepository alarmsRepo;

        public Handler Handler => new Handler();

        public AlarmActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AlarmTimeNow = DateTime.Now;

            Bundle bundle = Intent.Extras;

            id = (int)bundle.Get("id");

            if (id == -100000)
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
            SetAndFindViewById();

            AddWindowManagerFlags();

            SetAlarmAfterCalled();

            CreateSpeechRecognizer();

            SetIsFailedCountDown();

            if (bundle == null) return;
        }

        private void SetIsFailedCountDown()
        {
            countDown = new CountDown(60000, 1000, this, false);
            countDown.Start();
        }

        private void OnlyCountDown()
        {
            SetContentView(Resource.Layout.AlarmActivity);
            SetControlsForCountActivity();
            ShowCountActivity();
        }

        private void SetAlarmAfterCalled()
        {

            alarmsRepo = App.AlarmsRepo;

            alarm = App.AlarmsRepo?.GetAlarm(id);
            alarm.Days = App.AlarmsRepo?.GetDaysOfWeek(alarm.DaysId);

            if (IsLaterAlarm)
            {
                return;
            }

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
            IsLaterAlarm = (bool)bundle.Get("IsLaterAlarm");
        }


        private void SetAndFindViewById()
        {
            alarmTextLayout = FindViewById<RelativeLayout>(Resource.Id.alarmTextLayout);
            tellAlarmLayout = FindViewById<LinearLayout>(Resource.Id.tellAlarmLayout);
            buttonLayout = FindViewById<LinearLayout>(Resource.Id.buttonLayout);
            startButton = FindViewById<Button>(Resource.Id.startButton);
            laterButton = FindViewById<Button>(Resource.Id.laterButton);
            tellmeView = FindViewById<ImageView>(Resource.Id.tellmeView);
            countTextView = FindViewById<TextView>(Resource.Id.countTextView);
            timeTextView = FindViewById<TextView>(Resource.Id.timeTextView);
            alarmTextView = FindViewById<TextView>(Resource.Id.alarmTextView);
            pleaseSayText = FindViewById<TextView>(Resource.Id.pleaseSayText);
            alarmEditText = FindViewById<EditText>(Resource.Id.alarmEditText);
            timeOutTextView = FindViewById<TextView>(Resource.Id.timeOutTextView);

            timeOutTextView.Text = "60초 이내";
            countTextView.Text = "5.00";
            alarmTextView.Text = name;
            timeTextView.Text = AlarmTimeNow.ToShortTimeString();

            tellmeView.Click += StartListening_Click;
            startButton.Click += StartButton_Click;
            laterButton.Click += ShowLaterDialog;
        }

        private void SetControlsForCountActivity()
        {
            alarmTextLayout = FindViewById<RelativeLayout>(Resource.Id.alarmTextLayout);
            tellAlarmLayout = FindViewById<LinearLayout>(Resource.Id.tellAlarmLayout);
            buttonLayout = FindViewById<LinearLayout>(Resource.Id.buttonLayout);
            countTextView = FindViewById<TextView>(Resource.Id.countTextView);
            countTextView.Text = "5.00";
        }

        private void AddWindowManagerFlags()
        {
            // add flags to turn screen on and appear over lock screen
            var pm = GetSystemService(PowerService) as PowerManager;

            if (pm.IsInteractive)
            {
                IsPausePassed = true;
            }

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
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 3000);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 3000);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);

            mSpeechRecognizer.SetRecognitionListener(this);
        }

        private void StartListening_Click(object sender, EventArgs e)
        {
            //RequestRecordAudioPermission();
            SendBroadcast(new Intent(Intent.ActionBootCompleted));
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            HandleVoiceRecognitionResult();
        }

        private void HandleVoiceRecognitionResult()
        {
            startButton.Text = "시작!";

            var editText = alarmEditText.Text.Replace(" ", "");
            var textView = alarmTextView.Text.Replace(" ", "");

            if (editText == textView)
            {
                countDown.Cancel();

                IsSuccess = true;

                ShowFeedbackDialog();
            }
            else
            {
                pleaseSayText.Text = "라고 적어주세요";
                SetVisibilityOfControls();
                alarmEditText.RequestFocus();
                InputMethodManager imm = GetSystemService(InputMethodService) as InputMethodManager;
                imm.ShowSoftInput(alarmEditText, ShowFlags.Implicit);
                alarmEditText.SetSelection(alarmEditText.Text.Length);
                ShowToastDoNotMatchText();
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
                    _soundService?.PlayCountAudio();
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
            alarmEditText.Visibility = ViewStates.Visible;
            startButton.Visibility = ViewStates.Visible;
        }

        private void ShowToastDoNotMatchText()
        {
            Toast.MakeText(ApplicationContext, "알람 이름이 일치하지 않습니다. 알람 이름을 정확히 기입해주세요", ToastLength.Long).Show();
        }

        public void ShowFeedbackDialog()
        {
            var Records = App.AlarmsRepo.RecordFromDB;

            var alarmRecords = Records.FindAll(a => a.AlarmId == alarm.Id);
            var successRecords = alarmRecords.FindAll(a => a.IsSuccess == true);

            double successRate;

            feedbackDialog = new Dialog(this);
            feedbackDialog.SetContentView(Resource.Layout.AlarmFeedbackDialog);

            feedbackDialog.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));

            TextView titleText = feedbackDialog.FindViewById<TextView>(Resource.Id.titleText);
            TextView messageText = feedbackDialog.FindViewById<TextView>(Resource.Id.messageText);
            Button confirmBtn = feedbackDialog.FindViewById<Button>(Resource.Id.confirmBtn);
            adView = feedbackDialog.FindViewById<AdView>(Resource.Id.adView);


            SetAdView();


            if (IsSuccess)
            {
                titleText.Text = "알람 성공!";
                successRate = (double)(successRecords.Count + 1) / (alarmRecords.Count + 1);
            }
            else
            {
                titleText.Text = "알람 실패 ㅠ^ㅠ";
                successRate = (double)successRecords.Count / (alarmRecords.Count + 1);
            }

            if (alarmRecords.Count == 0)
            {
                messageText.Text = $"이번이 첫 알람이네요!";
            }
            else
            {
                messageText.Text = $"'{alarm.Name}' 알람의 전체 성공률은 {successRate * 100:0.##}% 입니다."; 
            }


            if (IsCountOn && IsSuccess)
            {
                confirmBtn.Text = "5초 카운트";
            }
            else
            {
                confirmBtn.Text = "확인";
            }

            confirmBtn.Click += ConfirmBtn_Click;

            feedbackDialog.Show();
        }

        private void ConfirmBtn_Click(object sender, EventArgs e)
        {
            IsFinished = true;

            if (IsCountOn && IsSuccess)
            {
                feedbackDialog.Dismiss();
                ShowCountActivity();
            }
            else
            {
                feedbackDialog.Dismiss();
                FinishAndRemoveTask();
            }
        }
        private void SetAdView()
        {
            SetMobileAds();
            var requestbuilder = new AdRequest.Builder().AddTestDevice("FA3E0133F649B126EB4B86A6DA3E60D2").Build();
            adView.AdListener = new AdListener(this);
            adView.LoadAd(requestbuilder);
        }
        private void SetMobileAds()
        {
            MobileAds.Initialize(ApplicationContext, GetString(Resource.String.admob_app_id));
        }

        private void SetMediaPlayer()
        {
            if (IsAlarmOn)
            {
                AlarmTone alarmTone = AlarmTone.Tones.Find(a => a.Name == toneName);
                _soundService?.PlayAudio(alarmTone, true, alarmVolume);
            }
        }

        private void SetVibrator()
        {
            if (IsVibrateOn)
            {
                _vibrator = Vibrator.FromContext(this);
                long[] mVibratePattern = new long[] { 0, 400, 1000, 600, 1000, 800, 1000, 1000 };
                VibrationEffect effect = VibrationEffect.CreateWaveform(mVibratePattern, 0);
                _vibrator?.Vibrate(effect);
            }
        }

        private void HideAllViewExceptForCountText()
        {
            alarmTextLayout.Visibility = ViewStates.Invisible;
            tellAlarmLayout.Visibility = ViewStates.Invisible;
            buttonLayout.Visibility = ViewStates.Invisible;
            countTextView.Visibility = ViewStates.Visible;
        }

        private async void SetCountDown()
        {
            await Task.Delay(330);
            countDown = new CountDown(5000, 10, this, true);
            countDown.Start();
        }

        public override void OnBackPressed()
        {
        }

        //protected override void OnUserLeaveHint()
        //{
        //    IsPausePassed = true;
        //    //notification으로 알람 울리는 중 표시
        //}

        public override void FinishAndRemoveTask()
        {
            if (alarm != null && IsFinished)
            {
                CreateRecord();
            }

            base.FinishAndRemoveTask();
        }

        private void CreateRecord()
        {
            alarm.Time = new TimeSpan(AlarmTimeNow.Hour, AlarmTimeNow.Minute, 0);
            var record = new Record(alarm, IsSuccess);
            alarmsRepo.SaveRecord(record);
        }

        public void OnBeginningOfSpeech()
        {
        }

        public void OnBufferReceived(byte[] buffer)
        {
        }

        public void OnEndOfSpeech()
        {
            mSpeechRecognizer?.StopListening();

            SetTellmeBtnDefault();
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
                    SetTellmeBtnDefault();
                    Toast.MakeText(ApplicationContext, "오디오 녹음 권한을 허용해주세요", ToastLength.Long).Show();
                    break;

                case SpeechRecognizerError.Network:
                    SetTellmeBtnDefault();
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
                    SetTellmeBtnDefault();
                    Toast.MakeText(ApplicationContext, "서버에 문제가 있습니다. 텍스트 창에 해야할 일을 적어주세요.", ToastLength.Long).Show();
                    break;

                case SpeechRecognizerError.SpeechTimeout:
                    SetTellmeBtnDefault();
                    Toast.MakeText(ApplicationContext, "음성 녹음 시간이 초과되었습니다. 다시 시도해주세요.", ToastLength.Long).Show();
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
            //IList<string> matches = partialResults.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
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
                            "이 앱은 음성 녹음 권한이 필요합니다.", ToastLength.Short).Show();
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
            TurnOffSoundAndVibration();
            SetTellmeBtnRecording();
            mSpeechRecognizer?.StartListening(mSpeechRecognizerIntent);
        }

        private void ShowLaterDialog(object s, EventArgs e)
        {
            TurnOffSoundAndVibration();

            LaterAlarmDialog = LayoutInflater.Inflate(Resource.Layout.LaterAlarmDialog, (ViewGroup)FindViewById(Resource.Id.laterAlarmLayout));

            var numberPicker = LaterAlarmDialog.FindViewById<NumberPicker>(Resource.Id.laterNumberPicker);
            numberPicker.MaxValue = 120;
            numberPicker.MinValue = 1;
            numberPicker.Value = 5;

            AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            dialog.SetTitle($"{AlarmTimeNow.ToShortTimeString()}에서");
            dialog.SetView(LaterAlarmDialog);
            dialog.SetPositiveButton("확인", (c, ev) =>
            {
                var numberPicker1 = LaterAlarmDialog.FindViewById<NumberPicker>(Resource.Id.laterNumberPicker);
                var number = numberPicker1.Value;

                var dateTimeNow = DateTime.Now;
                var alarmTime = AlarmTimeNow.AddMinutes(number);


                var diffTimeSpan = alarmTime.Subtract(dateTimeNow);

                if (diffTimeSpan.Ticks < 0)
                {
                    Toast.MakeText(ApplicationContext, $"이미 지난 시각은 설정할 수 없습니다.", ToastLength.Long).Show();
                    return;
                }

                SetAlarmByManager((long)diffTimeSpan.TotalMilliseconds);

                Toast.MakeText(ApplicationContext, CreateDateString.CreateTimeRemainingString(alarmTime), ToastLength.Long).Show();

                FinishAndRemoveTask();
            });
            dialog.SetNegativeButton("취소", (c, ev) =>
            {
            });

            AlertDialog alert = dialog.Create();

            alert.Show();

            dialog.Dispose();
        }

        private void SetAlarmByManager(long diffMillis)
        {
            var _alarmIntent = new Intent(ApplicationContext, typeof(AlarmReceiver));
            _alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            _alarmIntent.PutExtra("id", id);
            _alarmIntent.PutExtra("name", name);
            _alarmIntent.PutExtra("IsAlarmOn", IsAlarmOn);
            _alarmIntent.PutExtra("IsVibrateOn", IsVibrateOn);
            _alarmIntent.PutExtra("IsCountSoundOn", IsCountSoundOn);
            _alarmIntent.PutExtra("IsCountOn", IsCountOn);
            _alarmIntent.PutExtra("IsRepeating", IsRepeating);
            _alarmIntent.PutExtra("toneName", toneName);
            _alarmIntent.PutExtra("alarmVolume", alarmVolume);
            _alarmIntent.PutExtra("IsLaterAlarm", true);

            var pendingIntent = PendingIntent.GetBroadcast(ApplicationContext, -alarm.Id, _alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)ApplicationContext.GetSystemService("alarm");

            Intent showIntent = new Intent(ApplicationContext, typeof(MainActivity));
            PendingIntent showOperation = PendingIntent.GetActivity(ApplicationContext, 0, showIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager.AlarmClockInfo alarmClockInfo = new AlarmManager.AlarmClockInfo(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + diffMillis, showOperation);
            alarmManager.SetAlarmClock(alarmClockInfo, pendingIntent);
        }

        private void SetTellmeBtnDefault()
        {
            tellmeView.SetImageResource(Resource.Drawable.ic_mic);
            tellmeView.SetBackgroundResource(Resource.Drawable.rounded_button);
        }

        private void SetTellmeBtnRecording()
        {
            tellmeView.SetImageResource(Resource.Drawable.ic_settings_voice);
            tellmeView.SetBackgroundResource(Resource.Drawable.background_tellmebtn_recording);
        }

        private void TurnOffSoundAndVibration()
        {
            _soundService?.StopAudio();
            _vibrator?.Cancel();
        }

        protected override void OnPause()
        {
            if (IsPausePassed)
            {
                TurnOffSoundAndVibration();
            }
            else
            {
                IsPausePassed = true;
            }
            base.OnPause();
        }

        private class CountDown : CountDownTimer
        {
            public long CountDownInterval { get; }
            public long MillisInFuture { get; }
            public AlarmActivity Activity { get; set; }

            private bool IsCountDown { get; set; }

            public CountDown(long millisInFuture, long countDownInterval, Activity activity, bool isCountDown) : base(millisInFuture, countDownInterval)
            {
                Activity = activity as AlarmActivity;
                MillisInFuture = millisInFuture;
                CountDownInterval = countDownInterval;
                IsCountDown = isCountDown;
            }

            public override void OnFinish()
            {
                if (IsCountDown)
                {
                    Activity.FinishAndRemoveTask();
                }
                else
                {
                    Activity.ShowFeedbackDialog();
                }
            }

            public override void OnTick(long millisUntilFinished)
            {
                var alarmActivity = Activity as AlarmActivity;
                double count = (double)millisUntilFinished / 1000;

                if (IsCountDown)
                {
                    var countTextView = alarmActivity.countTextView;

                    var stringFormat = string.Format("{0:f2}", count);
                    countTextView.Text = stringFormat;
                }
                else
                {
                    var timeOutTextView = alarmActivity.timeOutTextView;


                    var stringFormat = $"{count:00}초 이내";
                    timeOutTextView.Text = stringFormat;
                }
            }
        }
        private class AdListener : Android.Gms.Ads.AdListener
        {
            private readonly AlarmActivity that;

            public AdListener(AlarmActivity t)
            {
                that = t;
            }

            public override void OnAdLoaded()
            {
                that.adView.Visibility = ViewStates.Visible;
                base.OnAdLoaded();
            }

            public override void OnAdFailedToLoad(int errorCode)
            {
                that.adView.Visibility = ViewStates.Gone;
                base.OnAdFailedToLoad(errorCode);
            }
        }
    }

    
}